using FileServer.Abstractions;

namespace Signer.Models;

public class Signature
{
    public Employee SignedBy { get; set; } = null!;
    public DateTime SignedDate { get; set; }
    
    public string? Base64Text { get; set; }
    public Document? SignDocument { get; set; }
    public Document? Document { get; set; }
    
    public SignType? SignType { get; set; }
}