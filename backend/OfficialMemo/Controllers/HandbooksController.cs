using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Context;
using OfficialMemo.Models;
using Mapster;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Services;

namespace OfficialMemo.Controllers;

[ApiController]
[Route("[controller]")]
public class HandbooksController: ControllerBase
{
    private readonly DataContext _dbContext;
    private readonly HandbooksService _handbooksService;

    public HandbooksController(DataContext dbContext, HandbooksService handbooksService)
    {
        _dbContext = dbContext;
        _handbooksService = handbooksService;
    }

    [HttpGet("confidenceTypes")]
    public async Task<ActionResult<IEnumerable<ConfidenceType>>> GetConfidenceTypes()
    {
        var confidenceTypes = await _handbooksService.GetConfidenceTypesAsync();
        if (confidenceTypes.Count() == 0) return NoContent();
        return Ok(confidenceTypes);
    }    
    
    [HttpGet("indexNomenclatures")]
    public async Task<ActionResult<IEnumerable<IndexNomenclatureDbo>>> GetIndexNomenclatures()
    {
        var indexNomenclatures = await _handbooksService.GetIndexNomenclaturesAsync();
        if (indexNomenclatures.Count() == 0) return NoContent();
        return Ok(indexNomenclatures);
    }
}