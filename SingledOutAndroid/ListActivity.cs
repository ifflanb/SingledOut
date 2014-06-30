
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

namespace SingledOutAndroid
{
	[Activity (Label = "")]			
	public class NearbyPlaces : ListActivity
	{
		string[] data = {"one", "two", "three", "four", "five"} ;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ArrayAdapter adapter = new ArrayAdapter (this, Resource.Layout.TextViewItem, data);        
			ListAdapter = adapter;
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			base.OnListItemClick (l, v, position, id);
			Toast.MakeText (this, data [position],
				ToastLength.Short).Show ();
		}
	}
}

