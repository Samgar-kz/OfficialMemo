using Dapper;
using OfficialMemo.Models.Poco;
using System.Data;
using System.Text.Json;

namespace OfficialMemo.Converters;

public class RecipientsArrayJsonConverter : SqlMapper.TypeHandler<List<Person>>
{

    //private readonly JsonSerializerOptions _jsonSerializerOptions;

    //public RecipientsArrayJsonConverter(JsonSerializerOptions jsonSerializerOptions)
    //{
    //    _jsonSerializerOptions = jsonSerializerOptions;
    //}

    public override List<Person> Parse(object value)
    {
        return JsonSerializer.Deserialize<List<Person>>(value.ToString() ?? "");
    }

    public override void SetValue(IDbDataParameter parameter, List<Person> value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

}