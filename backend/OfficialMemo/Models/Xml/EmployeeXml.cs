using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
public class EmployeeXml : PersonXml
{
    [XmlElement("code")]
    public string Code { get; set; }
    public override string ToString()
    {
        return Code;
    }
}

public class ApproverXml : EmployeeXml
{
    [XmlElement("next")]
    public ApproverXml Next { get; set; }
}

public class RecipientXml : EmployeeXml {}