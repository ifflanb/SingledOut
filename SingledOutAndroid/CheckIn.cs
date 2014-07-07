﻿using System;
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
		private Location _currentLocation;
		private UriCreator _googleApiUriCreator;
		private RestHelper _restHelper;
		private GooglePlacesResponse _placesFound;
		private CustomListAdapter _adapter;
		private AlertDialog _alertDialog;
		private UriCreator _uriCreator;
		private RangeSliderView _ageSlider;
		private AnimationHelper _animationHelper;

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

			// Set the action bar to show.
			ShowActionBarTabs = true;
			IsActionBarVisible = true;

			SetContentView (Resource.Layout.CheckIn);

			SlidingDrawer slidingDraw = (SlidingDrawer)FindViewById (Resource.Id.slidingDrawer);
			slidingDraw.BringToFront ();

			// Set the age slider up.
			_ageSlider = (RangeSliderView)FindViewById (Resource.Id.ageslider);
			_ageSlider.LeftValueChanged += value => {
				SetAgeRangeText();
			};

			_ageSlider.RightValueChanged += value => {
				SetAgeRangeText();
			};
			SetAgeRangeText();

			// Instantiate the helpers.
			_uiHelper = new UIHelper ();
			_mapHelper = new MapHelper (this);
			_restHelper = new RestHelper();
			_animationHelper = new AnimationHelper ();
			_uriCreator = new UriCreator(Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));
			// Create uri creator for Google Api related stuff.
			_googleApiUriCreator = new UriCreator (Resources.GetString(Resource.String.googleapihost), Resources.GetString(Resource.String.googleapipath));
			// Set tab selected event handler.
			_uiHelper.OnTabSelectedClick += OnTabSelected;
			// Add map tab.
			_mapTab = _uiHelper.AddActionBarTab (this, Resource.String.maptabname, Resource.Drawable.globe);
			// Add listview tab.
			_listViewTab = _uiHelper.AddActionBarTab (this, Resource.String.listtabname, Resource.Drawable.listview);

			// Set the location updated event.
			_mapHelper.OnLocationUpdated += LocationUpdated;

			// Find checkin button.
			_btnCheckin = (Button)FindViewById (Resource.Id.btnCheckin);
			_btnCheckin.Click += btnCheckin_OnClick;
			_btnCheckin.BringToFront ();

			var checkInChildLayout = FindViewById<LinearLayout> (Resource.Id.checkInChildLayout);
			var height = _animationHelper.GetScreenHeight (this);
			_animationHelper.SetLayoutHeight (checkInChildLayout, (int)(height * 0.60));

			// Set swipe activity.
			SwipeRightActivity = typeof(Tutorial2);

			// Show welcome back message.
			if (LastActivity == "Login" || LastActivity == "SplashPage") {
				ShowNotificationBox (string.Concat ("Welcome back ", CurrentUser.FirstName, "!"));
			}
		}

		/// <summary>
		/// Sets the age range text.
		/// </summary>
		private void SetAgeRangeText()
		{
			var agetosee = (TextView)FindViewById (Resource.Id.agetosee);
			agetosee.Text = String.Format ("Age from {0} to {1}", (int)_ageSlider.LeftValue, (int)_ageSlider.RightValue);
		}

		/// <summary>
		/// Saves the user location to the database.
		/// </summary>
		/// <param name="googlePlace">Google place.</param>
		private void SaveUserLocation(GooglePlace googlePlace)
		{
			// Save the users location to the database.
			var uri = _uriCreator.UserLocations (Resources.GetString (Resource.String.apiurluserlocations));
			var userLocationModel = CreateUserLocation (googlePlace);
			_restHelper.PostAsync (uri, userLocationModel);

		}

		/// <summary>
		/// Placeses the dialog_ on cancel click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void PlacesDialog_OnCancelClick(object sender, EventArgs eventArgs)
		{
			_alertDialog.Dismiss ();
			BtnCheckin.Enabled = true;
		}

		/// <summary>
		/// Creates a user location model.
		/// </summary>
		/// <returns>The user location.</returns>
		/// <param name="googlePlace">Google place.</param>
		private UserLocationModel CreateUserLocation(GooglePlace googlePlace)
		{
			var model = new UserLocationModel {
				CreatedDate = DateTime.UtcNow,
				UpdateDate = DateTime.UtcNow,
				Latitude = googlePlace.Latitude,
				Longitude = googlePlace.Longitude,
				UserID = int.Parse(GetUserPreference("UserID"))
			};
			return model;
		}

		/// <summary>
		/// List view item click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void ListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			_alertDialog.Dismiss ();
			// Get the Google Place object for the item selected
			var googlePlace = _adapter.GetItemAtPosition (e.Position);
			// Add a marker for the users position.
			_mapHelper.SetMarker (googlePlace.Latitude, googlePlace.Longitude, 16, "You are here!", Resource.Drawable.logopindialog, true); 
			// Set checkin button to 'Hide me'
			BtnCheckin.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.show,0, 0, 0);
			BtnCheckin.Text = "Hide Me";
			//SaveUserLocation (googlePlace);
		}

		/// <summary>
		/// Adds the places.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void AddPlaces_Click(object sender, EventArgs e)
		{
			_alertDialog.Dismiss();

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

		/// <summary>
		/// Locations the updated.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected async void LocationUpdated(object sender, LocationUpdatedEventArgs e)
		{
			_currentLocation = e.Location;
			if (_currentLocation == null)
			{
				ShowNotificationBox ("Could not determine your location");
			}
			else
			{
				// Stop the location listener.
				_mapHelper.StopLocationListener();

				// Make request to Google Places API to find places near here.
				var googleApiNearbyPlacesUri = Resources.GetString (Resource.String.googleapiurinearbyplaces);
				var placeTypes = Resources.GetString (Resource.String.googleapiplacetypes);

				// Create the Google Places Nearby Uri.
				var uri = _googleApiUriCreator.GooglePlaceApiNearbyPlaces (
					googleApiNearbyPlacesUri,
					GoogleApiKey,
					_currentLocation.Latitude,
					_currentLocation.Longitude,
					5000,
					placeTypes);

				// Create task to Save Singled Out Details.
				var response = FactoryStartNew (() => _restHelper.GetAsync (uri.ToString()));
				if (response != null) {
					// await so that this task will run in the background.
					await response;

					if (response.Result.StatusCode == HttpStatusCode.OK) {
						// Get json from response message.
						var result = response.Result.Content.ReadAsStringAsync ().Result;
						var json = JsonObject.Parse (result).ToString ();
						// Deserialize the Json.
						_placesFound = JsonConvert.DeserializeObject<GooglePlacesResponse> (json);

						if (_placesFound != null) {
							// Get a list of GooglePlace objects
							var googlePlaces = _placesFound.results.Select (o => 
								new GooglePlace {
									Name = o.name.Length > 35 ? o.name.Substring(0,35) : o.name,
									Latitude = double.Parse(o.geometry.location.lat),
									Longitude = double.Parse(o.geometry.location.lng),
									Image = Resource.Drawable.places
								}).ToList();

							//Create our adapter and populate with list of Google place objects.
							_adapter = new CustomListAdapter(this){
								CustomListItemID = Resource.Layout.customlistitem,
								CustomListItemImageID = Resource.Id.imageitem,
								CustomListItemLatitudeID = Resource.Id.latitude,
								CustomListItemLongitudeID = Resource.Id.longitude,
								CustomListItemNameID = Resource.Id.itemname,
								items = googlePlaces};

							// Add dialog with places found list.
							_alertDialog = _uiHelper.BuildAlertDialog (_adapter, true, true, Resource.Layout.NearbyPlaces, Resource.Layout.TextViewItem, this, "Places found near you", Resource.Drawable.places, Resource.Id.placeslist);
							_uiHelper.OnListViewItemClick += ListViewItemClick;

							// Add cancel button and event.
							_alertDialog.SetButton ("Cancel", (s, evt) => {
								PlacesDialog_OnCancelClick (s, evt);
							});

							//var nearbyPlacesLayout = (LinearLayout)_alertDialog.FindViewById (Resource.Id.placesDescription);
							var dialogDescription = (TextView)_uiHelper.DialogView.FindViewById (Resource.Id.placesDescription);
							dialogDescription.Click += AddPlaces_Click;
							//Enabled button again.
							BtnCheckin.Enabled = true;
							//Show diaog.
							_alertDialog.Show ();
						}
					}
				} else 
				{
					ShowNotificationBox ("An error occurred!");
				}
			}
			// Stop progress indicator.
			_spinner.Visibility = ViewStates.Gone;
		}
	}
}

