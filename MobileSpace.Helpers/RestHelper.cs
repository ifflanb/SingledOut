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
		HttpClient _httpClient;

		public RestHelper (string token = "", int? userID = null)
		{
			_securityHelper = new SecurityHelper ();
			_httpClient = new HttpClient ();
			if (!string.IsNullOrEmpty (token) && userID.HasValue) {
				_httpClient = AddTokenToHeader (_httpClient, token, (int)userID);
			}
		}

		/// <summary>
		/// Adds the token to header.
		/// </summary>
		/// <returns>The token to header.</returns>
		/// <param name="client">Client.</param>
		/// <param name="token">Token.</param>
		private HttpClient AddTokenToHeader(HttpClient client, string token, int userID)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token, userID.ToString());
			return client;
		}

		/// <summary>
		/// Posts the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		public HttpResponseMessage PostAsync(string uri, object data)
		{
			var json = JsonConvert.SerializeObject (data);
			HttpResponseMessage response;

			HttpContent cont = new StringContent(json);
			cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			try
			{
				response = _httpClient.PostAsync(uri, cont).Result;
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
		/// Puts the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		public HttpResponseMessage PutAsync(string uri, object data)
		{
			var json = JsonConvert.SerializeObject (data);
			HttpResponseMessage response = null;

			HttpContent cont = new StringContent(json);
			cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			try
			{
				response = _httpClient.PutAsync(uri, cont).Result;
			}
			catch (AggregateException ex)
			{
				if (ex != null) {
					throw ex;
				}
			}
			catch (WebException ex)
			{
				if (ex != null) {
					throw ex;
				}
			}   
			catch (Exception ex) {
				throw ex;
			}
			return response;
		}

		/// <summary>
		/// Deletes the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		public HttpResponseMessage DeleteAsync(string uri)
		{
			HttpResponseMessage response;

			try
			{
				response = _httpClient.DeleteAsync(uri).Result;
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
			return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
		}

		/// <summary>
		/// Login the specified path, username and password.
		/// </summary>
		/// <param name="uri">uri.</param>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		public HttpResponseMessage Login(string uri, string username, string password)
		{
			HttpResponseMessage response;

			_httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);

			try
			{
				response = _httpClient.GetAsync(uri).Result;	
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
		/// Retrieves the password.
		/// </summary>
		/// <returns>The password.</returns>
		/// <param name="uri">uri.</param>
		public HttpResponseMessage RetrievePassword(string uri)
		{
			HttpResponseMessage response;

			try
			{
				response = _httpClient.GetAsync(uri).Result;	
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
		/// Gets the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		public HttpResponseMessage GetAsync(string uri)
		{
			HttpResponseMessage response;

			try
			{
				response = _httpClient.GetAsync(uri).Result;
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
	}
}
