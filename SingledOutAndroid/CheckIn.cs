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

namespace SingledOutAndroid
{
	[Activity (Label = "Check-In")]//, Theme = "@android:style/Theme.NoTitleBar")]			
	public class CheckIn : BaseActivity
	{
		private Location _currentLocation;
		private LocationManager _locationManager;
		private ToggleButton _btnCheckin;
		private MapHelper _mapHelper;
		private RestHelper _restHelper;
		private UriCreator _googleApiUriCreator;
		private UIHelper _uiHelper;
		private AlertDialog _alertDialog;
		ProgressBar _spinner;
		private GooglePlacesResponse _placesFound;
		private CustomListAdapter _adapter;
		private UriCreator _uriCreator;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Instantiate the REST helper.
			_restHelper = new RestHelper();
			_uiHelper = new UIHelper ();
			_uriCreator = new UriCreator(Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));

			// Create uri creator for Google Api related stuff.
			_googleApiUriCreator = new UriCreator (Resources.GetString(Resource.String.googleapihost), Resources.GetString(Resource.String.googleapipath));

			// Set the action bar to show.
			IsActionBarVisible = true;

			SetContentView (Resource.Layout.CheckIn);

			// Find checkin button.
			_btnCheckin = (ToggleButton)FindViewById (Resource.Id.btnCheckin);
			_btnCheckin.Click += btnCheckin_OnClick;
			_btnCheckin.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.hide, 0, 0, 0);

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
							//dialogDescription.MovementMethod = LinkMovementMethod.Instance;
							dialogDescription.Click += AddPlaces_Click;
							//Enabled toggle button again.
							_btnCheckin.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.hide, 0, 0, 0);
							_btnCheckin.Enabled = true;
							//Show diaog.
							_alertDialog.Show ();
						}
					}
				} else {
					ShowNotificationBox ("An error occurred!");
				}
			}
			// Stop progress indicator.
			_spinner.Visibility = ViewStates.Gone;
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

			SaveUserLocation (googlePlace);
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
		/// Placeses the dialog_ on cancel click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void PlacesDialog_OnCancelClick(object sender, EventArgs eventArgs)
		{
			_alertDialog.Dismiss ();
		}

		protected void btnCheckin_OnClick(object sender, EventArgs eventArgs)
		{
			if (_btnCheckin.Checked) {
				// Start progress indicator.
				_spinner = (ProgressBar)FindViewById (Resource.Id.progressSpinner);
				_spinner.Visibility = ViewStates.Visible;

				_btnCheckin.SetCompoundDrawablesWithIntrinsicBounds (Resource.Drawable.show, 0, 0, 0);
				_btnCheckin.Enabled = false;
				// Start the location manager.
				_locationManager = _mapHelper.InitializeLocationManager (true, 2000, 10);
			} else {
				_mapHelper.RemoveMarker ();
			}
		}
	}
}

