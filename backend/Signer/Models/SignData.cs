using LocalizableStringLib;

namespace Signer.Models;

public class SignData
{
    public LocalizableString DocumentType { get; set; } = null!;
    public string RegNum { get; set; } = null!;
    public DateTime RegisterDate { get; set; }

    public Client? Sender { get; set; }

    public Client[]? Receivers { get; set; }

    public Signature? SignerSignature { get; set; }
    public Signature? RegistrarSignature { get; set; }
}