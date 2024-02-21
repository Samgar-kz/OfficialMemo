using System.Xml.Serialization;

namespace OfficialMemo.Models.Xml;

[Serializable]
[XmlRoot("root")]
public record OfficialMemoStart : XmlModelBase
{
    [XmlElement("messageGuid")]
    public Guid MessageGuid { get; set; }

    [XmlElement("executor")]
    public EmployeeXml? Executor { get; set; }

    [XmlElement("subject")]
    public string Subject { get; set; } = string.Empty;

    [XmlElement("language")]
    public string Language { get; set; } = string.Empty;

    [XmlElement("confidenceType")]
    public string ConfidenceType { get; set; } = string.Empty;

    [XmlElement("indexNomenclature")]
    public string IndexNomenclature { get; set; } = string.Empty;

    [XmlElement("amountPage")]
    public string? AmountPage { get; set; }

    [XmlArray("approvers")]
    [XmlArrayItem("approver")]
    public List<ApproverXml>? Approvers { get; set; }

    [XmlElement("approvalRequired")]
    public bool ApprovalRequired { get; set; }
    
    [XmlElement("registrarRequired")]
    public bool RegistrarRequired { get; set; }

    [XmlElement("approvalMode")]
    public string ApprovalMode { get; set; } = string.Empty;

    [XmlElement("signer")]
    public EmployeeXml Signer { get; set; } = new EmployeeXml();

    [XmlElement("registrarCode")]
    public string? RegistrarCode { get; set; }

    [XmlElement("summary")]
    public string Summary { get; set; } = string.Empty;

    [XmlArray("attachments")]
    [XmlArrayItem("attachment")]
    public List<string> Attachments { get; set; } = new List<string>();

    [XmlArray("recipients")]
    [XmlArrayItem("recipient")]
    public List<RecipientXml> Recipients { get; set; } = new List<RecipientXml>();

    [XmlElement("registerCode")]
    public string RegisterCode { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;
}