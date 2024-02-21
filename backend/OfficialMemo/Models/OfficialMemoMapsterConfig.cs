using Mapster;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Dto;
using OfficialMemo.Models.Poco;
using OfficialMemo.Models.Xml;

namespace OfficialMemo.Models;

public class OfficialMemoMapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<OfficialMemoModel, OfficialMemoDbo>
            .NewConfig()
            .TwoWays()
            .Map(dbo => dbo, message => message.Data)
            .Map(dbo => dbo.ApproveType, message => message.Data.ApprovalMode)
            .Map(dbo => dbo.SignerCode, "Data.Signer.Login")
            .Map(dbo => dbo.ExecutorCode, "Data.Executor.Login")
            // .Map(dbo => dbo.SignerCode, message => message.Data.Signer.Login)
            // .Map(dbo => dbo.ExecutorCode, message => message.Data.Executor.Login)
            // .Map(dbo => dbo.ConfidenceTypeId, message => message.Data.ConfidenceType.Id)
            .Map(dbo => dbo.ConfidenceType, message => message.Data.ConfidenceType)
            .Map(dbo => dbo.Summary, message => message.Data.Details)
            // .Map(dbo => dbo.InMessage, message => message.InMessage)
            ;
        TypeAdapterConfig<SignMessage, SignMessageDbo>
            .NewConfig()
            .TwoWays()
            .Map(dbo => dbo.SignedTime, message => message.SignedDate)
            .Map(dbo => dbo.RegisterSignedTime, message => message.RegisterSignedDate)
            .Map(dbo => dbo.SignDocumentName, message => message.SignDocument.Name)
            .Map(dbo => dbo.SignatureLink, message => message.SignDocument.Url);

        TypeAdapterConfig<SignMessageDbo, SignMessageDto>
            .NewConfig()
            .Map(dto => dto.SignDocument.Name, dbo => dbo.SignDocumentName)
            .Map(dto => dto.SignDocument.Url, dbo => dbo.SignatureLink)
            .TwoWays();


        //TypeAdapterConfig<SignMessageDto, SignMessageDbo>
        //    .NewConfig()
        //    .TwoWays()

        //    .Map(dbo => dbo.SignDocumentName, message => message.SignDocument.Name)
        //    .Map(dbo => dbo.SignatureLink, message => message.SignDocument.Url);
        TypeAdapterConfig<SignMessage, SignMessageDto>
            .NewConfig()
            .TwoWays()
        .Map(dbo => dbo.SignedTime, message => message.SignedDate);

        TypeAdapterConfig<OfficialMemoDto, OfficialMemoDbo>
            .NewConfig()
            .TwoWays()
            .Map(dbo => dbo, message => message.Data)
            .Map(dbo => dbo.ProcessData.ProcessGuid, message => message.ProcessGuid)
            .Map(dbo => dbo.ApproveType, message => message.Data.ApprovalMode)
             // .Map(dbo => dbo.SignerCode, message => message.Data.Signer.Login)
             // .Map(dbo => dbo.ExecutorCode, message => message.Data.Executor.Login)
             .Map(dbo => dbo.Summary, message => message.Data.Details)
            // .Map(dbo => dbo.ProcessData.ProcessGuid, message => message.ProcessGuid)
            .IgnoreNullValues(true);
        // .Map(dbo => dbo.InMessage, message => message.InMessage)
        ;
        TypeAdapterConfig<ApprovalResultDbo, ApprovalResult>.NewConfig().MapToConstructor(true);
        TypeAdapterConfig<OfficialMemoCoreData, OfficialMemoDbo>
            .NewConfig()
            .TwoWays()
            .Map(dbo => dbo.ApproveType, message => message.ApprovalMode);

        TypeAdapterConfig<OfficialMemoModel, OfficialMemoStart>.NewConfig().TwoWays()
            .Map(dbo => dbo, message => message.Data);
        TypeAdapterConfig<OfficialMemoDbo, OfficialMemoStart>.NewConfig()
            .Ignore(d => d.Signer)
            .Ignore(d => d.Executor)
            ;
        // .Ignore(d => d.Approvers)
        // .Map(d => d.ConfidenceType, s => s.ConfidenceType.DisplayTextRu);
        // .Map(dbo => dbo.Signer, message => new EmployeeXml { Code = message.SignerCode });

        TypeAdapterConfig<Employee, EmployeeXml>.NewConfig().TwoWays()
            .Map(dbo => dbo.Code, emp => emp.Login);
        TypeAdapterConfig<Person, PersonXml>.NewConfig().TwoWays()
            .Map(dbo => dbo.FirstName, person => person.Name);
        TypeAdapterConfig<Employee, ApproverXml>.NewConfig().TwoWays()
            .Map(dbo => dbo.Code, emp => emp.Login);
        TypeAdapterConfig<Employee, RecipientXml>.NewConfig().TwoWays()
            .Map(dbo => dbo.Code, emp => emp.Login);
        TypeAdapterConfig<string, EmployeeXml>.NewConfig().TwoWays()
            .Map(dbo => dbo.Code, emp => emp);
    }
}