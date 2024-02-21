using System.Xml.Serialization;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record ReplyXml : XmlModelBase
{
    [XmlElement("dec")]
    public string ReplyDecision { get; set; } = string.Empty;   

    [XmlElement("decn")]
    public string ReplyDecisionName { get; set; } = string.Empty;

    [XmlElement("comment")]
    public string ReplyComment { get; set; } = string.Empty;

    [XmlElement("repliedBy")]
    public string RepliedBy { get; set; } = string.Empty;

    [XmlElement("replyDate")]
    public DateTime ReplyDate { get; set; }

    [XmlArray("documents")]
    [XmlArrayItem("document")]
    public List<Document>? ReplyDocuments { get; set; }
}