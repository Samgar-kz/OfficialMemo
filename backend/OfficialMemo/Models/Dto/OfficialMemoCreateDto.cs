namespace OfficialMemo.Models.Dto
{
    public class OfficialMemoCreateDto
    {
        public OfficialMemoCoreDataDto Data { get; set; } = null!;
        public string? HtmlDocument { get; set; }
    }
}
