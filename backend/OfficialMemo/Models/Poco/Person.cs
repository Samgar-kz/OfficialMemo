namespace OfficialMemo.Models.Poco;

public class Client
{
    public string? Id { get; set; }
    public string Name { get; set; }
}
public class Person: Client
{

    public string? Phones { get; set; }
    //public DateOnly BirthDate { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}