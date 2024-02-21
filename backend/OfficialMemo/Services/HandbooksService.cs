using System.Data;
using Dapper;
using OfficialMemo.Models;
using OfficialMemo.Models.Dbo;

namespace OfficialMemo.Services;

public class HandbooksService
{
    private readonly IDbConnection _connection;
    public HandbooksService(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<ConfidenceType>> GetConfidenceTypesAsync()
    {
        const string @q = @"SELECT [Id]
                              ,[DisplayTextKz]
                              ,[DisplayTextRu]
                          FROM [Request].[DocEx].[ConfidenceTypes] with (nolock)";
        return await _connection.QueryAsync<ConfidenceType>(q);
    }

    public async Task<IEnumerable<IndexNomenclatureDbo>> GetIndexNomenclaturesAsync()
    {
        const string @q = @"SELECT [Department]
                                  ,[Index]
                                  ,[Name]
                              FROM [Request].[OffMemo].[IndexNomenclatures] with (nolock)";
        return await _connection.QueryAsync<IndexNomenclatureDbo>(q);
    }
}
