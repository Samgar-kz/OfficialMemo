using System.Data;
using Dapper;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Context;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Services;

public class EmployeesDbService
{
    private readonly IDbConnection _connection;
    private readonly DataContext _dbContext;

    public EmployeesDbService(IDbConnection connection, DataContext dbContext)
    {
        _connection = connection;
        _dbContext = dbContext;
    }

    //public async Task<Employee?> GetEmployeeInfoByCodeAsync(string? code)
    //{
    //    if (string.IsNullOrEmpty(code)) return null;
    //    var employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(dbo => dbo.Login == code);
    //    return employee?.Adapt<Employee>();
    //}

    public async Task<Employee?> GetEmployeeInfoByCodeAsync(string? code)
    {
        if (string.IsNullOrEmpty(code)) return null;
        const string @q = @"SELECT Id, Code, Login, Iin, Name, Email, Phones, LocalPhone, WorkStatus, PositionKz, 
                                PositionRu, PositionEn, DepartmentCode
                            FROM [Request].[OffMemo].[v_Employees] (nolock)
                            where Login=@code";
        return await _connection.QueryFirstAsync<Employee>(q, new { code });
    }

    public async Task<EmployeeDbo?> GetEmployeeDboByCodeAsync(string code)
    {
        const string @q = @"SELECT Id, Code, Login, Iin, Name, Email, Phones, LocalPhone, WorkStatus,
                                DepartmentCode, ParentDepartmentCode, BranchCode, Deleted, IsStaff,
                                PositionKz, PositionRu, PositionEn, PositionCode
                            FROM [Request].[OffMemo].[v_Employees] (nolock)
                            where Login=@code";
        return await _connection.QueryFirstAsync<EmployeeDbo>(q, new { code });
    }

    public async Task<string?> GetEmployeeBranchCodeByLoginAsync(string login)
    {
        const string @q = @"SELECT top 1 BranchCode
                            FROM [Request].[OffMemo].[v_Employees] (nolock)
                            where Login=@login";
        return await _connection.ExecuteScalarAsync<string>(q, new { login });
    }

    public async Task<IndexNomenclatureDbo> GetIndexNomenclaturesAsync()
    {
        const string @q = @"SELECT [Department]
                                  ,[Index]
                                  ,[Name]
                              FROM [Request].[OffMemo].[IndexNomenclatures] (nolock)";
        return await _connection.QueryFirstAsync<IndexNomenclatureDbo>(q);
    }
}