using System.Xml.Serialization;
namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record ReviewMessageXml:XmlModelBase
{
    [XmlElement("id")]
    public Guid Id { get; set; }

    [XmlElement("reply")]
    public ReplyXml Reply { get; set; }

}