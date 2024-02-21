using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Context;
using OfficialMemo.Models;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Dto;
using OfficialMemo.Models.Poco;
using OfficialMemo.Models.ProcessModels.Poco;
using OfficialMemo.Services;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OfficialMemo.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProcessesController : ControllerBase
{
    private readonly ProcessesDbService _processesDbService;
    private readonly IMapper _mapper;
    private readonly DataContext _dbContext;

    public ProcessesController(
        ProcessesDbService processesDbService,
        IMapper mapper,
        DataContext dbContext
    )
    {
        _processesDbService = processesDbService;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<ProcessInfo>> GetProcessInfo(Guid requestGuid)
    {
        var result = await _processesDbService.GetProcessInfo(requestGuid);
        if (result is null)
            return NoContent();
        return Ok(result);
    }

    [HttpGet("getProcessInfoByProcessGuid")]
    public async Task<ActionResult<ProcessInfo>> GetProcessInfoByProcessGuid(Guid processGuid)
    {
        var result = await _processesDbService.GetProcessInfoByProcessGuid(processGuid);
        if (result is null)
            return NoContent();
        return Ok(result);
    }

    [HttpGet("getProcessStatus")]
    public async Task<ActionResult<string>> GetProcessStatus(Guid processGuid)
    {
        var result =
            await _dbContext.OfficialMemoProcessData.FirstOrDefaultAsync(dbo => dbo.ProcessGuid == processGuid);
        if (result is null)
            return NoContent();
        return Ok(result.ProcessStatus);
    }

    [HttpPost("all")]
    public async Task<ActionResult<IEnumerable<ProcessReportDto>>> AllMessages(
        [FromBody] FilterRules filterRules,
        bool isBranch,
        [FromServices] EmployeesDbService employeesDbService
    )
    {
        if (isBranch)
        {
            var branchCode = await employeesDbService.GetEmployeeBranchCodeByLoginAsync(User.Identity!.Name!);
            if (branchCode == null) return BadRequest("Employee was not found");
            filterRules.BranchCode!.Operand = branchCode;
        }

        return await
            _dbContext.ProcessReports.Filter(filterRules!).FilterDocumentStatus(filterRules)
                .ProjectToType<ProcessReportDto>().AsNoTracking().ToListAsync();
    }


    [HttpPost("get_user_process_messages")]
    public async Task<List<ProcessReportDto>> GetUserProcessMessages(
        [FromBody] FilterRules filterRules,
        string? status
    )
    {
        var userCode = User.Identity?.Name ?? "";


        var query = _dbContext.UserProcessReports
            .Where(e => e.UserCode == userCode)
            .AsNoTracking()
            .Filter(filterRules!)
            .FilterDocumentStatus(filterRules);

        if (status == "Completed")
            query = query.Where(e => e.ProcessStatus == status);
        else
            query = query.Where(e => e.ProcessStatus != "Completed");

        var result = await query
            .ProjectToType<ProcessReportDto>()
            .ToListAsync();

        return result;
    }


    [HttpGet("messageHistory")]
    public async Task<ActionResult<List<ProcessMessage>>> ClientRequestHistory(Guid processGuid)
    {
        var messages = await GetMessages(processGuid);
        if (!messages.Any())
            return NotFound();

        messages.AddRange(await GetOffMemoRelatedHistory(processGuid));
        messages = messages.DistinctBy(msg => msg.Id).ToList();

        var dict = new Dictionary<Guid, ProcessMessage>(messages.Count);
        var excludeProcessGuidsAndOrigins = new Dictionary<Guid, Guid>(messages.Count);
        foreach (var message in messages)
        {
            #region comment

            //var message = _mapper.Map<ProcessMessage2>(messageDbo);
            // if (message.Data != null)
            // {
            //     var task = JsonSerializer.Deserialize<ProcessTask>(message.Data);
            //     if (task is not null)
            //     {
            //         message.MessageDocuments ??= task.TaskDocuments;
            //         message.MessageComment ??= task.TaskComment;
            //         message.UserCode ??= task.RepliedBy;
            //         message.UserName ??= task.RepliedByName;
            //         message.Data = null;
            //         message.ReplyComment ??= task.ReplyComment;
            //         message.ReplyDocuments = task.ReplyDocuments;
            //         message.ApprovalDocuments = task.ReplyDocuments;
            //     }
            // }

            #endregion

            if (!dict.ContainsKey(message.Id))
                dict.Add(message.Id, message);

            var parent =
                message.PreviousTaskGuid ?? message.ParentGuid ?? message.RequestGuid ?? Guid.Empty;
            if (message.MessageType?.ToLower() == "task" && dict.ContainsKey(parent))
            {
                dict[parent].Children ??= new List<ProcessMessage>();
                dict[parent].Children!.Add(message);
            }
        }

        foreach (var item in excludeProcessGuidsAndOrigins)
        {
            foreach (var message2 in messages)
            {
                try
                {
                    if (
                        (message2.ProcessGuid != item.Key || (message2.MessageType == "task"))
                        || !dict.ContainsKey(item.Value)
                    )
                        continue;
                    dict[item.Value].Children ??= new List<ProcessMessage>();
                    dict[item.Value].Children!.Add(message2);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        var res = dict.Select(x => x.Value)
            .Where(
                x =>
                    x.MessageType != "task"
                    && !excludeProcessGuidsAndOrigins.ContainsKey(x.ProcessGuid)
            )
            .OrderBy(x => x.MessageDate)
            .Select(
                x =>
                    x.Children == null
                        ? x
                        : x with
                        {
                            ChildCount = x.Children.Count(
                                message2 =>
                                    message2.MessageStatus != "completed"
                                    && message2.MessageStatus != "redirected"
                                    && message2.MessageStatus != "approved"
                                    && message2.MessageStatus != "canceled"
                            )
                        }
            )
            .ToList();

        return Ok(res);
    }

    private async Task<List<ProcessMessage>> GetMessages(Guid processGuid)
    {
        if (processGuid == Guid.Empty)
            return Enumerable.Empty<ProcessMessage>().ToList();

        var messages = await _dbContext.MessageHistories
            .Where(p => p.ProcessGuid == processGuid)
            .OrderBy(p => p.MessageDate)
            .AsNoTracking()
            .ToListAsync();
        return messages.Any() ? messages : Enumerable.Empty<ProcessMessage>().ToList();
    }

    private async Task<List<ProcessMessage>> GetOffMemoRelatedHistory(Guid processGuid)
    {
        var inMessageProcessGuid = await _dbContext.OfficialMemoProcessData
            .Where(outProcessData => outProcessData.ProcessGuid == processGuid)
            .Include(outProcessData => outProcessData.OfficialMemo)
            .Select(outProcessData => outProcessData.OfficialMemo.ProcessData.ProcessGuid)
            .FirstOrDefaultAsync();

        return await GetMessages(inMessageProcessGuid);
    }
}