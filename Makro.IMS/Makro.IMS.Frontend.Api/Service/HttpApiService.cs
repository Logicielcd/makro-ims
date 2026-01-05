using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Makro.IMS.Frontend.Api.Service
{
    public static class HttpApiService
    {

        public static HttpResponseMessage Get(string host, string uri, string token)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {

                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                try
                {
                    response = client.GetAsync("api/" + uri).Result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            return response;
        }

        public static HttpResponseMessage Get(string host, string uri,object value, string token)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {

                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                try
                {                    
                    response = client.GetAsync("api/" + uri).Result;                    
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            return response;
        }

        public static async Task<HttpResponseMessage> Get(string host, string uri)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(host);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync("api/" + uri);

            }

            return response;
        }

        public static HttpResponseMessage Post(string host, string uri, object value, string token)
        {
            HttpResponseMessage response;

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                using (var client = new HttpClient(httpClientHandler))
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.BaseAddress = new Uri(host);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    try
                    {
                        response = client.PostAsJsonAsync("api/" + uri, value).Result;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }

                }
            }
            return response;
        }

        public static HttpResponseMessage Post(string host, string uri, object value)
        {
            HttpResponseMessage response;

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                using (var client = new HttpClient(httpClientHandler))
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.BaseAddress = new Uri(host);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //id == 0 means select all records    
                    response = client.PostAsJsonAsync("api/" + uri, value).Result;
                }
            }



            return response;
        }
    }
}
