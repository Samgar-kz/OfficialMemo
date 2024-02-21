using System.Data;
using Dapper;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Services;

public class ProcessesDbService
{

    private readonly IDbConnection _connection;

    public ProcessesDbService(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> IsExistsRequest(Guid requestGuid)
    {
        bool result;
        result = await _connection.ExecuteScalarAsync<bool>(
            @"
        select top 1 cast(1 as bit ) from [AbprocSystem].[dbo].[Message_request] with (nolock) where request_guid =@requestGuid
        ",
            new { requestGuid }
        );
        return result;
    }

    public async Task<ProcessInfo?> GetProcessInfo(Guid requestGuid)
    {
        const string q = @"
             select pm.PROCESS_GUID as ProcessGuid,
                    mr.REQUEST_GUID as RequestGuid,
                    pm.SCHEME_ID as SchemeId,
                    ompd.PROCESS_STATUS as ProcessStatus,
                    ompd.MessageGuid
             from ABProcsystem.dbo.PROCESS_MSG pm with (nolock)
             inner join ABProcsystem.dbo.MESSAGE_REQUEST mr with (nolock) on mr.MESSAGE_GUID = pm.MESSAGE_GUID
             inner join Request.OffMemo.OfficialMemoProcessData ompd with (nolock) on ompd.PROCESS_GUID = pm.PROCESS_GUID
            where mr.REQUEST_GUID = @requestGuid 
        ";
        return await _connection.QueryFirstOrDefaultAsync<ProcessInfo>(q, new { requestGuid });
    }    
    
    public async Task<ProcessInfo?> GetProcessInfoByProcessGuid(Guid processGuid)
    {
        const string q = @"
            select pm.PROCESS_GUID as ProcessGuid,
                   mr.REQUEST_GUID as RequestGuid,
                   pm.SCHEME_ID as SchemeId,
                   ompd.PROCESS_STATUS as ProcessStatus,
                   ompd.MessageGuid
            from ABProcsystem.dbo.PROCESS_MSG pm with (nolock)
            inner join ABProcsystem.dbo.MESSAGE_REQUEST mr with (nolock) on mr.MESSAGE_GUID = pm.MESSAGE_GUID
            inner join Request.OffMemo.OfficialMemoProcessData ompd with (nolock) on ompd.PROCESS_GUID = pm.PROCESS_GUID
            where pm.PROCESS_GUID = @processGuid 
        ";
        return await _connection.QueryFirstOrDefaultAsync<ProcessInfo>(q, new { processGuid });
    }

    public async Task<string> NextRegNum(string counterCode, string processCode)
    {
        var index = await NextIndex(counterCode, processCode);
        var regNum = $"{index}-{DateTime.Now.Year}";
        return regNum;
    }
        
    public async Task<int> NextIndex(string counterCode, string processCode)
    {
        var @params = new DynamicParameters(new
        {
            @in_counter_code = counterCode,
            @in_process_code = processCode
        });
        @params.Add("@out_counter_value", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _connection.ExecuteAsync("[Request].[dbo].[GetCounterByProcessNew]",
            @params, commandType: CommandType.StoredProcedure);
            
        return @params.Get<int>("@out_counter_value");
    }

}