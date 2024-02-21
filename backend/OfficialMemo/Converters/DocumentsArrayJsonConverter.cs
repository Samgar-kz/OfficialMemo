using System.Data;
using System.Text.Json;
using Dapper;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Converters;

public class DocumentsArrayJsonConverter : SqlMapper.TypeHandler<Document[]?>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public override Document[]? Parse(object value)
    {
        return JsonSerializer.Deserialize<Document[]>(value.ToString() ?? "", _jsonSerializerOptions);
    }

    public override void SetValue(IDbDataParameter parameter, Document[]? value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}