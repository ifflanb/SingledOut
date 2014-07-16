using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SingledOut.SearchParameters;

namespace MobileSpace.Helpers
{
	public class UriCreator
	{
		private UriBuilder _uriBuilder;
		private string _rootPath;
		private string _hostPath;

		public UriCreator (String host, String root)
		{
			_hostPath = host;
			_rootPath = root;
		}

		/// <summary>
		/// Builds the root path.
		/// </summary>
		/// <returns>The root path.</returns>
		/// <param name="host">Host.</param>
		/// <param name="path">Path.</param>
		public UriBuilder BuildRootPath(string path)
		{
			var uriBuilder = new UriBuilder {
				Host = _hostPath,
				Path = path
			};
			return uriBuilder;
		}			

		/// <summary>
		/// Googles the place API nearby places.
		/// </summary>
		/// <returns>The place API nearby places.</returns>
		/// <param name="googleApiKey">Google API key.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		/// <param name="radius">Radius.</param>
		/// <param name="types">Types.</param>
		public string GooglePlaceApiNearbyPlaces(string nearbyPlacesUri, string googleApiKey, double latitude, double longitude, int radius, string placeTypes)
		{
			var path = string.Concat(_rootPath, nearbyPlacesUri);
			_uriBuilder = BuildRootPath(path);	
			_uriBuilder.Scheme = "https"; 
			var uri = _uriBuilder.Uri.AbsoluteUri;//"&radius=", radius,
			uri += string.Concat("?rankby=distance&location=", latitude, ",", longitude, "&types=", placeTypes, "&key=", googleApiKey);

			return uri;
		}

		/// <summary>
		/// User the specified user.
		/// </summary>
		/// <param name="user">User.</param>
		public string User(string userUri)
		{
			var path = string.Concat(_rootPath, userUri);
			_uriBuilder = BuildRootPath(path);

			return _uriBuilder.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Users locations.
		/// </summary>
		/// <returns>The locations.</returns>
		/// <param name="userLocationsUri">User locations URI.</param>
		public string UserLocations(string userLocationsUri)
		{
			var path = string.Concat(_rootPath, userLocationsUri);
			_uriBuilder = BuildRootPath(path);

			return _uriBuilder.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Deletes the user locations.
		/// </summary>
		/// <returns>The user locations.</returns>
		/// <param name="userLocationsUri">User locations URI.</param>
		/// <param name="ID">I.</param>
		public string DeleteUserLocations(string userLocationsUri, string deleteuserlocation, string ID)
		{
			var path = string.Concat(_rootPath, userLocationsUri, "/", deleteuserlocation, "/", ID);
			_uriBuilder = BuildRootPath(path);

			return _uriBuilder.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Builds the login.
		/// </summary>
		/// <returns>The login.</returns>
		public string Login(string loginUri)
		{
			var path = string.Concat(_rootPath, loginUri);
			_uriBuilder = BuildRootPath(path);

			return _uriBuilder.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Builds the login.
		/// </summary>
		/// <returns>The login.</returns>
		public string RetrievePassword(string retrievePasswordUri, string email)
		{
			var path = string.Concat(_rootPath, retrievePasswordUri);
			_uriBuilder = BuildRootPath(string.Concat(path,email));

			return _uriBuilder.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Registers the account.
		/// </summary>
		/// <returns>The account.</returns>
		/// <param name="registerAccountdUri">Register accountd URI.</param>
		public string RegisterAccount(string registerAccountdUri)
		{
			var path = string.Concat(_rootPath, registerAccountdUri);
			_uriBuilder = BuildRootPath(path);

			return _uriBuilder.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Builds the search.
		/// </summary>
		/// <returns>The search.</returns>
		/// <param name="sp">Sp.</param>
		public Uri BuildSearch(UsersSearchParameters sp)
		{
			if(!string.IsNullOrEmpty(sp.FacebookUserName))
			{
				_uriBuilder.Query = string.Concat("FacebookUserName=", sp.FacebookUserName);
			}
			if(!string.IsNullOrEmpty(sp.FirstName))
			{
				_uriBuilder.Query = string.Concat("FirstName=", sp.FirstName);
			}
			if(!string.IsNullOrEmpty(sp.Surname))
			{
				_uriBuilder.Query = string.Concat("Surname=", sp.Surname);
			}
			if(!string.IsNullOrEmpty(sp.Sex))
			{
				_uriBuilder.Query = string.Concat("Sex=", sp.Sex);
			}
			if(!string.IsNullOrEmpty(sp.Email))
			{
				_uriBuilder.Query = string.Concat("Email=", sp.Email);
			}
			if(!string.IsNullOrEmpty(_uriBuilder.Query))
			{
				_uriBuilder.Query = string.Concat("?", _uriBuilder.Query);
			}
				
			return _uriBuilder.Uri;
		}	
	}
}

