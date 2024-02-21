using Mapster;

namespace OfficialMemo.Logging;

public class BusinessProcessLogger: ILogger
{
    private readonly string _name;
    private readonly SqlLoggerProvider _provider;

    public BusinessProcessLogger(string name, SqlLoggerProvider provider)
    {
        _name = name;
        _provider = provider;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null!;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (state is not BusinessProcessLogEntry logEntry) return;
        
        logEntry.LogLevel = logLevel.ToString();
        logEntry.EventName = logEntry.EventName;
        logEntry.Message = formatter(state, exception);
        logEntry.UserCode = _provider.GetCurrentUserId();
        logEntry.Timestamp = DateTime.Now;
        _provider.AddLogEntry(logEntry);
    }
}