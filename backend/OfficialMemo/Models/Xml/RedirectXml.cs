using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record RedirectXml : ReplyXml
{
    [XmlElement("redirectTo")]
    public string RedirectTo { get; set; } = null!;
}