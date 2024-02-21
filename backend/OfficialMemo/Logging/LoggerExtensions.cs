using System.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace OfficialMemo.Logging;

public static class LoggerExtensions
{
    public static class ProcessNames
    {
        public const string OfficialMemos = "OfficialMemo";
    }
    
    public static void LogOffMemo(this ILogger logger, Guid documentId, string eventName, object? additionalData = default)
    {
        LogEvent(logger, LogLevel.Information, documentId, ProcessNames.OfficialMemos, eventName, additionalData);
    }

    private static void LogEvent(ILogger logger, LogLevel logLevel, Guid documentId, string processName, string eventName, object? additionalData)
    {
        var logEntry = new BusinessProcessLogEntry 
        {
            DocumentId = documentId,
            ProcessName = processName,
            EventName = eventName,
        };
        if (additionalData is not null)
            logEntry.AdditionalData = JsonSerializer.Serialize(additionalData,
                new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
        logger.Log(logLevel, new EventId(1, eventName), logEntry, null, (_, _) => "");
    }

    public static void AddBusinessProcessLogger(this ILoggingBuilder builder, Action<SqlLoggerOptions>? configure)
    {
        var options = new SqlLoggerOptions();
        configure?.Invoke(options);
        
        if (builder.Services.All(x => x.ServiceType != typeof(IHttpContextAccessor)))
        {
            builder.Services.AddHttpContextAccessor();
        }

        builder.AddProvider(builder.Services.BuildServiceProvider().GetRequiredService<SqlLoggerProvider>());
        // builder.AddProvider(new SqlLoggerProvider(options, 
        //     builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>(), 
        //     builder.Services.BuildServiceProvider().GetRequiredService<IDbContextFactory<DocExchangeDbContext>>()));
    }
}
