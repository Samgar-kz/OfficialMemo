using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record OfficialMemoRework : OfficialMemoBase
{
    [XmlElement("message")] public OfficialMemoStart Message { get; init; } = null!;
    
    [XmlElement("reply")]
    public ReplyXml? Reply { get; init; }
}