using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Context;

namespace OfficialMemo.Logging;

public class SqlLoggerProvider: ILoggerProvider
{
    private readonly ConcurrentQueue<BusinessProcessLogEntry> _logQueue = new();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const int IntervalMs = 5000;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IServiceScopeFactory _scopeFactory;
    
    public SqlLoggerProvider(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _scopeFactory = scopeFactory;
        WriteLoop(TimeSpan.FromMilliseconds(IntervalMs), _cancellationTokenSource.Token);
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        return new BusinessProcessLogger(categoryName, this);
    }

    public void AddLogEntry(BusinessProcessLogEntry entry)
    {
        _logQueue.Enqueue(entry);
    }

    private async Task WriteLogs()
    {
        using var scope = _scopeFactory.CreateScope();
        try
        {
            await using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            while (_logQueue.TryDequeue(out var entry))
            {
                dbContext.BusinessProcessLogEntries.Add(entry);
                await dbContext.SaveChangesAsync();
            }
        }
        catch
        {
            return;
        }
        
    }

    private async Task WriteLoop(TimeSpan interval, CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(interval);
        while (true)
        {
            await WriteLogs();
            await timer.WaitForNextTickAsync(cancellationToken);
        }
    }
    
    public string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "";
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}
