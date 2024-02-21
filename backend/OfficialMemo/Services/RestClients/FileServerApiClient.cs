using OfficialMemo.Models.Poco;
using System.Net.Http.Headers;

namespace OfficialMemo.Services.RestClients
{
    public class FileServerApiClient
    {
        public HttpClient _httpClient { get; set; }

        public FileServerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Document> UploadFileAsync(Stream stream, string fileName, string contentType = "application/pdf", CancellationToken cancellationToken = new())
        {
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            
            using var multipartFormContent = new MultipartFormDataContent();
            var fileStreamContent = new StreamContent(stream);
            fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            multipartFormContent.Add(fileStreamContent, name: "file", fileName: fileName);
            
            var response = await _httpClient.PostAsync("api/file", multipartFormContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), (int)response.StatusCode);
            var documentInfo = await response.Content.ReadFromJsonAsync<Document>(cancellationToken: cancellationToken);
            
            return documentInfo!;
        }
        
        public async Task<IEnumerable<Document>> UploadFilesAsync(IEnumerable<IFormFile> datas,string contentType= "application/pdf")
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            foreach (var item in datas)
            {
                var fileStreamContent = new StreamContent(item.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(fileStreamContent, "files", item.FileName??"printM.pdf");
            }
            HttpResponseMessage response = await _httpClient.PostAsync("api/", content);

            return await response.Content.ReadFromJsonAsync<IEnumerable<Document>>();
        }
        public async Task<List<string>> GetFilesAsBase64(List<Document> documents)
        {
            var base64List = new List<string>();
            foreach (var item in documents)
            {
                var id = item.Url.Substring(item.Url.IndexOf("id=") + 3);
                var res = await _httpClient.GetStringAsync("api/GetBase64?id=" + id);
                if (string.IsNullOrEmpty(res)) throw new Exception("FileServerApiClient => GetfileAsBase64 'response content returned null or empty' ");

                base64List.Add(res);
            }

            return base64List;
        }

        public async Task<string> GetfileAsBase64(string id)
        {

            string res = await _httpClient.GetStringAsync("api/GetBase64?id=" + id);
            if (string.IsNullOrEmpty(res)) throw new Exception("FileServerApiClient => GetfileAsBase64 'response content returned null or empty' ");

            return res;
        }

        public async Task<Stream> Getfile(string id)
        {

            var response = await _httpClient.GetAsync("api/?id=" + id);
            if (response.StatusCode!=System.Net.HttpStatusCode.OK) throw new Exception("FileServerApiClient => GetfileAsBase64 'response content returned null or empty' ");

            var ms = await response.Content.ReadAsStreamAsync();
            System.Net.Http.HttpContent content = response.Content;
            var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
            //return File(ms, "application/pdf", "result.pdf");

            return await _httpClient.GetStreamAsync("api/?id=" + id);
            

        }


        public async Task<string> UploadFileAsync(IFormFile file)
        {
            IEnumerable<IFormFile> files = new List<IFormFile>() { file };
            var documents = await UploadFilesAsync(files);
            List<Document> documentList = documents.ToList();
            return (documentList != null && documentList.Count > 0) ? documentList[0].Url : throw new Exception("Method UploadFilesAsync in service Shared.Services.FileServerApiService is Failed");

        }
    }
}
