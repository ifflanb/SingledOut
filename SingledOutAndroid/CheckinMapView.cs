using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Locations;
using MobileSpace.Helpers;
using RangeSlider;
using System.Json;
using Newtonsoft.Json;
using System.Net;
using SingledOut.Model;
using Android.Gms.Maps;

namespace SingledOutAndroid
{
	public class CheckinMapView : Fragment
	{
		private MapHelper _mapHelper;
		private UIHelper _uiHelper;
		private CheckIn _checkInActivity;

		/// <summary>
		/// Gets or sets the map helper.
		/// </summary>
		/// <value>The map helper.</value>
		public MapHelper MapHelper { 
			get {
				return _mapHelper;
			}
			set {
				_mapHelper = value;
			}
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view =  inflater.Inflate(Resource.Layout.CheckinMapView, null);
			return view;
		}

		public override void OnStart ()
		{
			base.OnStart ();

			// Get reference to parent activity;
			_checkInActivity = (CheckIn)Activity;
			_uiHelper = new UIHelper ();

			// Get the map fragment.
			MapFragment mapfragment = (MapFragment)this.FragmentManager.FindFragmentById(Resource.Id.map);

			// Create map helper.
			_mapHelper = new MapHelper (_checkInActivity);
		
			// Show the map.
			_mapHelper.ShowMap (mapfragment, true, true);
		}

		public override void OnDestroyView ()
		{
			//userMarker.remove();

			Fragment f = (Fragment) FragmentManager.FindFragmentById(Resource.Id.map);        
			if (f != null) {
				FragmentManager.BeginTransaction().Remove(f).Commit();
			}

			base.OnDestroyView(); 
		}
	}
}

