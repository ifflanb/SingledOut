using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingledOut.Model;
using System;
using System.Json;

namespace CSS.Helpers
{
	public class RestHelper<T>
    {

		public HttpResponseMessage PostAsync(string uri, object data)
		{
			var httpClient = new HttpClient();
			var json = JsonConvert.SerializeObject(data);

			HttpContent cont = new StringContent(json);
			cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = httpClient.PostAsync(uri, cont).Result;

			response.EnsureSuccessStatusCode();

			return response;
		}

//		public void Post(string url, T objectClass) {
//
//
//			HttpClient client = new HttpClient();
//
//			client.BaseAddress = new Uri(url);
//
//			// Add an Accept header for JSON format.
//			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//					
//			// 1. Post a new object
//			var json = JsonConvert.SerializeObject(objectClass);
//
//			var response = client.PostAsync(url, new StringContent(json)).Result;
//			if  (response.IsSuccessStatusCode)
//			{
//				// display the new employee
//
//			}
//			else
//			{
//				//Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
//			}
//		}

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