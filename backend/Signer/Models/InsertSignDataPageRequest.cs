using FileServer.Abstractions;

namespace Signer.Models;

public class InsertSignDataPageRequest
{
    // public IFormFile SourcePdf { get; set; } = null!;
    public Document Document { get; set; } = null!;
    public SignData SignData { get; set; } = null!;
}