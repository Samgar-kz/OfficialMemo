// using FileServer.Abstractions;
// using LocalizableStringLib;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Signer.Models;
// using Signer.Services;
//
// namespace Signer.Controllers;
//
// [Authorize]
// [ApiController]
// [Route("[controller]")]
// public class SignerController : ControllerBase
// {
//     private readonly RazorViewToStringRenderer _razorViewToStringRenderer;
//
//     public SignerController(RazorViewToStringRenderer razorViewToStringRenderer)
//     {
//         _razorViewToStringRenderer = razorViewToStringRenderer;
//     }
//
//     [HttpPost("add-sign-data-to-document")]
//     public async Task<ActionResult> InsertSignData(SignData signData)
//     {
//         var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync<SignData>("SignDataPage", signData);
//         return Ok(htmlContent);
//     }
//
//     // [HttpPost("insertSignDataPage")]
//     // public async Task<ActionResult> InsertSignData([FromForm] InsertSignDataPageRequest request, [FromServices] PdfProcessor.Client.PdfApiClient pdfApiClient)
//     // {
//     //     var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("SignDataPage", request.SignData);
//     //     using var sourcePdfStream = new MemoryStream();
//     //     await request.SourcePdf.CopyToAsync(sourcePdfStream);
//     //     sourcePdfStream.Seek(0, SeekOrigin.Begin);
//     //     var pdf = await pdfApiClient.InsertPagesFromHtml(sourcePdfStream, htmlContent);
//     //     return new FileStreamResult(pdf.ContentStream, pdf.ContentType)
//     //     {
//     //         FileDownloadName = $"signed_{DateTime.Now:MM_dd_yyyy__hh_mm}.pdf"
//     //     };
//     // }
//     
//     [HttpPost("insertSignDataPage")]
//     public async Task<ActionResult> InsertSignData([FromForm] InsertSignDataPageRequest request, [FromServices] PdfProcessor.Client.PdfApiClient pdfApiClient)
//     {
//         var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("SignDataPage", request.SignData);
//         var pdf = await pdfApiClient.InsertHtmlPageToPdf(request.Document, htmlContent);
//         return new FileStreamResult(pdf.ContentStream, pdf.ContentType)
//         {
//             FileDownloadName = $"signed_{DateTime.Now:MM_dd_yyyy__hh_mm}.pdf"
//         };
//     }
//     
//     [HttpPost("test")]
//     public async Task<ActionResult> InsertSignData(Document document, [FromServices] PdfProcessor.Client.PdfApiClient pdfApiClient)
//     {
//         var model = new SignData()
//         {
//             DocumentType = new LocalizableString("Исходящий документ", "Исходящий документ"),
//             Receivers = new[]
//             {
//                 new Client
//                 {
//                     Name = "БАНКИ ВТОРОГО УРОВНЯ"
//                 }
//             },
//             Sender = new Client
//             {
//                 Name = "НАО \"ГОСУДАРСТВЕННАЯ КОРПОРАЦИЯ \"ПРАВИТЕЛЬСТВО ДЛЯГРАЖДАН\"\""
//         
//             },
//             RegisterDate = DateTime.Now,
//             RegNum = "02-06-20/1889",
//             SignerSignature = new Signature
//             {
//                 Base64Text = "dsgfvcvxbvczxgcfgvjhb vcc hghbvhdcfb",
//                 SignedBy = new Employee { Name = "Жансейт Асылжан" },
//                 SignedDate = DateTime.Now,
//             },
//             RegistrarSignature = new Signature
//             {
//                 Base64Text = "dsgbcghg xcgf vcc hghhftghfgjhbvhdcfb",
//                 SignedBy = new Employee { Name = "Мұратбеков Ерасыл" },
//                 SignedDate = DateTime.Now,
//             }
//         };
//
//         
//         var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("SignDataPage", model);
//         var pdf = await pdfApiClient.InsertHtmlPageToPdf(document, htmlContent);
//         return new FileStreamResult(pdf.ContentStream, pdf.ContentType)
//         {
//             FileDownloadName = $"signed_{DateTime.Now:MM_dd_yyyy__hh_mm}.pdf"
//         };
//     }
// }