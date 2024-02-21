using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
public class PersonXml
{
    [XmlElement("firstName")]
    public string FirstName { get; set; }

    [XmlElement("middleName")]
    public string MiddleName { get; set; }

    [XmlElement("lastName")]
    public string LastName { get; set; }
    public override string ToString()
    {
        return LastName + " " + FirstName + " " + MiddleName;
    }
}