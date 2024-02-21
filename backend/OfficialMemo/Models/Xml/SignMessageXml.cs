using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record SignMessageXml: OfficialMemoBase
{
    [XmlElement("reply")]
    public ReplyXml Reply { get; set; }
}