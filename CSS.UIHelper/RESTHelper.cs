using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Json;
using System.Net;

namespace CSS.Helpers
{
	public class RestHelper
    {
		public HttpResponseMessage PostAsync(string uri, object data)
		{
			var httpClient = new HttpClient();
			var json = SerializeObject (data);

			HttpContent cont = new StringContent(json);
			cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			HttpResponseMessage response;

			try
			{
				response = httpClient.PostAsync(uri, cont).Result;
			}
			catch (AggregateException ex)
			{
				throw ex;
			}
			catch (WebException ex)
			{
				throw ex;
			}       
			catch (Exception ex)
			{
				throw ex;
			}

			return response;
		}

		public string SerializeObject(object data)
		{
			var json = JsonConvert.SerializeObject(data);
			return json;
		}

		public T DeserializeObject<T>(string json)
		{
			var objectClass = JsonConvert.DeserializeObject(json);
			return (T)objectClass;
		}
    }
}