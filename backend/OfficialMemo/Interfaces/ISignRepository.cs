using OfficialMemo.Models;

namespace OfficialMemo.Interfaces;

public interface ISignRepository
{

    public Task<IEnumerable<SignMessage>> Get();
    public Task<SignMessage> Get(Guid guid);
    public Task<Guid> Add(SignMessage signMessage);
    public void Delete(Guid guid);
    public Task<Guid> Update(SignMessage signMessage);
    //public string GetSignerName(string cmsData);

}