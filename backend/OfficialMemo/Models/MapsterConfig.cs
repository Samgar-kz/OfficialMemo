using Mapster;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Dto;
using OfficialMemo.Models.Poco;
using OfficialMemo.Models.Xml;

namespace OfficialMemo.Models;

public static class MapsterConfig
{
    public static void Configure()
    {
        ConfigureDateMappings();
        ConfigureListLongMappings();
        ConfigureListStringMappings<string>();
        TypeAdapterConfig<Employee, EmployeeXml>
            .NewConfig()
            .Map(dest => dest.Code, src => src.Login);

        TypeAdapterConfig<EmployeeDbo, EmployeeXml>
            .NewConfig()
            .Map(dest => dest.Code, src => src.Login);

        TypeAdapterConfig<EmployeeDbo, ApproverXml>
            .NewConfig()
            .Inherits<EmployeeDbo, EmployeeXml>();

        TypeAdapterConfig<EmployeeDbo, RecipientXml>
            .NewConfig()
            .Inherits<EmployeeDbo, EmployeeXml>();

        TypeAdapterConfig<MessageDbo, MessageDto>
            .NewConfig()
            .TwoWays()
            .Map(dest => dest.Step, src => src.SchemaName);

        TypeAdapterConfig<AssignmentMessageDbo, MessageDto>
            .NewConfig()
            .TwoWays()
            .Map(dest => dest.Step, src => src.SchemaName);

        TypeAdapterConfig<ProcessReportDbo, ProcessReportDto>
            .NewConfig()
            .TwoWays();

        TypeAdapterConfig<EmployeeShortDto, string>.NewConfig().TwoWays().Map(s => s, dto => dto.Login);

        TypeAdapterConfig<RedirectDto, RedirectXml>.NewConfig().Map(d => d.RedirectTo, s => s.RedirectTo.Login);

        TypeAdapterConfig<ReplyDto, ReplyXml>.NewConfig()
            .Map(d => d.RepliedBy, s => s.RepliedBy!.Login);
        TypeAdapterConfig<ReplyXml, ReplyDto>.NewConfig()
            .Map(d => d.RepliedBy, s => new EmployeeDto { Code = s.RepliedBy });

        TypeAdapterConfig<EmployeeShortDto, EmployeeXml>.NewConfig().TwoWays().Map(d => d.Code, s => s.Login);


        TypeAdapterConfig<PositionRenameRequest, EmployeePositionsDbo>.NewConfig()
            .Map(dest => dest.UserCode, src => src.Login)
            .Map(dest => dest.Kz, src => src.PositionKz)
            .Map(dest => dest.Ru, src => src.PositionRu)
            .Map(dest => dest.En, src => src.PositionEn);
    }


    static string GetPerformersAsString(List<long> performers)
    {
        string res = "";
        for (int i = 0; i < performers.Count; i++)
        {
            res += performers[i].ToString();
            if (i + 1 < performers.Count) res += ";";
        }

        return res;
    }

    static List<long> GetPerformersAsList(string performers)
    {
        List<long> res = new List<long>();
        string[] performArray = performers.Split(';');
        foreach (string performer in performArray)
        {
            res.Add(long.Parse(performer));
        }

        return res;
    }

    static string ListToString<T>(List<T> performers)
    {
        string res = "";
        for (int i = 0; i < performers.Count; i++)
        {
            res += performers[i]?.ToString();
            if (i + 1 < performers.Count && performers[i] != null) res += ";";
        }

        return res;
    }

    static List<T> StringToList<T>(string performers)
    {
        List<T> res = new List<T>();
        string[] performArray = performers.Split(';');
        foreach (string performer in performArray)
        {
            T convertedPerformer = (T)Convert.ChangeType(performer, typeof(T));
            res.Add(convertedPerformer);
        }

        return res;
    }

    static DateTime? Con(DateTimeOffset? d)
    {
        return d.HasValue ? new DateTime(d.Value.UtcTicks, DateTimeKind.Utc) : null;
    }

    static DateTimeOffset? Con2(DateTime? d)
    {
        return d.HasValue ? new DateTimeOffset(d.Value.ToUniversalTime(), TimeSpan.Zero) : null;
    }

    private static void ConfigureDateMappings()
    {
        TypeAdapterConfig<DateTime, DateTimeOffset>
            .NewConfig()
            .MapWith(time => new DateTimeOffset(time.ToUniversalTime(), TimeSpan.Zero));
        TypeAdapterConfig<DateTimeOffset, DateTime>
            .NewConfig()
            .MapWith(time => new DateTime(time.UtcTicks, DateTimeKind.Utc));

        TypeAdapterConfig<DateTimeOffset?, DateTime?>
            .NewConfig()
            .MapWith(time => Con(time));
        TypeAdapterConfig<DateTime?, DateTimeOffset?>
            .NewConfig()
            .MapWith(time => Con2(time));
    }

    private static void ConfigureListLongMappings()
    {
        TypeAdapterConfig<List<long>, string>
            .NewConfig()
            .MapWith(performers => GetPerformersAsString(performers));
        TypeAdapterConfig<string, List<long>>
            .NewConfig()
            .MapWith(performers => GetPerformersAsList(performers));
    }

    private static void ConfigureListStringMappings<T>()
    {
        TypeAdapterConfig<List<T>, string>
            .NewConfig()
            .MapWith(list => ListToString<T>(list));
        TypeAdapterConfig<string, List<T>>
            .NewConfig()
            .MapWith(listAsString => StringToList<T>(listAsString));
    }
}