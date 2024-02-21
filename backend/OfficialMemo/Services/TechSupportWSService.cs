using TechSupportWS;
using System.Xml;

namespace OfficialMemo.Services;

public class TechSupportWSService
{

    private readonly TechSupportSoapClient _techSupportSoapClient;

    public TechSupportWSService(TechSupportSoapClient techSupportSoapClient)
    {
        _techSupportSoapClient = techSupportSoapClient;
    }

    public async Task<int> GoToBlockProcess(Guid processGuid, string processCode, string blockId, string blockType)
    {
        var error = await _techSupportSoapClient.CheckRunningBlocksToStartAsync(processGuid.ToString());
        if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        
        var doc = new XmlDocument();

        var xmlStr = $"""
            <ns0:ATFProcessMeta xmlns:ns0="http://ATFProcBizTalk3.ATFProcessMeta">
                <Branch xmlns="">
                    <BranchGuid>{processGuid}</BranchGuid>            
                    <BranchNbr>1</BranchNbr>            
                    <ChildBranchNbr>0</ChildBranchNbr>            
                </Branch>
                <Block xmlns="">             
                    <BlockType>{blockType}</BlockType>             
                    <BlockID>{blockId}</BlockID>             
                    <Condition />            
                    <Error />             
                    <ErrorDetails />             
                    <MsgClass>-error-</MsgClass>             
                </Block>  
                <Info xmlns="">
                    <ProcessID>{processGuid}</ProcessID>           
                    <ProcessCode>{processCode}</ProcessCode>               
                    <ProcessVersion>1</ProcessVersion>
                <ParentID>{processGuid}</ParentID>
                    <MainID>{processGuid}</MainID>
                    <StartDateTime>{DateTime.Now.Date:s}</StartDateTime>
                <InitDocumentPath />
                    <RequestGuid />
                </Info>
                <Stack xmlns = "" />
            </ns0:ATFProcessMeta>
            """;
        doc.LoadXml(xmlStr);
        
        await _techSupportSoapClient.AddToQueueRetryAsync(doc);

        return 1;
    }
}