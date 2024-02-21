using AutoMapper;
using OfficialMemo.Models.Dbo;
using Microsoft.AspNetCore.Mvc;
using OfficialMemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Context;
using OfficialMemo.Models.Dto;
using FuzzySharp;
using Mapster;

namespace OfficialMemo.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly EmployeesDbService _employeesDbService;
    private readonly DataContext _dbContext;
    public EmployeesController(IMapper mapper, EmployeesDbService employeesDbService, DataContext dbContext)
    {
        _mapper = mapper;
        _employeesDbService = employeesDbService;
        _dbContext = dbContext;
    }


    [HttpGet("whoAmI")]
    public async Task<ActionResult<EmployeeDto?>> WhoAmI()
    {
        var me = await _employeesDbService.GetEmployeeInfoByCodeAsync(User.Identity?.Name);
        if (me is null) return Empty;
        return _mapper.Map<EmployeeDto>(me);
    }

    [HttpPost("position/rename")]
    public async Task<ActionResult> RenamePosition(PositionRenameRequest request)
    {
        var employeePosition = await _dbContext.EmployeePositions.FirstOrDefaultAsync(dbo => dbo.UserCode == request.Login);
        if (employeePosition is null)
        {
            _dbContext.EmployeePositions.Add(request.Adapt<EmployeePositionsDbo>());
        }
        else
        {
            employeePosition.Kz = request.PositionKz;
            employeePosition.Ru = request.PositionRu;
            employeePosition.En = request.PositionEn;
            employeePosition.LocalPhone = request.LocalPhone;
        }

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> SearchEmployees(string searchString, bool isOnlyEmployee = true, int count = 20, CancellationToken cancellationToken = new())
    {
        searchString = searchString.Trim().ToUpperInvariant();

        var employees2 = (await _dbContext.Employees.Where(dbo => !dbo.Deleted && (isOnlyEmployee ? !string.IsNullOrEmpty(dbo.Code) : true))
            .Select(dbo => new { dbo.Login, dbo.Name })
            .ToListAsync(cancellationToken)).DistinctBy(dbo => dbo.Login).ToList();

        var fuzzyLogins = Process.ExtractSorted(searchString,
                employees2.Select(c => c.Name), s => s.ToUpperInvariant(), cutoff: 60)
            .Select(fr => employees2[fr.Index].Login.ToUpperInvariant())
            .ToList();
        var resultDict = (await _dbContext.Employees
                .Where(dbo => fuzzyLogins.Contains(dbo.Login))
                .ToListAsync(cancellationToken))
            .DistinctBy(dbo => dbo.Login.ToUpperInvariant())
            .ToDictionary(entity => entity.Login.ToUpperInvariant());

        var result = fuzzyLogins.Select(e => resultDict[e.ToUpperInvariant()].Adapt<EmployeeDto>());
        return Ok(result);
    }
}
