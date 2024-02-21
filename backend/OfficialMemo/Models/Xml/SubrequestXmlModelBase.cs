using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record SubrequestXmlModelBase: XmlModelBase
{
    [XmlElement("reply")]
    public string Reply { get; set; }
}


[Serializable]
[XmlRoot("root")]
public record RequestXmlModelBase : XmlModelBase
{

    [XmlAttribute("code")]
    public ReplyXml Reply { get; set; }

}