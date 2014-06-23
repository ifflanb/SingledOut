using System;
using Android.Content;
using Android.Net;

namespace MobileSpace.Helpers
{
	public class ConnectionDetector
	{
		private Context _context;

		public ConnectionDetector(Context context){
			this._context = context;
		}

		/// <summary>
		/// Determines whether this instance is connected to the internet.
		/// </summary>
		/// <returns>true if this instance is connected to internet. Otherwise, false.</returns>
		public bool IsConnectedToInternet()
		{
			var connectivity = (ConnectivityManager) _context.GetSystemService(Context.ConnectivityService);
			if (connectivity != null) 
			{
				try
				{
				var info = connectivity.GetAllNetworkInfo();

				if (info != null) 
					for (int i = 0; i < info.Length; i++) 
						if (info[i].GetState() == NetworkInfo.State.Connected)
						{
							return true;
						}
				}
				catch (Exception ex) {

				}
			}
			return false;
		}
	}
}

