using AutoMapper;
using Dapper;
using OfficialMemo.Interfaces;
using OfficialMemo.Models;
using OfficialMemo.Models.Dbo;
using System.Data;
using System.Security.Cryptography.X509Certificates;
namespace OfficialMemo.Services;

public class SignRepository : ISignRepository
{
    private readonly IDbConnection _requestContext;
    private readonly IMapper _mapper;
    public SignRepository(IDbConnection requestContext, IMapper mapper)
    {
        //_context = context;
        _mapper = mapper;
        _requestContext = requestContext; // new DapperContext(context._configuration, "SqlRequestDB");
    }

    public async Task<Guid> Add(SignMessage signMessage)
    {
        var signMessageDbo = _mapper.Map<SignMessageDbo>(signMessage);

        const string query = @"
            declare @isExist int=0;
            select @isExist=count(MessageGuid)
            from Request.OffMemo.SignedData with (nolock)
            where MessageGuid = @MessageGuid
            if(@isExist<1)
            begin 
                INSERT INTO Request.OffMemo.SignedData (MessageGuid, Data, Signature, SignedBy, SignedTime, SignatureLink,SignDocumentName,SignType) 
                VALUES(@MessageGuid, @Data, @Signature, @SignedBy, @SignedTime, @SignatureLink, @SignDocumentName,@SignType)
            end
            else
            begin
            update Request.OffMemo.SignedData
            set
                Data=@Data,
                Signature=@Signature,
                SignedBy=@SignedBy,
                SignedTime=@SignedTime,
                SignatureLink=@SignatureLink,
                SignDocumentName=@SignDocumentName,
                SignType=@SignType

            where MessageGuid = @MessageGuid
            end
        ";

        await _requestContext.ExecuteAsync(query, signMessageDbo);
        return signMessageDbo.MessageGuid;
    }

    public Task<IEnumerable<SignMessage>> Get()
    {
        throw new NotImplementedException();
    }

    public async Task<SignMessage> Get(Guid guid)
    {

        const string query = @"
            SELECT TOP 1 * FROM Request.OffMemo.SignedData with (nolock)
            WHERE MessageGuid = @MessageGuid
        ";
        SignMessageDbo signMessageDbo = await _requestContext.QueryFirstOrDefaultAsync<SignMessageDbo>(query, new
        {
            @MessageGuid = guid
        });
        var signMessage = _mapper.Map<SignMessage>(signMessageDbo);

        return signMessage;
    }

    public async void Delete(Guid guid)
    {

        const string query = @"
            DELETE Request.OffMemo.SignedData 
            WHERE MessageGuid = @MessageGuid
        ";
        await _requestContext.ExecuteAsync(query, new
        {
            @MessageGuid = guid
        });

    }

    public async Task<Guid> Update(SignMessage signMessage)
    {
        var signMessageDbo = _mapper.Map<SignMessageDbo>(signMessage);

        const string query = @"
            UPDATE Request.OffMemo.SignedData 
            SET RegisterSignature = @RegisterSignature
            ,RegisterSignedBy = @RegisterSignedBy
            ,RegisterSignedTime = @RegisterSignedTime
            WHERE MessageGuid = @MessageGuid
        ";
        await _requestContext.ExecuteAsync(query, new { 
            @RegisterSignature = signMessageDbo.RegisterSignature,
            @RegisterSignedBy = signMessageDbo.RegisterSignedBy,
            @RegisterSignedTime = signMessageDbo.RegisterSignedTime,
            @MessageGuid = signMessageDbo.MessageGuid,
        });
        return signMessageDbo.MessageGuid;
    }


    //public string GetSignerName(string cmsData)
    //{
    //    //ContentInfo contentInfo = new System.Security.Cryptography.Pkcs.ContentInfo();

    //    // Создаем объект SignedCms и загружаем в него данные CMS
    //    SignedCms signedCms = new SignedCms();
    //    signedCms.Decode(Convert.FromBase64String(cmsData));

    //    // Получаем коллекцию сертификатов из подписи CMS
    //    X509Certificate2Collection certificates = signedCms.Certificates;

        

    //    // Проверяем, есть ли сертификаты в коллекции
    //    if (certificates.Count > 0)
    //    {
    //        // Извлекаем первый сертификат из коллекции (можно выбрать нужный вам сертификат)
    //        X509Certificate2 certificate = certificates[0];

    //        // Дальше вы можете использовать сертификат по своему усмотрению
    //        // Например, вы можете получить информацию о сертификате:
    //        string subject = certificate.Subject;
    //        string CN = subject.Substring(subject.LastIndexOf("CN=") + 3);
    //        int gIndex = subject.IndexOf("G=") + 2;
    //        string docAuthor = CN + " " + subject.Substring(gIndex, subject.IndexOf(",", gIndex) - gIndex);
    //        return docAuthor;
    //        // и т.д.
    //    }
    //    throw new Exception("В подписи регистратора не найден сертификат");
        
    //}
}