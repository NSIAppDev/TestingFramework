﻿using System.IO;
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
