using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RazorEngineCore;
using Signer.Services;

namespace Signer;

public static class SignStamperExtensions
{
    public static IServiceCollection AddSignStamperService(this IServiceCollection services)
    {
        // // Register the RazorViewToStringRenderer
        // services.AddScoped<RazorViewToStringRenderer>();
        //
        // // Register necessary services
        // services.AddSingleton<IRazorViewEngine, RazorViewEngine>();
        // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        // services.AddSingleton<IModelMetadataProvider, DefaultModelMetadataProvider>();
        // services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        //
        // // Register TempData provider
        // services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        //
        // // Register services required for runtime Razor view compilation
        // services.AddRazorPages().AddRazorRuntimeCompilation();
        //
        // services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        // {
        //     options.FileProviders.Add(new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "Views")));
        // });
        //
        // services.AddScoped<SignStamperService>();
    
        var path = Path.Combine(AppContext.BaseDirectory, "Views");
        var directory = new DirectoryInfo(path);
        foreach (var file in directory.EnumerateFiles())
        {
            var hash = file.Name.Substring(0, file.Name.Length - file.Extension.Length).GetHashCode();
            RazorViewToStringRenderer.TemplateCache.GetOrAdd(hash, i =>
            {
                var razorEngine = new RazorEngine();
                var content = File.ReadAllText(file.FullName);
                // var contentNormalized = content.Substring(content.IndexOf("<html "), content.IndexOf("<html ") content.Length - content.LastIndexOf("</html>"));
                return razorEngine.Compile(content, builder =>
                {
                    builder.AddAssemblyReference(Assembly.GetExecutingAssembly());
                    builder.AddAssemblyReference(Assembly.GetAssembly(typeof(Signer.Models.SignData)));
                    builder.AddAssemblyReference(Assembly.GetAssembly(typeof(MemoryExtensions)));
                    builder.AddUsing("Signer.Models");
                    builder.AddUsing("System");
                    // builder.AddUsing("System.Memory");
                });
            });
        }

        services.AddScoped<IRazorEngine, RazorEngine>();
        services.AddScoped<RazorViewToStringRenderer>();
        services.AddScoped<SignStamperService>();
        
        return services;
    }
}