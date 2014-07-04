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
using Android.Views.Animations;
using System.Threading;
using Android.Locations;
using MobileSpace.Helpers;
using System.Net;
using System.Json;
using Newtonsoft.Json;
using System.Net.Http;
using Android.Text.Method;
using SingledOut.Model;
using RangeSlider;

namespace SingledOutAndroid
{
	[Activity (Label = "Check-In")]
	public class CheckIn : BaseActivity
	{
		private LocationManager _locationManager;
		private Button _btnCheckin;
		private MapHelper _mapHelper;
		private UIHelper _uiHelper;
		ProgressBar _spinner;
		private ActionBar.Tab _mapTab;
		private ActionBar.Tab _listViewTab;

		/// <summary>
		/// Gets or sets the button checkin.
		/// </summary>
		/// <value>The button checkin.</value>
		public Button BtnCheckin {
			get {
				return _btnCheckin;
			}
			set {
				_btnCheckin = value;
			}
		}

		/// <summary>
		/// Gets or sets the spinner.
		/// </summary>
		/// <value>The spinner.</value>
		public ProgressBar Spinner {
			get {
				return _spinner;
			}
			set {
				_spinner = value;
			}
		}

		private enum TabPosition
		{
			Map = 0,
			ListView = 1
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_uiHelper = new UIHelper ();
			_mapHelper = new MapHelper (this);

			// Set the action bar to show.
			ShowActionBarTabs = true;
			IsActionBarVisible = true;

			SetContentView (Resource.Layout.CheckIn);

			_uiHelper.OnTabSelectedClick += OnTabSelected;
			// Add map tab.
			_mapTab = _uiHelper.AddActionBarTab (this, Resource.String.maptabname, Resource.Drawable.globe);
			// Add listview tab.
			_listViewTab = _uiHelper.AddActionBarTab (this, Resource.String.listtabname, Resource.Drawable.listview);

			// Find checkin button.
			_btnCheckin = (Button)FindViewById (Resource.Id.btnCheckin);
			_btnCheckin.Click += btnCheckin_OnClick;

			// Set swipe activity.
			SwipeRightActivity = typeof(Tutorial2);

			// Show welcome back message.
			if (LastActivity == "Login" || LastActivity == "SplashPage") {
				ShowNotificationBox (string.Concat ("Welcome back ", CurrentUser.FirstName, "!"));
			}
		}

		/// <summary>
		/// Raises the tab selected event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void OnTabSelected (object sender, ActionBar.TabEventArgs e)
		{
			switch (((ActionBar.Tab)sender).Position)
			{
			case ((int)TabPosition.Map):
				Fragment mapFragment = new CheckinMapView ();
				e.FragmentTransaction.Replace(Resource.Id.checkinFrameLayout, mapFragment);
				break;
			case ((int)TabPosition.ListView):
				Fragment listViewFragment = new CheckinListView ();
				e.FragmentTransaction.Replace(Resource.Id.checkinFrameLayout, listViewFragment);
				break;
			}
		}

		protected void btnCheckin_OnClick(object sender, EventArgs eventArgs)
		{
			if (!_mapHelper.IsUserLocationSet) {
				// Start progress indicator.
				_spinner = (ProgressBar)FindViewById (Resource.Id.progressSpinner);
				_spinner.Visibility = ViewStates.Visible;

				_btnCheckin.Enabled = false;
				// Start the location manager.
				_locationManager = _mapHelper.InitializeLocationManager (true, 2000, 10);
			} 
			else {
				_mapHelper.RemoveMarker ();
				_btnCheckin.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.hide, 0, 0, 0);
				_btnCheckin.Text = "Make Me Visible";
			}
		}
	}
}

