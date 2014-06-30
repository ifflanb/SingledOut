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

namespace SingledOutAndroid
{
	[Activity (Label = "Check-In")]//, Theme = "@android:style/Theme.NoTitleBar")]			
	public class CheckIn : BaseActivity
	{
		private Location _currentLocation;
		private LocationManager _locationManager;
		private Button _btnCheckin;
		private MapHelper _mapHelper;
		private RestHelper _restHelper;
		private UriCreator _googleApiUriCreator;
		private UIHelper _uiHelper;
		ProgressBar _spinner;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Instantiate the REST helper.
			_restHelper = new RestHelper();
			_uiHelper = new UIHelper ();

			// Create uri creator for Google Api related stuff.
			_googleApiUriCreator = new UriCreator (Resources.GetString(Resource.String.googleapihost), Resources.GetString(Resource.String.googleapipath));

			// Set the action bar to show.
			IsActionBarVisible = true;

			SetContentView (Resource.Layout.CheckIn);

			// Find checkin button.
			_btnCheckin = (Button)FindViewById (Resource.Id.btnCheckin);
			_btnCheckin.Click += btnCheckin_OnClick;

			// Set swipe activity.
			SwipeRightActivity = typeof(Tutorial2);

			// Show welcome back message.
			if (LastActivity == "Login" || LastActivity == "SplashPage") {
				ShowNotificationBox (string.Concat ("Welcome back ", CurrentUser.FirstName, "!"));
			}

			// Create map helper.
			_mapHelper = new MapHelper (this);
			// Set the location updated event.
			_mapHelper.OnLocationUpdated += LocationUpdated;
			// Show the map.
			_mapHelper.ShowMap (Resource.Id.map, true, true);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is marker set.
		/// </summary>
		/// <value><c>true</c> if this instance is marker set; otherwise, <c>false</c>.</value>
		public bool IsMarkerSet { get; set;	}

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
				// Start progress indicator.
				_spinner = (ProgressBar)FindViewById(Resource.Id.progressSpinner);
				_spinner.Visibility = ViewStates.Visible;

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
						var json = JsonObject.Parse (result).ToString ();//.Replace ("{{", "{").Replace ("}}", "}");
						// Deserialize the Json.
						var returnPlacesModel = JsonConvert.DeserializeObject<GooglePlacesResponse> (json);

						if (returnPlacesModel != null) {
							var placesList = returnPlacesModel.results.Select(o => o.name).ToList();
							var dialog = _uiHelper.BuildAlertDialog (Resource.Layout.NearbyPlaces, Resource.Layout.TextViewItem, this, "Places found near you", Resource.Drawable.places, Resource.Id.placeslist, placesList);
							_uiHelper.OnListViewItemClick += ListViewItemClick;

							dialog.SetButton ("OK", (s, evt) => {
								PlacesDialog_OnOkClick (s, evt);
							});
							dialog.SetButton2 ("Cancel", (s, evt) => {
								PlacesDialog_OnCancelClick (s, evt);
							});
								
							dialog.Show ();
						}

					}
					if (!IsMarkerSet) {
						_mapHelper.SetMarker (_currentLocation.Latitude, _currentLocation.Longitude, 16, "You are here!", Resource.Drawable.logopindialog, true); 
					}
				} else {
					ShowNotificationBox ("An error occurred!");
				}
			}
			// Stop progress indicator.
			_spinner.Visibility = ViewStates.Gone;
		}

		protected void ListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var text = ((ListView)sender).Adapter.GetItem(e.Position).ToString();
			Toast.MakeText(this, text, ToastLength.Long);
		}

		protected void PlacesDialog_OnOkClick(object sender, EventArgs eventArgs)
		{

		}

		protected void PlacesDialog_OnCancelClick(object sender, EventArgs eventArgs)
		{

		}

		protected void btnCheckin_OnClick(object sender, EventArgs eventArgs)
		{
			// Start the location manager.
			_locationManager = _mapHelper.InitializeLocationManager (true, 2000, 10);
		}
	}
}

