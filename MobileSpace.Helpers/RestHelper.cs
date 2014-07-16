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

		public RestHelper ()
		{
			_securityHelper = new SecurityHelper ();
		}

		/// <summary>
		/// Posts the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		public HttpResponseMessage PostAsync(string uri, object data)
		{
			var httpClient = new HttpClient();
			var json = JsonConvert.SerializeObject (data);
			HttpResponseMessage response;

			HttpContent cont = new StringContent(json);
			cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

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
		/// Deletes the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		public HttpResponseMessage DeleteAsync(string uri)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

			try
			{
				response = httpClient.DeleteAsync(uri).Result;
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
		/// <param name="uri">uri.</param>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		public HttpResponseMessage Login(string uri, string username, string password)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

			httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);

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
		/// Retrieves the password.
		/// </summary>
		/// <returns>The password.</returns>
		/// <param name="uri">uri.</param>
		public HttpResponseMessage RetrievePassword(string uri)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

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
		/// Gets the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="uri">URI.</param>
		public HttpResponseMessage GetAsync(string uri)
		{
			var httpClient = new HttpClient();
			HttpResponseMessage response;

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