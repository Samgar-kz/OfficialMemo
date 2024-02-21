using LocalizableStringLib;

namespace Signer.Models;

public class Employee
{
    public string Login { get; set; } = null!;
    public string Name { get; set; } = null!;
    public LocalizableString Position { get; set; } = null!;
}