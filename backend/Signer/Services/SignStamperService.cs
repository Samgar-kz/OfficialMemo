using FileServer.Abstractions;
using PdfProcessor.Client;
using Signer.Models;

namespace Signer.Services;

public class SignStamperService
{
    private readonly PdfProcessor.Client.PdfApiClient _pdfApiClient;
    private readonly RazorViewToStringRenderer _razorViewToStringRenderer;

    public SignStamperService(PdfApiClient pdfApiClient, RazorViewToStringRenderer razorViewToStringRenderer)
    {
        _pdfApiClient = pdfApiClient;
        _razorViewToStringRenderer = razorViewToStringRenderer;
    }

    public async Task<FileResult> AddSignDataPageToPdf(Document document, SignData signData)
    {
        var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("SignDataPage", signData);
        var pdf = await _pdfApiClient.InsertPagesToUrl(document, htmlContent);
        return pdf;
    }
}