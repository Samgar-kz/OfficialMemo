using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Models;
using OfficialMemo.Services;
using OfficialMemo.Context;
using OfficialMemo.Services.RestClients;
using OfficialMemo.Extensions;
using OfficialMemo.Models.Xml;
using OfficialMemo.Models.Poco;
using OfficialMemo.Models.Dto;
using PdfProcessor.Client;
using OfficialMemo.Interfaces;
using Microsoft.AspNetCore.SignalR.Protocol;
using OfficialMemo.Logging;
using FileServer.Client;
using Signer.Services;
using PdfProcessor.Models;
using Signer.Models;
using LocalizableStringLib;
using Employee = OfficialMemo.Models.Poco.Employee;
using SignType = OfficialMemo.Models.Dbo.SignType;
using System.Linq;
using HRServices;
using SignMessageDto = OfficialMemo.Models.Dto.SignMessageDto;
using Document = OfficialMemo.Models.Poco.Document;
using OfficialMemo.Models.ProcessModels.Poco;
using System.ServiceModel.Channels;

// using FileDownloader = DocExchange.Shared.Services.FileDownloader;

namespace OfficialMemo.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OfficialMemoController : ControllerBase
{
    private readonly ProcessService _processService;
    private readonly ProcessesDbService _processesDbService;
    private readonly DataContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<OfficialMemoController> _logger;
    private readonly IHRClient _hrClient;

    public OfficialMemoController(IMapper mapper, ProcessService processService, DataContext dbContext, ILogger<OfficialMemoController> logger, IHRClient hrClient,
        ProcessesDbService processesDbService)
    {
        _processService = processService;
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
        _hrClient = hrClient;
        _processesDbService= processesDbService;
    }

    [HttpPost]
    public async Task<ActionResult> Create(OfficialMemoCreateDto officialMemoShortDto,
        [FromServices] PdfApiClient pdfApiClient,
        [FromServices] FileServerApiClient fileServerApiClient, [FromServices] ProcessesDbService processesDbService,
        [FromServices] EmployeesDbService employeesDbService, [FromServices] FileServerClient fileServerClient)
    {
        string? documentUrl = null;
        if (!string.IsNullOrEmpty(officialMemoShortDto.HtmlDocument))
        {
            var pdfResult = await pdfApiClient.GenerateFromHtml(officialMemoShortDto.HtmlDocument);
            var file = new FormFile(pdfResult.ContentStream, 0, pdfResult.Size, pdfResult.Name, pdfResult.Name);
            documentUrl = await fileServerApiClient.UploadFileAsync(file);
        }

        var regNum = $"СЗ-{await processesDbService.NextRegNum("OfficialMemo", "OfficialMemo")}";


        //var text = $"№{regNum}";

        //var documentWithRegisterRegNum = await pdfApiClient.InsertTextToUrl(
        //    new FileServer.Abstractions.Document("", documentUrl), text,
        //    new TextInsertOptions
        //    {
        //        X = 87,
        //        Y = 59,
        //        FontName = "Times New Roman",
        //        PageNumber = 1,
        //        Bold = true,
        //    });
        //if (documentWithRegisterRegNum is null) throw new Exception("InsertTextToUrl returned null value");

        //documentWithRegisterRegNum.ContentStream.Seek(0, SeekOrigin.Begin);
        //var concatenatedPdfLoaded = await fileServerClient.UploadFileAsync(documentWithRegisterRegNum);

        //documentUrl = concatenatedPdfLoaded.Url;

        var officialMemo = officialMemoShortDto.Adapt<OfficialMemoModel>();

        // var initiator = await employeesDbService.GetEmployeeDboByCodeAsync(User.Identity!.Name!);
        var signer = await employeesDbService.GetEmployeeDboByCodeAsync(officialMemoShortDto.Data.SignerCode);
        if (signer is null)
            return BadRequest("Signer not found");


        var officialMemoDbo = officialMemo.Adapt<OfficialMemoDbo>() with
        {
            ExecutorCode = officialMemoShortDto.Data.Executor.Login,
            RegistrarCode = signer.ParentDepartmentCode?.Length < 3 && signer.ParentDepartmentCode != "18" &&
                            signer.BranchCode is not null
                ? "DOC_CLERK_" + signer.BranchCode
                : "ROLE_DOC2_052",
            SignerCode = officialMemoShortDto.Data.Signer!.Login,
            Signer = null!,
            Executor = null!,
            DocumentUrl = documentUrl,
            OriginalDocumentUrl = documentUrl
        };

        if (officialMemoShortDto.Data.ApproverCodes is not null && officialMemoShortDto.Data.ApproverCodes!.Any())
        {
            officialMemoDbo.Approvers = (await _dbContext.Employees.GetFullInfoAsync(
                officialMemoShortDto.Data.ApproverCodes.Select(code => new EmployeeDbo { Login = code }))).ToList();
            foreach (var approver in officialMemoDbo.OfficialMemoApprovers!)
            {
                _dbContext.Entry(approver).State = EntityState.Added;
            }
        }

        officialMemoDbo.Recipients = (await _dbContext.Employees.GetFullInfoAsync(
                officialMemoShortDto.Data.RecipientCodes.Select(code => new EmployeeDbo { Login = code }))).ToList();


        foreach (var recipient in officialMemoDbo.OfficialMemoRecipients)
        {
            _dbContext.Entry(recipient).State = EntityState.Added;
        }

        var dueToDate = officialMemoShortDto.Data.DueToDate;
        officialMemoDbo.DueToDate = dueToDate ?? DateTime.MaxValue;

        officialMemoDbo.ApprovalRequired =
            officialMemoShortDto.Data.Approvers is not null && officialMemoShortDto.Data.Approvers.Any();

        officialMemoDbo.RegistrarRequired = IsManagement(officialMemoShortDto.Data.RecipientCodes) && await IsBranchStaff(officialMemoShortDto.Data.Executor.Login);
        officialMemoDbo.ConfidenceType =
            await _dbContext.ConfidenceTypes.FirstOrDefaultAsync(dbo =>
                dbo.Id == officialMemoShortDto.Data.ConfidenceType.Id);
            
        officialMemoDbo.MessageDate = DateTime.Now;
        officialMemoDbo.MessageGuid = Guid.NewGuid();
        officialMemoDbo.RegNum = regNum;



        _dbContext.OfficialMemos.Add(officialMemoDbo);
        _dbContext.Entry(officialMemoDbo.ConfidenceType!).State = EntityState.Unchanged;
        await _dbContext.SaveChangesAsync();

        var officialMemoXml = officialMemoDbo.Adapt<OfficialMemoStart>() with
        {
            Signer = new EmployeeXml { Code = officialMemoDbo.SignerCode },
            RegistrarCode = officialMemoDbo.RegistrarCode,
            Executor = new EmployeeXml { Code = officialMemoDbo.ExecutorCode },
            //Recipients = new List<RecipientXml> { new RecipientXml { Code = } },
        };
        if (officialMemoShortDto.Data.ApprovalMode == "serial" && officialMemo.Data.Approvers is not null &&
            officialMemo.Data.Approvers.Any())
        {
            officialMemoXml = officialMemoXml with
            {
                Approvers = new List<ApproverXml> { GetApproversTree(officialMemo.Data.Approvers) },
            };
        }

        await _processService.StartProcess(officialMemoXml, officialMemoShortDto.Data.Executor.Login ?? "Unknown", "OfficialMemo");

        return Ok(regNum);
    }

    private bool IsManagement(List<string> logins)
    {
        foreach (var login in logins)
        {
            var isManagement = _dbContext.Employees.AsNoTracking().Any(dbo => dbo.Login == login && dbo.WorkStatus != "H_DISMISSED" &&
                (dbo.PositionCode == "1939" ||
                dbo.PositionCode == "1974" ||
                dbo.PositionCode == "2057" ||
                dbo.PositionCode == "2487"));

            if (!isManagement) return false;
        }
        
        return true;
    }
    
    private async Task<bool> IsBranchStaff(string login)
    {
        var isManagement = await _hrClient.CheckCAAsync(login);
        return !isManagement;
    }

    private static ApproverXml GetApproversTree(IReadOnlyList<Employee> employees)
    {
        var root = new ApproverXml { Code = employees[0].Login };
        var nextNode = root;

        for (var i = 1; i < employees.Count; i++)
        {
            nextNode.Next = new ApproverXml { Code = employees[i].Login };
            nextNode = nextNode.Next;
        }

        return root;
    }

    [HttpPost("update/{requestGuid:guid}")]
    public async Task<ActionResult> Update(OfficialMemoCreateDto officialMemoDto, Guid requestGuid,
        [FromServices] PdfApiClient pdfApiClient,
        [FromServices] FileServerApiClient fileServerApiClient, [FromServices] EmployeesDbService employeesDbService,
        [FromServices] ISignRepository signRepository, [FromServices] FileServerClient fileServerClient)
    {
        string? documentUrl = null;

        if (!string.IsNullOrEmpty(officialMemoDto.HtmlDocument))
        {
            var pdfResult = await pdfApiClient.GenerateFromHtml(officialMemoDto.HtmlDocument);
            var file = new FormFile(pdfResult.ContentStream, 0, pdfResult.Size, pdfResult.Name, pdfResult.Name);
            documentUrl = await fileServerApiClient.UploadFileAsync(file);
        }

        //var text = $"№{officialMemoDto.Data.RegNum}";

        //var documentWithRegisterRegNum = await pdfApiClient.InsertTextToUrl(
        //    new FileServer.Abstractions.Document("", documentUrl), text,
        //    new TextInsertOptions
        //    {
        //        X = 87,
        //        Y = 59,
        //        FontName = "Times New Roman",
        //        PageNumber = 1,
        //        Bold = true,
        //    });
        //if (documentWithRegisterRegNum is null) throw new Exception("InsertTextToUrl returned null value");

        //documentWithRegisterRegNum.ContentStream.Seek(0, SeekOrigin.Begin);
        //var concatenatedPdfLoaded = await fileServerClient.UploadFileAsync(documentWithRegisterRegNum);

        //documentUrl = concatenatedPdfLoaded.Url;


        var request = await _processService.GetMessageDocument<OfficialMemoBase>(requestGuid);

        signRepository.Delete(request.Id);

        // var initiator = await employeesDbService.GetEmployeeDboByCodeAsync(User.Identity!.Name!);
        var signer = await employeesDbService.GetEmployeeDboByCodeAsync(officialMemoDto.Data.Signer!.Login);
        if (signer is null)
            return BadRequest("Signer not found");

        var officialMemo = officialMemoDto.Adapt<OfficialMemoModel>();
        var officialMemoDbo = officialMemo.Adapt<OfficialMemoDbo>() with
        {
            ExecutorCode = User.Identity!.Name!,
            SignerCode = officialMemoDto.Data.Signer!.Login,
            Signer = null!,
            Executor = null!,
            SignData = null,
            ApprovalResults = officialMemoDto.Data.ApprovalResults.Adapt<List<ApprovalResultDbo>>(),
            //ReceivingResults = officialMemoDto.Data.ReceivingResults.Adapt<List<ReceivingResultDbo>>(),
            RegistrarCode = signer.ParentDepartmentCode?.Length < 3 && signer.ParentDepartmentCode != "18" &&
                            signer.BranchCode is not null
                ? "DOC_CLERK_" + signer.BranchCode
                : "ROLE_DOC2_052",
            DocumentUrl = documentUrl,
            OriginalDocumentUrl = documentUrl,
            Recipients = (await _dbContext.Employees.AsNoTracking()
                .Where(dbo => officialMemoDto.Data.RecipientCodes.Contains(dbo.Login))
                .ToListAsync())
        };

        if (officialMemoDto.Data.ApproverCodes is not null && officialMemoDto.Data.ApproverCodes!.Any())
        {
            officialMemoDbo.Approvers = (await _dbContext.Employees
                .Where(dbo => officialMemoDto.Data.ApproverCodes!.Contains(dbo.Login))
                .ToListAsync());

            var existingEntryApprover = _dbContext.OfficialMemoApprovers
                   .Where(e => e.OfficialMemoDboMessageGuid == officialMemoDbo.MessageGuid);

            foreach (var approver in existingEntryApprover)
            {
                _dbContext.Entry(approver).State = EntityState.Deleted;
            }

            foreach (var approver in officialMemoDbo.OfficialMemoApprovers!)
            {
                _dbContext.Entry(approver).State = EntityState.Added;
            }
        }

        if(officialMemoDbo.ApprovalResults is not null)
        {
            var existingEntryApproval = _dbContext.ApprovalResults
                   .Where(e => e.MessageId == officialMemoDbo.MessageGuid);

            foreach (var approver in existingEntryApproval)
            {
                _dbContext.Entry(approver).State = EntityState.Deleted;
            }
            officialMemoDbo.ApprovalResults = null;
        }

        if (officialMemoDbo.ReceivingResults is not null)
        {
            var existingEntryReceiving = _dbContext.ReceivingResults
                   .Where(e => e.MessageId == officialMemoDbo.MessageGuid);

            foreach (var receiver in existingEntryReceiving)
            {
                _dbContext.Entry(receiver).State = EntityState.Deleted;
            }
            officialMemoDbo.ReceivingResults = null;
        }


        officialMemoDbo.ApprovalRequired = officialMemoDto.Data.Approvers is not null && officialMemoDto.Data.Approvers.Any();

        officialMemoDbo.RegistrarRequired = IsManagement(officialMemoDto.Data.RecipientCodes) && await IsBranchStaff(officialMemoDto.Data.Executor.Login);

        var existingEntryRecipient = _dbContext.OfficialMemoRecipients
                  .Where(e => e.OfficialMemoDboMessageGuid == officialMemoDbo.MessageGuid);

        foreach (var recipient in existingEntryRecipient)
        {
            _dbContext.Entry(recipient).State = EntityState.Deleted;
        }

        foreach (var recipient in officialMemoDbo.OfficialMemoRecipients)
        {
            _dbContext.Entry(recipient).State = EntityState.Added;
        }

        _dbContext.Attach(officialMemoDbo);
        _dbContext.Entry(officialMemoDbo).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        var officialMemoXml = officialMemoDbo.Adapt<OfficialMemoStart>() with
        {
            Signer = new EmployeeXml { Code = officialMemoDbo.SignerCode },
            Executor = new EmployeeXml { Code = officialMemoDbo.ExecutorCode },
            RegistrarCode = officialMemoDbo.RegistrarCode,
        };
        if (officialMemoDto.Data.ApprovalMode == "serial" && officialMemo.Data.Approvers is not null &&
            officialMemo.Data.Approvers.Any())
        {
            officialMemoXml = officialMemoXml with
            {
                Approvers = new List<ApproverXml> { GetApproversTree(officialMemo.Data.Approvers) },
            };
        }

        var officialMemoRework = new OfficialMemoRework
        {
            Message = officialMemoXml,
            Reply = new ReplyXml
            {
                ReplyDecision = "accept",
                ReplyDecisionName = "Изменен",
                ReplyDate = DateTime.Now,
                RepliedBy = User.Identity!.Name!
            }
        };

        await _processService.SendReplyMessageAsync(officialMemoRework with
        {
            Code = "OfficialMemo",
            Version = "1.0",

        }, requestGuid, User.Identity!.Name!);

        return Ok(request.Id);
    }

    [HttpPost("updateByProcessGuid/{processGuid:guid}")]
    public async Task<ActionResult> UpdateByProcessGuid(OfficialMemoCreateDto officialMemoDto, Guid processGuid,
        [FromServices] PdfApiClient pdfApiClient,
        [FromServices] FileServerApiClient fileServerApiClient, [FromServices] EmployeesDbService employeesDbService,
        [FromServices] ISignRepository signRepository, [FromServices] FileServerClient fileServerClient, [FromServices] TasksService tasksService,
        [FromServices] ProcessesDbService processesDbService)
    {
        string? documentUrl = null;

        if (!string.IsNullOrEmpty(officialMemoDto.HtmlDocument))
        {
            var pdfResult = await pdfApiClient.GenerateFromHtml(officialMemoDto.HtmlDocument);
            var file = new FormFile(pdfResult.ContentStream, 0, pdfResult.Size, pdfResult.Name, pdfResult.Name);
            documentUrl = await fileServerApiClient.UploadFileAsync(file);
        }

        var signData = await signRepository.Get(officialMemoDto.Data.MessageGuid);

        if (signData is not null)
        {
            var regNumDate = GetRegNumDate(signData.SignedDate);

            var text = officialMemoDto.Data.Language.Equals("Қазақша", StringComparison.OrdinalIgnoreCase)
                    ? $"{regNumDate:dd.MM.yyyy}ж. №{officialMemoDto.Data.RegNum}"
                    : $"№{officialMemoDto.Data.RegNum} от {regNumDate:dd.MM.yyyy}г.";

            var documentWithRegisterRegNum = await pdfApiClient.InsertTextToUrl(
            new FileServer.Abstractions.Document("", documentUrl!), text,
            new TextInsertOptions
            {
                X = 87,
                Y = 59,
                FontName = "Times New Roman",
                PageNumber = 1,
                Bold = true,
            });
            if (documentWithRegisterRegNum is null) throw new Exception("InsertTextToUrl returned null value");

            documentWithRegisterRegNum.ContentStream.Seek(0, SeekOrigin.Begin);
            var concatenatedPdfLoaded = await fileServerClient.UploadFileAsync(documentWithRegisterRegNum);

            documentUrl = concatenatedPdfLoaded.Url;
        }
        

        var signer = await employeesDbService.GetEmployeeDboByCodeAsync(officialMemoDto.Data.Signer!.Login);
        if (signer is null)
            return BadRequest("Signer not found");

        var request = await _processService.GetMessageDocumentByProcessGuid<OfficialMemoBase>(processGuid);

        var officialMemo = officialMemoDto.Adapt<OfficialMemoModel>();
        var officialMemoDbo = officialMemo.Adapt<OfficialMemoDbo>() with
        {
            ExecutorCode = User.Identity!.Name!,
            DocumentUrl = documentUrl,
            OriginalDocumentUrl = documentUrl,
            RegistrarCode = signer.ParentDepartmentCode?.Length < 3 && signer.ParentDepartmentCode != "18" &&
                            signer.BranchCode is not null
                ? "DOC_CLERK_" + signer.BranchCode
                : "ROLE_DOC2_052",
            Recipients = (await _dbContext.Employees.AsNoTracking()
                .Where(dbo => officialMemoDto.Data.RecipientCodes.Contains(dbo.Login))
                .ToListAsync())
        };

        var result = await processesDbService.GetProcessInfoByProcessGuid(processGuid);
        if (result is null)
            return NoContent();

        if (officialMemoDto.Data.ApproverCodes is not null && officialMemoDto.Data.ApproverCodes!.Any())
        {
            officialMemoDbo.Approvers = (await _dbContext.Employees.GetFullInfoAsync(
                officialMemoDto.Data.ApproverCodes.Select(code => new EmployeeDbo { Login = code }))).ToList();

            var existingEntryApprover = _dbContext.OfficialMemoApprovers
                   .Where(e => e.OfficialMemoDboMessageGuid == officialMemoDbo.MessageGuid);

            foreach (var approver in existingEntryApprover)
            {
                _dbContext.Entry(approver).State = EntityState.Deleted;
            }

            foreach (var approver in officialMemoDbo.OfficialMemoApprovers!)
            {
                _dbContext.Entry(approver).State = EntityState.Added;
            }
        }

        officialMemoDbo.ApprovalRequired = officialMemoDto.Data.Approvers is not null && officialMemoDto.Data.Approvers.Any();

        officialMemoDbo.RegistrarRequired = IsManagement(officialMemoDto.Data.RecipientCodes) && await IsBranchStaff(officialMemoDto.Data.Executor.Login);

        officialMemoDbo.Recipients = (await _dbContext.Employees.GetFullInfoAsync(
                officialMemoDto.Data.RecipientCodes.Select(code => new EmployeeDbo { Login = code }))).ToList();


        var existingEntryRecipient = _dbContext.OfficialMemoRecipients
                  .Where(e => e.OfficialMemoDboMessageGuid == officialMemoDbo.MessageGuid);

        foreach (var recipient in existingEntryRecipient)
        {
            _dbContext.Entry(recipient).State = EntityState.Deleted;
        }

        foreach (var recipient in officialMemoDbo.OfficialMemoRecipients)
        {
            _dbContext.Entry(recipient).State = EntityState.Added;
        }

        _dbContext.Attach(officialMemoDbo);
        _dbContext.Entry(officialMemoDbo).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        await tasksService.UpdateDueToDateTasks(result.RequestGuid, officialMemoDbo.DueToDate!.Value);


        if (result.SchemeId == "OfficialMemoApprove" && officialMemoDto.Data.ApproverCodes is not null && officialMemoDto.Data.ApproverCodes!.Any())
        {
            await tasksService.UpdateSubrequests(processGuid, "subrequest_approve",
            officialMemoDbo.Approvers.Select(dbo => dbo.Login).ToArray());
        }

        if(result.SchemeId == "OfficialMemoPerform")
        {
            await tasksService.UpdateSubrequests(processGuid, "subrequest_receive",
                officialMemoDbo.Recipients.Select(dbo => dbo.Login).ToArray());
        }
            
        

        return Ok(request.Id);
    }

    private DateTime GetRegNumDate(DateTime? signDateTime)
    {
        if (signDateTime.HasValue)
        {
            int hourComparison = signDateTime.Value.Hour.CompareTo(18);
            if (hourComparison >= 0)
            {
                int minuteComparison = signDateTime.Value.Minute.CompareTo(30);

                if (minuteComparison > 0) return signDateTime.Value.Date.AddDays(1).AddHours(9);
            }
            return signDateTime.Value;
        }
        return DateTime.Now;
    }

    [HttpGet("receivingResults")]
    public async Task<ActionResult<List<ReceivingResultDto>>> ReceivingResults(Guid messageGuid)
    {
        var messages = await GetReceivingResults(messageGuid);
        if (!messages.Any())
            return NoContent();

        messages = messages.DistinctBy(msg => msg.Id).ToList();

        var dict = new Dictionary<string, ReceivingResultDto>(messages.Count);
        var excludeProcessGuidsAndOrigins = new Dictionary<Guid, Guid>(messages.Count);
        foreach (var message in messages)
        {
            if (!dict.ContainsKey(message.ReceiverCode))
            {
                dict.Add(message.ReceiverCode, message.Adapt<ReceivingResultDto>() with
                {
                    Receiver = message.Receiver.Adapt<EmployeeDto>(),
                    Created = message.Created,
                });
            }

        }

        //foreach (var item in countRepeat)
        //{
            foreach (var message2 in messages)
            {
                try
                {
                    if (message2.MessageId != messageGuid || message2.IsRoot)
                        continue;

                    dict[message2.ReceiverCode].Comment = null;
                    dict[message2.ReceiverCode].Created = null;
                    //dict[message2.ReceiverCode].Receiver = null;
                    dict[message2.ReceiverCode].Executor = null;
                    dict[message2.ReceiverCode].Documents = null;
                    dict[message2.ReceiverCode].Result = null;


                    dict[message2.ReceiverCode].Children ??= new List<ReceivingResultDto>();
                    dict[message2.ReceiverCode].Children!.Add(message2.Adapt<ReceivingResultDto>() with
                    {
                        Executor = message2.Executor.Adapt<EmployeeDto>(),
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        //}

        var res = dict.Select(x => x.Value)
            .OrderBy(x => x.Created)
            .Select(
                x =>
                    x.Children == null
                        ? x
                        : x with
                        {
                            ChildCount = x.Children.Count()
                        }
            )
            .ToList();

        return Ok(res);
    }

    private async Task<List<ReceivingResultDbo>> GetReceivingResults(Guid messageId)
    {
        if (messageId == Guid.Empty)
            return Enumerable.Empty<ReceivingResultDbo>().ToList();

        var messages = await _dbContext.ReceivingResults
            .Include(dbo => dbo.Receiver)
            .Include(dbo => dbo.Executor)
            .Where(p => p.MessageId == messageId)
            .OrderBy(p => p.Created)
            .AsNoTracking()
            .ToListAsync();
        return messages.Any() ? messages : Enumerable.Empty<ReceivingResultDbo>().ToList();
    }

    [HttpGet("byRequestGuid")]
    public async Task<ActionResult<OfficialMemoDto>> GetByRequestGuid(Guid requestGuid)
    {
        var request = await _processService.GetMessageDocument<OfficialMemoBase>(requestGuid);
        // var result = await officialMemoRepository.Get(request.Id);
        var result = await _dbContext.OfficialMemos
            .Include(dbo => dbo.ProcessData)
            .Include(dbo => dbo.ApprovalResults!)
                .ThenInclude(dbo => dbo.Approver)
                .IncludeApprovalExecutor()
            .IncludeApprovers()
            .IncludeRecipients()
            .Include(dbo => dbo.SignData)
                .ThenInclude(dbo => dbo!.Signer)
            .Include(dbo => dbo.Executor)
            .Include(dbo => dbo.Signer)
            .Include(dbo => dbo.ConfidenceType)
            .AsNoTracking()
            .FirstOrDefaultAsync(dbo => dbo.MessageGuid == request.Id);

        if (result is null) return NotFound();

        result.Executor ??= new EmployeeDbo { Login = result.ExecutorCode! };
        result.Signer ??= new EmployeeDbo { Login = result.SignerCode! };

        return result.Adapt<OfficialMemoDto>();
    }

    // Required for getofficialMemoByProcessGuid
    [HttpGet("byProcessGuid")]
    public async Task<ActionResult<OfficialMemoDto>> GetByProcessGuid(Guid processGuid)
    {
        var result = await _dbContext.OfficialMemos
            .Include(dbo => dbo.ProcessData)
            .Include(dbo => dbo.ApprovalResults!)
                .ThenInclude(dbo => dbo.Approver)
                .IncludeApprovalExecutor()
            .IncludeApprovers()
            .IncludeRecipients()
            .Include(dbo => dbo.SignData)
                .ThenInclude(dbo => dbo!.Signer)
            .Include(dbo => dbo.Executor)
            .Include(dbo => dbo.Signer)
            .Include(dbo => dbo.ConfidenceType)
            .AsNoTracking()
            .FirstOrDefaultAsync(dbo => dbo.ProcessData.ProcessGuid == processGuid);

        if (result is null) return NotFound();

        result.Executor ??= new EmployeeDbo { Login = result.ExecutorCode! };
        result.Signer ??= new EmployeeDbo { Login = result.SignerCode! };

        return result.Adapt<OfficialMemoDto>();
    }

    [HttpGet("byRegNum")]
    public async Task<ActionResult<OfficialMemoDto>> GetByRegNum(string regNum)
    {
        OfficialMemoDbo? result = null;
            result = (await _dbContext.OfficialMemos
                .Include(dbo => dbo.ProcessData)
                .Include(dbo => dbo.ApprovalResults!)
                .ThenInclude(dbo => dbo.Approver)
                .IncludeApprovers()
                .Include(dbo => dbo.SignData)
                .ThenInclude(dbo => dbo!.Signer)
                .Include(dbo => dbo.Executor)
                .Include(dbo => dbo.Signer)
                .Include(dbo => dbo.ConfidenceType)
                .AsNoTracking()
                .FirstOrDefaultAsync(dbo => dbo.RegNum == regNum));
       
        if (result is null) return NotFound();

        result.Executor ??= new EmployeeDbo { Login = result.ExecutorCode! };
        result.Signer ??= new EmployeeDbo { Login = result.SignerCode! };

        return result.Adapt<OfficialMemoDto>();
    }

    [HttpPost("approve")]
    public async Task<ActionResult> Approve(ApproveRequest approveRequest)
    {
        var decision = approveRequest.Decision?.Adapt<ApprovalResults>();
        if (decision is null) return BadRequest();
        var approvalResult = new ApprovalResultDbo(approveRequest.MessageId, decision.Value)
        {
            ApproverCode = approveRequest.EmployeeCode,
            ExecutorCode = User.Identity!.Name!,
            Created = DateTime.Now,
            Comment = approveRequest.Comment,
            Documents = approveRequest.Documents
        };

        _dbContext.ApprovalResults.Add(approvalResult);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("receive")]
    public async Task<ActionResult> Receive(ApproveRequest approveRequest)
    {
        if (approveRequest.Decision is null) return BadRequest();
        var receivingResult = new ReceivingResultDbo(approveRequest.MessageId, approveRequest.Decision)
        {
            ReceiverCode = approveRequest.EmployeeCode,
            ExecutorCode = approveRequest.ExecutorCode != null ? approveRequest.ExecutorCode! : User.Identity!.Name!,
            Created = DateTime.Now,
            Comment = approveRequest.Comment,
            Documents = approveRequest.Documents,
            IsRoot = approveRequest.IsRoot
        };

        _dbContext.ReceivingResults.Add(receivingResult);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("sign")]
    public async Task<ActionResult> Sign(SignMessageDto signMessageDto, [FromServices] PdfApiClient pdfApiClient, [FromServices] FileServerClient fileServerClient,
        [FromServices] FileServerApiClient fileServerApiClient, [FromServices] ISignRepository signRepository, [FromServices] SignStamperService signStamperService)
    {
        var userCode = User.Identity?.Name ?? "Unknown";

        var message = await _processService.GetMessageDocument<OfficialMemoBase>(signMessageDto.RequestGuid);

        var offMemo = await _dbContext.OfficialMemos.FirstOrDefaultAsync(dbo => dbo.MessageGuid == message.Id);
        if (offMemo == null) return NoContent();

        var regNumDate = GetRegNumDate(DateTime.Now);

        var text = offMemo.Language.Equals("Қазақша", StringComparison.OrdinalIgnoreCase)
                    ? $"{regNumDate:dd.MM.yyyy}ж. №{offMemo.RegNum}"
                    : $"№{offMemo.RegNum} от {regNumDate:dd.MM.yyyy}г.";

        var documentWithRegisterRegNum = await pdfApiClient.InsertTextToUrl(
            new FileServer.Abstractions.Document("", offMemo.DocumentUrl!), text,
            new TextInsertOptions
            {
                X = 87,
                Y = 59,
                FontName = "Times New Roman",
                PageNumber = 1,
                Bold = true,
            });
        if (documentWithRegisterRegNum is null) throw new Exception("InsertTextToUrl returned null value");

        documentWithRegisterRegNum.ContentStream.Seek(0, SeekOrigin.Begin);
        var concatenatedPdfLoaded = await fileServerClient.UploadFileAsync(documentWithRegisterRegNum);

        offMemo.DocumentUrl = concatenatedPdfLoaded.Url;
        offMemo.RegisterDate = regNumDate;
        offMemo.RegisterCode = userCode;

        _dbContext.Entry(offMemo).State = EntityState.Modified;
       

        var docList = new List<Document>();
        if (!string.IsNullOrEmpty(signMessageDto.SignDocument?.Name) && !string.IsNullOrEmpty(signMessageDto.Signature))
        {
            var supervisorSign = Convert.FromBase64String(signMessageDto.Signature);
            var stream = new MemoryStream(supervisorSign);
            IFormFile formFile = new FormFile(stream, 0, supervisorSign.Length, "Files",
                signMessageDto.SignDocument.Name);
            IEnumerable<IFormFile> files = new List<IFormFile> { formFile };
            var documents = await fileServerApiClient.UploadFilesAsync(files, contentType: "application/octet-stream");
            docList = documents.ToList();
            if (docList.Count == 0) return BadRequest("UploadFilesAsync не вернул файлы");
        }

        var signMessage = signMessageDto.Adapt<SignMessage>() with
        {
            SignedBy = userCode,
            SignedDate = DateTime.Now,
            MessageGuid = message.Id,
            SignDocument = docList.Count > 0 ? docList[0] : new Document()
        };

        await signRepository.Add(signMessage);

        await InsertSignDataPage(signMessage.MessageGuid, offMemo.DocumentUrl, userCode, signRepository,
           signStamperService,
           fileServerClient);


        var replyXml = new SignMessageXml
        {
            Reply = new ReplyXml
            {
                ReplyDecision = "accept",
                ReplyDecisionName = "Подписать"
            }
        };
        await _processService.SendReplyMessageAsync(replyXml, signMessage.RequestGuid, userCode);
        await _dbContext.SaveChangesAsync();

        return Ok(signMessage.MessageGuid);
    }

    [HttpPost("rework")]
    public async Task<ActionResult> ReviewRework(ReviewMessageDto reviewMessage)
    {
        var userCode = User.Identity!.Name!;

        var request = await _processService.GetMessageDocument<OfficialMemoRework>(reviewMessage.ReplyDto.RequestGuid);

        var receiveResults = await _dbContext.ReceivingResults.Where(dbo => dbo.MessageId == request.Id).ToListAsync();
        if(receiveResults.Any())
        {
            foreach (var result in receiveResults)
            {
                _dbContext.Entry(result).State = EntityState.Deleted;
            }
        }

        var replyXml = new ReviewMessageXml
        {
            Reply = reviewMessage.ReplyDto.Adapt<ReplyXml>() with
            {
                RepliedBy = userCode,
                ReplyDate = DateTime.Now
            }
        };

        await _processService.SendReplyMessageAsync(replyXml, reviewMessage.ReplyDto.RequestGuid, userCode);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }


    [HttpGet("lastReply")]
    public async Task<ActionResult<ReplyDto>> GetLastReply(Guid requestGuid,
        [FromServices] EmployeesDbService employeesDbService)
    {
        var request = await _processService.GetMessageDocument<OfficialMemoRework>(requestGuid);
        if (request.Reply is null) return NoContent();

        var result = request.Reply.Adapt<ReplyDto>();
        result.RepliedBy = await employeesDbService.GetEmployeeInfoByCodeAsync(request.Reply?.RepliedBy ?? "");

        return Ok(result);
    }

    [HttpPost("delete")]
    public async Task<ActionResult> Delete(Guid processGuid, [FromServices] TechSupportWSService techSupportWsService)
    {
        await techSupportWsService.GoToBlockProcess(processGuid, "OfficialMemo", "709", "Calc");
        var officialMemoProcessDataDbo =
            await _dbContext.OfficialMemoProcessData.FirstOrDefaultAsync(dbo => dbo.ProcessGuid == processGuid);

        _logger.LogOffMemo(officialMemoProcessDataDbo!.MessageGuid, "SendToArchive",
            new { officialMemoProcessDataDbo.RegNum, ProcessGuid = processGuid });

        return Ok();
    }

    [HttpPost("registerRegNum")]
    public async Task<ActionResult> RegisterRegNum([FromBody] Guid requestGuid,
        [FromServices] ProcessesDbService processesDbService, [FromServices] ISignRepository signRepository,
        [FromServices] SignStamperService signStamperService,
        [FromServices] FileServerClient fileServerApiClient,
        [FromServices] PdfApiClient pdfApiClient
        )
    {
        var request = await _processService.GetMessageDocument<OfficialMemoBase>(requestGuid);

        var officialMemo = await _dbContext.OfficialMemos.Where(d => d.MessageGuid == request.Id)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(officialMemo?.DocumentUrl) || string.IsNullOrEmpty(officialMemo.OriginalDocumentUrl))
            return BadRequest("Document url not found");
        // Временно допустить перерегистрацию для неправильно назначенных рег-номеров
        if (!officialMemo.RegisterDate.HasValue
            || (!string.IsNullOrEmpty(officialMemo.RegNum) && !officialMemo.RegNum!.Contains(officialMemo.RegisterCode ?? "")))
        {
            var (signerCode, initiatorCode) = await _dbContext.OfficialMemos
                .Include(dbo => dbo.ProcessData)
                .Where(d => d.MessageGuid == request.Id)
                .Select(dbo => new ValueTuple<string, string>(dbo.Signer!.Login, dbo.ProcessData.InitiatorCode))
                .FirstOrDefaultAsync();

            var (signerDepCode, signerPDepCode, signerBranchCode, signerIsStaff) = await _dbContext.Employees
                .Where(dbo => dbo.Login == signerCode)
                .Select(dbo => new ValueTuple<string, string, string, bool>(dbo.DepartmentCode ?? "",
                    dbo.ParentDepartmentCode ?? "", dbo.BranchCode ?? "", dbo.IsStaff))
                .FirstOrDefaultAsync();

            if (signerIsStaff && string.IsNullOrEmpty(signerPDepCode)) return NotFound();

            string depCode;
            if (!signerIsStaff) depCode = "00";
            else if (signerDepCode == "1827")
            {
                var (initiatorDepCode, initiatorPDepCode, initiatorBranchCode, initiatorIsStaff) = await _dbContext.Employees
                    .Where(dbo => dbo.Login == initiatorCode)
                    .Select(dbo => new ValueTuple<string, string, string, bool>(dbo.DepartmentCode ?? "",
                        dbo.ParentDepartmentCode ?? "", dbo.BranchCode ?? "", dbo.IsStaff))
                    .FirstOrDefaultAsync();

                if (!initiatorIsStaff) depCode = "00";
                else if (initiatorPDepCode == "18") depCode = initiatorDepCode;
                else if (initiatorPDepCode.StartsWith("18")) depCode = initiatorPDepCode;
                else if (initiatorDepCode == "1827") throw new Exception("Ты зампред");
                else depCode = initiatorBranchCode;
            }
            else if (signerPDepCode == "18") depCode = signerDepCode;
            else if (signerPDepCode.StartsWith("18")) depCode = signerPDepCode;
            else depCode = signerBranchCode;

            var regCode = await _dbContext.DepCodeToRegCodes.Where(d => d.DepartmentCode == depCode)
                .Select(c => c.Code).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(regCode)) return NotFound("RegCode not found");

            //var index = await processesDbService.NextIndex("outgoingRegisterCodes", "DocExchangeOutgoing");

            //var registerCode = $"{officialMemo.RegisterCode}-{regCode}/{index}";
            //officialMemo.RegNum = registerCode;
            officialMemo.RegisterDate = DateTime.Now;
            officialMemo.VerticalText =
                $"{officialMemo.RegNum} от {officialMemo.RegisterDate:dd.MM.yyyy}. " +
                $"Дата: {officialMemo.RegisterDate:dd.MM.yyyy}. Версия СЭД: ВРМ 2.0. Положительный результат проверки ЭЦП";

            // _dbContext.Entry(officialMemo).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        //await InsertRegisterRegNum(officialMemo.MessageGuid, officialMemo.OriginalDocumentUrl, pdfApiClient,
        //    fileServerApiClient);
        //await InsertSignDataPage(officialMemo.MessageGuid, officialMemo.DocumentUrl, User.Identity!.Name!, signRepository,
        //    signStamperService,
        //    fileServerApiClient);

        _logger.LogOffMemo(officialMemo.MessageGuid, "registerRegNum",
            new { RegisterCode = officialMemo.RegisterCode, User = User.Identity.Name });
        await _dbContext.SaveChangesAsync();

        return Ok(new { officialMemo.RegNum, officialMemo.RegisterDate });
        // return Ok(new { officialMemo.OutRegNum, officialMemo.RegisterDate });
    }

    private async Task InsertRegisterRegNum(Guid messageId, string documentUrl,
        PdfApiClient pdfApiClient, FileServerClient fileServerApiClient)
    {
        var officialMemo = await _dbContext.OfficialMemos.FirstOrDefaultAsync(d => d.MessageGuid == messageId);
        if (officialMemo is null) throw new Exception("Out message not found");

        var text = officialMemo.Language.Equals("Қазақша", StringComparison.OrdinalIgnoreCase)
            ? $"{officialMemo.RegisterDate:dd.MM.yyyy}ж. №{officialMemo.RegNum}"
            : $"№{officialMemo.RegNum} от {officialMemo.RegisterDate:dd.MM.yyyy}г.";

        var documentWithRegisterRegNum = await pdfApiClient.InsertTextToUrl(
            new FileServer.Abstractions.Document("", documentUrl), text,
            new TextInsertOptions
            {
                X = 235,
                Y = 60,
                FontName = "Times New Roman",
                PageNumber = 1,
                Bold= true,
            });
        if (documentWithRegisterRegNum is null) throw new Exception("InsertTextToUrl returned null value");

        documentWithRegisterRegNum.ContentStream.Seek(0, SeekOrigin.Begin);
        var concatenatedPdfLoaded = await fileServerApiClient.UploadFileAsync(documentWithRegisterRegNum);

        officialMemo.DocumentUrl = concatenatedPdfLoaded.Url;
        await _dbContext.SaveChangesAsync();
    }

    private async Task InsertSignDataPage(Guid messageId, string documentUrl, string userCode,
        ISignRepository signRepository, SignStamperService signStamperService,
        FileServerClient fileServerApiClient)
    {
        var officialMemo = await _dbContext.OfficialMemos.IncludeRecipients().Include(dbo => dbo.Executor).FirstOrDefaultAsync(d => d.MessageGuid == messageId);
        if (officialMemo is null) throw new Exception("Out message not found");

        var signMessage = await signRepository.Get(messageId);
        if (signMessage is null) throw new Exception("Sign message not found");

        if (signMessage.SignType == SignType.HandWritten) return;

        var signedBy = await _dbContext.Employees.Where(dbo => dbo.Login == signMessage.SignedBy).FirstOrDefaultAsync();
        if (signedBy is null) throw new Exception($"User {signMessage.SignedBy} not found");
        var registeredBy = await _dbContext.Employees.Where(dbo => dbo.Login == userCode).FirstOrDefaultAsync();
        if (registeredBy is null) throw new Exception($"User {userCode} not found");


        var signData = new SignData
        {
            RegisterDate = DateTime.Now,// officialMemo.RegisterDate!.Value,
            RegNum = officialMemo.RegNum!,
            Receivers = officialMemo.Recipients.Select(r => new Signer.Models.Client
            { Name = new LocalizableString(r.Name, r.Name)}).ToArray(),
            Sender = new Signer.Models.Client
            {
                Name = new LocalizableString(officialMemo.Executor.Name, officialMemo.Executor.Name)
            },
            DocumentType = new LocalizableString("Қызметтік жазба", "Служебная записка")
        };

        if (!string.IsNullOrEmpty(signMessage.Signature))
        {
            signData.SignerSignature = new Signature
            {
                Base64Text = signMessage.Signature,
                SignedBy = new Signer.Models.Employee
                {
                    Login = "",
                    Name = signMessage.SignDocument!.Name.Substring(0, signMessage.SignDocument.Name.Length - 5),
                    Position = new LocalizableString("", "")
                },
                SignType = signMessage.SignType!.Adapt<Signer.Models.SignType>(),
                SignedDate = signMessage.SignedDate,
            };
        }

        if (!string.IsNullOrEmpty(signMessage.RegisterSignature))
        {
            signData.RegistrarSignature = new Signature
            {
                Base64Text = signMessage.RegisterSignature,
                SignedBy = new Signer.Models.Employee
                {
                    Login = registeredBy.Login,
                    Name = registeredBy.Name,
                    Position = new LocalizableString(registeredBy.PositionKz!, registeredBy.PositionRu!)
                },
                SignType = Signer.Models.SignType.Digital,
                SignedDate = signMessage.RegisterSignedDate!.Value,
            };
        }

        var documentWithSignData = await signStamperService.AddSignDataPageToPdf(
            new FileServer.Abstractions.Document { Name = "document.pdf", Url = documentUrl },
            signData);

        if (documentWithSignData is null) throw new Exception("AddSignDataPageToPdf returned null value");

        documentWithSignData.ContentStream.Seek(0, SeekOrigin.Begin);
        var concatenatedPdfLoaded = await fileServerApiClient.UploadFileAsync(documentWithSignData);

        // _dbContext.Entry(officialMemo).State = EntityState.Modified;
        officialMemo.DocumentUrl = concatenatedPdfLoaded.Url;
        await _dbContext.SaveChangesAsync();
    }

    [HttpPost("close")]
    public async Task<ActionResult> CloseOOfficialMemo(ReviewMessageDto reviewMessage,
    [FromServices] TasksService taskCloserService)
    {
        var message = await _processService.GetMessageDocument<ReviewMessageXml>(reviewMessage.ReplyDto.RequestGuid);
        var officialMemoDbo = await _dbContext.OfficialMemos
            .Include(dbo => dbo.Executor)
            .Where(dbo => dbo.MessageGuid == message.Id).FirstOrDefaultAsync();

        if (officialMemoDbo is null) throw new Exception("Official memo not found");

        await _processService.SendReplyMessageAsync(
            message with
            {
                Reply = reviewMessage.ReplyDto.Adapt<ReplyXml>(),
            }, reviewMessage.ReplyDto.RequestGuid, User.Identity!.Name!);
        return Ok();
    }

    [HttpPost("registerSign")]
    public async Task<ActionResult> RegisterSign(SignMessageDto signMessageDto,
    [FromServices] ISignRepository signRepository)
    {
        var userCode = User.Identity?.Name ?? "Unknown";

        var message = await _processService.GetMessageDocument<OfficialMemoBase>(signMessageDto.RequestGuid);

        var signMessage = signMessageDto.Adapt<SignMessage>() with
        {
            RegisterSignedBy = userCode,
            RegisterSignedDate = DateTime.Now,
            MessageGuid = message.Id
        };
        await signRepository.Update(signMessage);
        return Ok(signMessage.MessageGuid);
    }

    [HttpGet("signature")]
    public async Task<SignMessageDto> GetSignature(Guid requestGuid, [FromServices] ISignRepository signRepository)
    {
        var message = await _processService.GetMessageDocument<OfficialMemoBase>(requestGuid);

        var signMessage = await signRepository.Get(message.Id);
        var signMessageDto = _mapper.Map<SignMessageDto>(signMessage);

        return signMessageDto;
    }

    [HttpPost("accept")]
    public async Task<ActionResult> AcceptOfficialMemo(ReviewMessageDto reviewMessage)
    {
        var message = await _processService.GetMessageDocument<ReviewMessageXml>(reviewMessage.ReplyDto.RequestGuid);

        await _processService.SendReplyMessageAsync(
            message with
            {
                Reply = reviewMessage.ReplyDto.Adapt<ReplyXml>(),
            }, reviewMessage.ReplyDto.RequestGuid, User.Identity!.Name!);
        return Ok();
    }

    [HttpPost("setBookmark")]
    public async Task<ActionResult> SetBookmark(string regNum, bool isBookmark)
    {
        var message = await _dbContext.OfficialMemos.Where(dbo => dbo.RegNum == regNum).FirstOrDefaultAsync();
        if(message is not null)
        {
            message.IsBookmark = isBookmark;
            _dbContext.Entry(message).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        return Ok();
    }
}
