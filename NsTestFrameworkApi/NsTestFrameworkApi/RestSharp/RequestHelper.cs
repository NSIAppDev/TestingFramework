using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using RestSharp;
using RestSharp.Authenticators;


namespace NsTestFrameworkApi.RestSharp
{
    public static class RequestHelper
    {
        private static readonly ReaderWriterLockSlim MethodLock = new ReaderWriterLockSlim();

        public static RestClient GetRestClient(string url, string user = "", string password = "")
        {
            var client = new RestClient(url);

            if (user.Equals("") | password.Equals("")) return client;

            SetupAuthentication(url, user, password, client);

            return client;
        }

        public static void SetupAuthentication(string url, string user, string password, IRestClient client)
        {
            var credential = new CredentialCache
            {
                {new Uri(url), "NTLM", new NetworkCredential(user, password)}
            };

            client.Authenticator = new NtlmAuthenticator(credential);
            client.PreAuthenticate = true;
        }

        public static IRestResponse CreateRequest(string url, string resource, object bodyParameter, Method method)
        {
            MethodLock.EnterWriteLock();
            try
            {
                var request = BaseHelper.GetRequest(resource, bodyParameter, method);

                ServicePointManager.ServerCertificateValidationCallback =
                    (senderX, certificate, chain, sslPolicyErrors) => true;

                return GetRestClient(url).Execute(request);
            }
            finally
            {
                MethodLock.ExitWriteLock();
            }
        }

        public static IRestResponse CreateRequest(string url, string resource, Method method = Method.GET)
        {
            MethodLock.EnterWriteLock();
            try
            {
                var request = BaseHelper.GetRequest(resource, method);

                ServicePointManager.ServerCertificateValidationCallback =
                    (senderX, certificate, chain, sslPolicyErrors) => true;

                return GetRestClient(url).Execute(request);
            }
            finally
            {
                MethodLock.ExitWriteLock();
            }
        }

        public static IRestResponse CreateRequest(this RestClient client, string resource, object bodyParameter, Method method)
        {
            MethodLock.EnterWriteLock();
            try
            {
                var request = BaseHelper.GetRequest(resource, bodyParameter, method);

                ServicePointManager.ServerCertificateValidationCallback =
                    (senderX, certificate, chain, sslPolicyErrors) => true;

                return client.Execute(request);
            }
            finally
            {
                MethodLock.ExitWriteLock();
            }
        }

        public static IRestResponse CreateRequest(this RestClient client, string resource, Method method = Method.GET)
        {
            MethodLock.EnterWriteLock();
            try
            {
                var request = BaseHelper.GetRequest(resource, method);

                ServicePointManager.ServerCertificateValidationCallback =
                    (senderX, certificate, chain, sslPolicyErrors) => true;

                return client.Execute(request);
            }
            finally
            {
                MethodLock.ExitWriteLock();
            }
        }

        public static IRestRequest AddCookies(string endpoint, IDictionary<string, string> cookies, Method method = Method.POST)
        {
            var restRequest = new RestRequest(endpoint, method);
            foreach (var cookie in cookies)
            {
                restRequest.AddCookie(cookie.Key, cookie.Value);
            }
            return restRequest;
        }
    }
}
