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

namespace CSS.Helpers
{
	public class UriCreator<T>
	{
		private UriBuilder _uriBuilder;

		public UriCreator(String host, String path)
		{
			_uriBuilder = new UriBuilder();
			_uriBuilder.Host = host;
			_uriBuilder.Path = path;
		}

		public Uri Build(UsersSearchParameters sp)
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
			if(!string.IsNullOrEmpty(sp.Username))
			{
				_uriBuilder.Query = string.Concat("Username=", sp.Username);
			}
			if(!string.IsNullOrEmpty(_uriBuilder.Query))
			{
				_uriBuilder.Query = string.Concat("?", _uriBuilder.Query);
			}

				
			return _uriBuilder.Uri;
		}
	}
}

