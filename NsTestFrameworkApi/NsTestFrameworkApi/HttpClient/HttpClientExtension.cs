using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace NsTestFrameworkApi.HttpClient
{
    public static class HttpClientExtension
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this System.Net.Http.HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this System.Net.Http.HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);

            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Content = new StringContent(dataAsString);
            return httpClient.SendAsync(request);
        }

        public static Task<HttpResponseMessage> PostFile(this System.Net.Http.HttpClient httpClient, string resource, string filePath)
        {
            using var requestContent = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            var fileName = Path.GetFileName(filePath);
            requestContent.Add(new StreamContent(fileStream), "file", fileName);
            return httpClient.PostAsync(resource, requestContent);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this System.Net.Http.HttpClient httpClient, string resource, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PutAsync(resource, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(dataAsString);
        }

        public static async Task<T> GetDeserializeXml<T>(this HttpContent xmlContent)
        {
            var dataAsString = await xmlContent.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(T));
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(dataAsString));
            return (T)serializer.Deserialize(memStream);
        }
    }
}
