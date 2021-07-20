using System.Net.Http;


namespace bulkio.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpResponseMessage Get(this HttpClient httpClient, string uri)
        {
            return httpClient.GetAsync(uri).GetAwaiter().GetResult();
        }
        public static string ReadAsString(this HttpContent content)
        {
            return content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        public static string ReadContentAsString(this HttpResponseMessage res) 
        {
            return res.Content.ReadAsString();
        }
    }
}