using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OfficialMemo.Converters;

public class EfJsonConverter<T>: ValueConverter<T?, string>
{
    public static readonly JsonSerializerOptions? JsonSerializerOptions;

    static EfJsonConverter()
    {
        JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = false,
            AllowTrailingCommas = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
    
    public EfJsonConverter() : base(
        v => JsonSerializer.Serialize(v, JsonSerializerOptions),
        v => JsonSerializer.Deserialize<T>(v, JsonSerializerOptions) ?? default)
    { }
}