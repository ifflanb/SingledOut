using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using SingledOut.Model;

namespace SingledOutAndroid.Helpers
{
    public static class RestHelper
    {
//        public static async Task<T> PostAsync<T>(string uri, T data)
//        {
//            var json = JsonConvert.SerializeObject(data);
//            HttpResponseMessage response;
//
//            using (var httpClient = new HttpClient())
//            {
//                var postBody = json;
//                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//                response = await httpClient.PostAsync(uri, new StringContent(postBody, Encoding.UTF8, "application/json"));
//             }  
//            response.EnsureSuccessStatusCode();
//            var content = await response.Content.ReadAsStringAsync();
//            var task = await Task.Run(() => JsonConvert.DeserializeObject<T>(content));
//
//            return task;
//        }
    }
}