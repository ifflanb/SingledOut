using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using Newtonsoft.Json;

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

		/// <summary>
		/// Posts the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		public HttpResponseMessage PostAsync(string path, object data)
		{
			var httpClient = new HttpClient();
			var json = JsonConvert.SerializeObject (data);
			HttpResponseMessage response;

			HttpContent cont = new StringContent(json);
			cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var uri = _uriCreator.Login(path);

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


			return response;
		}

		/// <summary>
		/// Creates the basic header.
		/// </summary>
		/// <returns>The basic header.</returns>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		public AuthenticationHeaderValue CreateBasicHeader(string username, string password)
		{
			byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
			//System.Diagnostics.Debug.WriteLine("AuthenticationHeaderValue" + new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray)));
			return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
		}

		/// <summary>
		/// Login the specified path, username and password.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		public HttpResponseMessage Login(string path, string username, string password)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

			httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);

			var uri = _uriCreator.Login(path);

			try
			{
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

			return response;
		}

		/// <summary>
		/// Searchs the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
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
    }
}