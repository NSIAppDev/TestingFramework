using RestSharp;

namespace NsTestFrameworkApi.RestSharp
{
    public static class BaseHelper
    {
        public static RestRequest GetRequest(string resource, object bodyParameter, Method method)
        {
            var request = new RestRequest(resource, method) { RequestFormat = DataFormat.Json };
            request.AddJsonBody(bodyParameter);
            return request;
        }

        public static RestRequest GetRequest(string resource, Method method = Method.GET)
        {
            var request = new RestRequest(resource, method) { RequestFormat = DataFormat.Json };
            return request;
        }
    }
}
