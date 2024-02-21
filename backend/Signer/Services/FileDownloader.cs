using FileServer.Abstractions;
using Signer.Models;

namespace Signer.Services;

// public class FileDownloader
// {
//     private readonly HttpClient _httpClient;
//
//     public FileDownloader(HttpClient httpClient)
//     {
//         _httpClient = httpClient;
//     }
//
//     public async Task<FileResult> DownloadFileAsync(string url)
//     {
//         var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
//         if (!response.IsSuccessStatusCode)
//         {
//             throw new Exception($"Error downloading file: {response.ReasonPhrase}");
//         }
//
//         var contentDisposition = response.Content.Headers.ContentDisposition;
//         var filename = contentDisposition?.FileNameStar ?? contentDisposition?.FileName ?? "unknown";
//
//         var fileResult = new FileResult
//         {
//             Name = filename,
//             Size = response.Content.Headers.ContentLength ?? 0,
//             ContentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
//             ContentStream = await response.Content.ReadAsStreamAsync()
//         };
//
//         return fileResult;
//     }
// }