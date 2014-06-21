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

		public UriCreator (String host, String root)
		{
			_uriBuilder = BuildRootPath(host, root);
		}

		/// <summary>
		/// Builds the root path.
		/// </summary>
		/// <returns>The root path.</returns>
		/// <param name="host">Host.</param>
		/// <param name="root">Root.</param>
		public UriBuilder BuildRootPath(string host, string root)
		{
			var uriBuilder = new UriBuilder {
				Host = host,
				Path = root
			};
			return uriBuilder;
		}			

		/// <summary>
		/// Builds the login.
		/// </summary>
		/// <returns>The login.</returns>
		public Uri Login(string path)
		{
			_uriBuilder.Query = path;
			return _uriBuilder.Uri;
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

