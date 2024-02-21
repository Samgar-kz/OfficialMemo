namespace OfficialMemo.Logging;

public class SqlLoggerOptions
{
    public LogLevel Level { get; set; }
    public TimeSpan FlushPeriod { get; set; }
}