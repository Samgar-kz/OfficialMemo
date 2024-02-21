// using System.Diagnostics;
// using System.Reflection;
// using System.Text.Encodings.Web;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Razor;
// using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
// using Microsoft.AspNetCore.Mvc.ViewEngines;
// using Microsoft.Extensions.FileProviders;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace Signer.Services;
//
// public class EmbeddedRazorViewEngine : RazorViewEngine
// {
//     private readonly Assembly _assembly;
//     private readonly string _baseNamespace;
//
//     public EmbeddedRazorViewEngine(IRazorPageFactoryProvider pageFactory, IRazorPageActivator pageActivator, HtmlEncoder htmlEncoder, IOptions<MvcRazorRuntimeCompilationOptions> optionsAccessor, ILoggerFactory loggerFactory, DiagnosticListener diagnosticListener, IServiceProvider serviceProvider) : base(pageFactory, pageActivator, htmlEncoder, optionsAccessor, loggerFactory, diagnosticListener)
//     {
//         _assembly = Assembly.GetExecutingAssembly();
//         _baseNamespace = _assembly.GetName().Name!;
//
//         var embeddedFileProvider = new EmbeddedFileProvider(_assembly, _baseNamespace);
//         optionsAccessor.Value.FileProviders.Add(embeddedFileProvider);
//     }
//
//
//     public override ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
//     {
//         var viewPath = GetViewPath(viewName);
//         return base.FindView(context, viewPath, isMainPage);
//     }
//
//     private string GetViewPath(string viewName)
//     {
//         var resourceNames = _assembly.GetManifestResourceNames();
//         var viewResourceName = resourceNames.FirstOrDefault(r => r.EndsWith(viewName + ".cshtml", StringComparison.OrdinalIgnoreCase));
//
//         if (viewResourceName == null)
//         {
//             throw new InvalidOperationException($"The view '{viewName}' was not found.");
//         }
//
//         var viewPath = viewResourceName.Substring(_baseNamespace.Length).Replace(".", "/");
//         return viewPath;
//     }
// }