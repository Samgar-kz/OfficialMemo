using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record XmlModelBase
{
    [XmlAttribute("code")]
    public string Code { get; set; } = string.Empty;

    [XmlAttribute("ver")]
    public string Version { get; set; } = string.Empty;

    //[XmlAttribute("vguid")]
    //public string Vguid { get; set; }

    [XmlAttribute("guid")]
    public string GuidString
    {
        get => RequestGuid.HasValue ? RequestGuid.ToString() : string.Empty;
        set
        {
            if (Guid.TryParse(value, out Guid valueGuid)) RequestGuid = valueGuid;
            else RequestGuid = null;
        }
    }

    [XmlIgnore]
    public Guid? RequestGuid { get; set; }
}




