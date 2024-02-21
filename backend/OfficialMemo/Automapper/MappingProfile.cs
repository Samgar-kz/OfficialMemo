using AutoMapper;
using Microsoft.AspNetCore.SignalR.Protocol;
using OfficialMemo.Models;
using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Dto;
using OfficialMemo.Models.Poco;
using OfficialMemo.Models.Xml;

namespace OfficialMemo.Automapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDbo>().ReverseMap();
        CreateMap<Employee, EmployeeDto>().ReverseMap();
        CreateMap<Employee, EmployeeShortDto>().ReverseMap();
        CreateMap<OfficialMemoModel, OfficialMemoDto>()
             // .ForMember(outMessage => outMessage.MessageGuid, opt => opt.MapFrom(outMessage => outMessage.MessageGuid))
             .ReverseMap();

        CreateMap<OfficialMemoCreateDto, OfficialMemoModel>()
            .ForMember(outMessageCreate => outMessageCreate.DocumentUrl, opt => opt.MapFrom(outMessage => outMessage.HtmlDocument));
        CreateMap<OfficialMemoModel, OfficialMemoDbo>()
             .ForMember(outMessage => outMessage.ExecutorCode, opt => opt.MapFrom(outMessage => outMessage.Data.Executor.Login))
             .ForMember(outMessage => outMessage.SignerCode, opt => opt.MapFrom(outMessage => outMessage.Data.Signer.Login))
             .ReverseMap()
             .ForPath(outMessage => outMessage.Data.Executor.Login, opt => opt.MapFrom(outMessage => outMessage.ExecutorCode))
             .ForPath(outMessage => outMessage.Data.Signer.Login, opt => opt.MapFrom(outMessage => outMessage.SignerCode));

        CreateMap<SignMessage, SignMessageDbo>()
             .ForMember(signMessageDbo => signMessageDbo.SignedTime, opt => opt.MapFrom(signMessage => signMessage.SignedDate))
             .ForMember(signMessageDbo => signMessageDbo.RegisterSignedTime, opt => opt.MapFrom(signMessage => signMessage.RegisterSignedDate))
             .ForMember(signMessageDbo => signMessageDbo.SignDocumentName, opt => opt.MapFrom(signMessage => signMessage.SignDocument.Name))
             .ForMember(signMessageDbo => signMessageDbo.SignatureLink, opt => opt.MapFrom(signMessage => signMessage.SignDocument.Url))
            .ReverseMap();
        CreateMap<SignMessage, SignMessageDto>().ReverseMap();

        CreateMap<Employee, EmployeeDbo>().ReverseMap();
        CreateMap<EmployeeXml, EmployeeDto>().ReverseMap();

        CreateMap<OfficialMemoStart, OfficialMemoModel>().ReverseMap();
        CreateMap<OfficialMemoStart, OfficialMemoModel>().ReverseMap();

        CreateMap<Employee, EmployeeXml>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Login))
            .ReverseMap();

        CreateMap<Employee, ApproverXml>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Login))
            .ReverseMap();        
        
        CreateMap<Employee, RecipientXml>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Login))
            .ReverseMap();

        CreateMap<Person, PersonXml>().ReverseMap();
    }
}