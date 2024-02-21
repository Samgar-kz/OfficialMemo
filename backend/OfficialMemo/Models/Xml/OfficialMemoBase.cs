using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record OfficialMemoBase: XmlModelBase
{
    [XmlElement("id")]
    public Guid Id { get; set; }
}