using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using UserWS;
using OfficialMemo.Models.Xml;
using Dapper;

namespace OfficialMemo.Services;

public class ProcessService
{
    private readonly UserWSSoapClient _userSoapClient;
    private readonly IDbConnection _dbConnection;

    public ProcessService(UserWSSoapClient userSoapClient, IDbConnection dbConnection)
    {
        _userSoapClient = userSoapClient;
        _dbConnection = dbConnection;
    }

    public async Task StartProcess<T>(T message, string userCode, string processName, string processVersion = "1.0") where T : XmlModelBase
    {
        message.Code = processName;
        message.Version = processVersion;
        await SendReplyMessageAsync(message, null, userCode);
    }
    public async Task SendReplyMessageAsync<T>(T message, Guid? requestGuid, string userCode) where T : XmlModelBase 
    {           
        message.RequestGuid = requestGuid;
        var xmlElement = Serialize(message).DocumentElement;
        await _userSoapClient.SendReplyMsgAsync(xmlElement, userCode);
    }

    public async Task<T> GetMessageDocument<T>(Guid requestGuid) where T : XmlModelBase
    {
        const string @q = @"
            select [MESSAGE] from ABProcSystem.dbo.MESSAGE_REQUEST mr with (nolock)
            inner join ABProcSystem.dbo.PROCESS_MSG pm with (nolock) on mr.MESSAGE_GUID = pm.MESSAGE_GUID
            where REQUEST_GUID = @requestGuid";
        var document = await _dbConnection.QueryFirstAsync<XmlDocument>(q, new { requestGuid });
        if (TryDeserialize<T>(document.OuterXml, out var result))
        {
            return result;
        }

        throw new XmlException();
    } 
    
    public async Task<T> GetMessageDocumentByProcessGuid<T>(Guid processGuid) where T : XmlModelBase
    {
        const string @q = @"
            select [MESSAGE] from ABProcSystem.dbo.MESSAGE_REQUEST mr with (nolock)
            inner join ABProcSystem.dbo.PROCESS_MSG pm with (nolock) on mr.MESSAGE_GUID = pm.MESSAGE_GUID
            where pm.PROCESS_GUID = @processGuid";
        var document = await _dbConnection.QueryFirstAsync<XmlDocument>(q, new { processGuid });
        if (TryDeserialize<T>(document.OuterXml, out var result))
        {
            return result;
        }

        throw new XmlException();
    }


    public static XmlDocument Serialize<T>(T obj, bool omitNamespaces = true)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        var serializer = new XmlSerializer(typeof(T));

        using var memoryStream = new MemoryStream();
        if (omitNamespaces)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(memoryStream, obj, ns);
        }
        else
        {
            serializer.Serialize(memoryStream, obj);
        }

        memoryStream.Position = 0;
        var settings = new XmlReaderSettings { IgnoreWhitespace = true };
        using var reader = XmlReader.Create(memoryStream, settings);
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(reader);
        return xmlDoc;
    }

    private bool TryDeserialize<T>(string xml, out T result)
    {

        result = default;
        try
        {
            using var stringReader = new StringReader(xml);
            XDocument xdoc = XDocument.Load(stringReader, LoadOptions.None);
            stringReader.Close();

            xdoc.Descendants().Where(x => string.IsNullOrEmpty(x.Value)).Remove();
            var serializer = new XmlSerializer(typeof(T));
            using var reader = xdoc.CreateReader();
            result = (T)serializer.Deserialize(reader);
            reader.Close();
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}