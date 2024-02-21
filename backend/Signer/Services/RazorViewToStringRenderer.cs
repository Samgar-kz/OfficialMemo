using System.Collections.Concurrent;
using System.Text;
using RazorEngineCore;

namespace Signer.Services;

public class RazorViewToStringRenderer
{
    internal static readonly ConcurrentDictionary<int, IRazorEngineCompiledTemplate> TemplateCache = new();

    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
    {
        await using var sw = new StringWriter();

        if (!TemplateCache.TryGetValue(viewName.GetHashCode(), out var template))
        {
            throw new ArgumentNullException($"{viewName} does not match any available view");
        }

        var result = await template.RunAsync(model);

        var sb = new StringBuilder(result);
        var startIndex = 0;
        while (startIndex < sb.Length && char.IsWhiteSpace(sb[startIndex]))
            startIndex++;
        sb.Remove(0, startIndex);
        
        while (sb.Length > 0 && char.IsWhiteSpace(sb[sb.Length - 1]))
        {
            sb.Length--;
        }
        
        return sb.ToString();
    }
}