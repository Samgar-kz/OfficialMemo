using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OfficialMemo.Models.Dbo;

namespace OfficialMemo.Extensions;

public static class DbContextExtensions
{
    public static async Task<IEnumerable<EmployeeDbo>> GetFullInfoAsync(this DbSet<EmployeeDbo> employees, IEnumerable<EmployeeDbo> originalEmployees)
    {
        var originalEmployeeLogins = originalEmployees.Select(t => t.Login).ToArray();
        var employeesFullInfo = await employees
            .Where(dbo => originalEmployeeLogins.Contains(dbo.Login))
            .AsNoTracking()
            .ToDictionaryAsync(dbo => dbo.Login, StringComparer.OrdinalIgnoreCase);
        
        var result = new List<EmployeeDbo>(originalEmployeeLogins.Length);
        foreach (var login in originalEmployeeLogins)
        {
            if(employeesFullInfo.ContainsKey(login))
                result.Add(employeesFullInfo[login]);
        }

        return result;
    }

    public static IIncludableQueryable<OfficialMemoDbo, EmployeeDbo> IncludeApprovers(this IQueryable<OfficialMemoDbo> source)
    {
        return source
            .Include(dbo => dbo.OfficialMemoApprovers!)
            .ThenInclude(dbo => dbo.Approver);
    }    
    public static IIncludableQueryable<OfficialMemoDbo, EmployeeDbo> IncludeRecipients(this IQueryable<OfficialMemoDbo> source)
    {
        return source
            .Include(dbo => dbo.OfficialMemoRecipients)
            .ThenInclude(dbo => dbo.Recipient);
    }    
    
    public static IIncludableQueryable<OfficialMemoDbo, EmployeeDbo> IncludeApprovalExecutor(this IQueryable<OfficialMemoDbo> source)
    {
        return source
            .Include(dbo => dbo.ApprovalResults!)
            .ThenInclude(dbo => dbo.Executor!);
    }    
    
    //public static IIncludableQueryable<OfficialMemoDbo, EmployeeDbo> IncludeReceiverExecutor(this IQueryable<OfficialMemoDbo> source)
    //{
    //    return source
    //        .Include(dbo => dbo.ReceivingResults!)
    //        .ThenInclude(dbo => dbo.Executor!);
    //}
}