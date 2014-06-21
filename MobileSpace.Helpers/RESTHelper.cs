using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Json;
using System.Net;

namespace MobileSpace.Helpers
{
	public class RestHelper
    {
		SecurityHelper _securityHelper;
		UriCreator _uriCreator;

		public RestHelper (string host, string root)
		{
			_securityHelper = new SecurityHelper ();
			_uriCreator = new UriCreator (host, root);
		}

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

		public AuthenticationHeaderValue CreateBasicHeader(string username, string password)
		{
			password = _securityHelper.CreateHash (password);
			byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
			//System.Diagnostics.Debug.WriteLine("AuthenticationHeaderValue" + new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray)));
			return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
		}

		public HttpResponseMessage Login(string path, string username, string password)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

			httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);

			try
			{
				var uri = _uriCreator.Login(path);
				response = httpClient.GetAsync(uri).Result;
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

		public HttpResponseMessage SearchAsync(string uri, string username, string password)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

			httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);

			try
			{
				uri = string.Concat(uri);
				response = httpClient.GetAsync(uri).Result;
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
			var objectClass = JsonConvert.DeserializeObject<T>(json);

			return objectClass;
		}
    }
}