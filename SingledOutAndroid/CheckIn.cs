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
using Android.Gms.Maps;
using Android.Support.V4.View;
using Android.Gms.Maps.Model;
using Android.Graphics;
using SingledOut.SearchParameters;
using System.Threading.Tasks;

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
		private SeekBar _distanceSlider;
		private AnimationHelper _animationHelper;
		private ViewSwitcher _viewSwitcher;
		private bool isStartingUp = false;

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
			// Set this variable (used to stop the control events from firing the 
			// users search during page loading.
			isStartingUp = true;

			// Set the action bar to show.
			ShowActionBarTabs = true;
			IsActionBarVisible = true;

			SetContentView (Resource.Layout.CheckIn);

			_uriCreator = new UriCreator(Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));

			_distanceSlider = (SeekBar)FindViewById (Resource.Id.distanceslider);

			// Instantiate the helpers.
			_uiHelper = new UIHelper ();
			_mapHelper = new MapHelper (this);
			_restHelper = new RestHelper(AuthenticationToken, UserID);
			_animationHelper = new AnimationHelper ();

			// Create uri creator for Google Api related stuff.
			_googleApiUriCreator = new UriCreator (Resources.GetString(Resource.String.googleapihost), Resources.GetString(Resource.String.googleapipath));
			// Set tab selected event handler.
			_uiHelper.OnTabSelectedClick += OnTabSelected;
			// Add map tab.
			_mapTab = _uiHelper.AddActionBarTab (this, Resource.String.maptabname, Resource.Drawable.globe);
			// Add listview tab.
			_listViewTab = _uiHelper.AddActionBarTab (this, Resource.String.listtabname, Resource.Drawable.listview);

			// Get the map fragment.
			MapFragment mapfragment = (MapFragment)this.FragmentManager.FindFragmentById(Resource.Id.map);

			// Create map helper.
			_mapHelper = new MapHelper (this);

			// Show the map.
			_mapHelper.ShowMap (mapfragment, true, true);

			_viewSwitcher = (ViewSwitcher)FindViewById (Resource.Id.viewSwitcher);

			Button btnFilter = (Button)FindViewById (Resource.Id.btnFilter);
			btnFilter.Click += FilterClick;

			// Set the location updated event.
			_mapHelper.OnLocationUpdated += LocationUpdated;

			// Find checkin button.
			_btnCheckin = (Button)FindViewById (Resource.Id.btnCheckin);
			_btnCheckin.Click += btnCheckin_OnClick;

			// Show welcome back message.
			if (LastActivity == "Login" || LastActivity == "SplashPage") {
				ShowNotificationBox (string.Concat ("Welcome back ", CurrentUser.FirstName, "!"),true);
			}

			// Set the age slider up.
			_ageSlider = (RangeSliderView)FindViewById (Resource.Id.ageslider);
			_ageSlider.LeftValueChanged += values => {
				SetAgeRangeText();
			};

			_ageSlider.RightValueChanged += value => {
				SetAgeRangeText();
			};

			SetAgeRangeText();

			_distanceSlider.ProgressChanged += SetDistanceRangeText;

			isStartingUp = false;

			// Now call the method to get the other users that are around.
			DisplayOtherUsers ();

			var btnApply = (Button)FindViewById (Resource.Id.btnApply);
			btnApply.Click += BtnApplyClick;
		}

		protected void BtnApplyClick(object sender, EventArgs e)
		{
			CloseSlidingDrawer ();
			DisplayOtherUsers ();
		}

		/// <summary>
		/// Displays the other users.
		/// </summary>
		private async void DisplayOtherUsers()
		{
			if (!isStartingUp) {
				_uiHelper.DisplayProgressDialog (this, Resource.Style.CustomDialogTheme, "Finding other users", "Please wait ...");

				var task = GetOtherUsers ();

				if (task != null) {
					// await so that this task will run in the background.
					await task;

					var userModelList = task.Result;

					double? currentUserLatitude = !string.IsNullOrEmpty(GetUserPreference ("CurrentUserLatitude")) ? double.Parse(GetUserPreference ("CurrentUserLatitude")) : (double?)null;
					double? currentUserLongitude = !string.IsNullOrEmpty(GetUserPreference ("CurrentUserLongitude")) ? double.Parse(GetUserPreference ("CurrentUserLongitude")) : (double?)null;

					_mapHelper.SetOtherUserMarkers (userModelList, 16, currentUserLatitude, currentUserLongitude);

				}
				_uiHelper.HideProgressDialog ();
			}
		}

		/// <summary>
		/// Gets the other users.
		/// </summary>
		private async Task<List<UserModel>> GetOtherUsers()
		{
			List<UserModel> userModelList = null;

			var rgGender = (RadioGroup)FindViewById (Resource.Id.rgGender);
			var gender = GenderEnum.Both;

			switch (rgGender.CheckedRadioButtonId) {
			case Resource.Id.rbMen:
				gender = GenderEnum.Male;
				break;
			case Resource.Id.rbWomen:
				gender = GenderEnum.Female;
				break;
			default:
				gender = GenderEnum.Both;
				break;
			}

			UsersSearchParameters sp = new UsersSearchParameters {
				AgeFrom = (int)_ageSlider.LeftValue,
				AgeTo = (int)_ageSlider.RightValue,
				Distance = _distanceSlider.Progress,
				Sex = gender
			};

			// Create task to get other users.
			var userSearchControllerUri = Resources.GetString (Resource.String.apiurlusersearch);
			var uri = _uriCreator.Search (userSearchControllerUri, sp);

			var response = FactoryStartNew (() => _restHelper.GetAsync (uri));
			if (response != null) {
				// await so that this task will run in the background.
				await response;

				if (response.Result.StatusCode == HttpStatusCode.OK) 
				{
					var result = response.Result.Content.ReadAsStringAsync ().Result;
					if (result != "[]") {
						var json = JsonObject.Parse (result).ToString ();
						// Deserialize the Json.
						userModelList = JsonConvert.DeserializeObject<List<UserModel>> (result);		
					}
				} 
			}

			return userModelList;
		}

		/// <summary>
		/// Filters the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void FilterClick(object sender, EventArgs e)
		{
			CloseSlidingDrawer ();
		}

		/// <summary>
		/// Closes the sliding drawer.
		/// </summary>
		private void CloseSlidingDrawer()
		{
			var slidingDrawer = (SlidingDrawer)FindViewById (Resource.Id.slidingDrawer);
			if (slidingDrawer.IsOpened) {
				slidingDrawer.AnimateClose();

				_mapHelper.SetAllGesturesEnabled(true);
			} else {
				slidingDrawer.AnimateOpen();
				_mapHelper.SetAllGesturesEnabled(false);
			}
		}

		/// <summary>
		/// Sets the age range text.
		/// </summary>
		private void SetAgeRangeText()
		{
			var agetosee = (TextView)FindViewById (Resource.Id.agetosee);
			agetosee.Text = String.Format ("Age {0} to {1}", (int)_ageSlider.LeftValue, (int)_ageSlider.RightValue);
		}

		/// <summary>
		/// Sets the distance range text.
		/// </summary>
		private void SetDistanceRangeText(object sender, SeekBar.ProgressChangedEventArgs e)
		{
			var distancetosee = (TextView)FindViewById (Resource.Id.distancetosee);
			distancetosee.Text = String.Format ("Within {0}M", e.Progress);
		}

		/// <summary>
		/// Saves the user location to the database.
		/// </summary>
		/// <param name="googlePlace">Google place.</param>
		private async void SaveUserLocation(GooglePlace googlePlace)
		{
			// Save the users location to the database.
			var uri = _uriCreator.UserLocations (Resources.GetString (Resource.String.apiurluserlocations));
			var userLocationModel = CreateUserLocation (googlePlace);

			// Create task to Save Singled Out Details.
			var response = FactoryStartNew (() => _restHelper.PostAsync (uri, userLocationModel));
			if (response != null) {
				// await so that this task will run in the background.
				await response;

				if (response.Result.StatusCode == HttpStatusCode.Created) {
					// Get json from response message.
					var result = response.Result.Content.ReadAsStringAsync ().Result;
					var json = JsonObject.Parse (result).ToString ();
					// Deserialize the Json.
					var returnnModel = JsonConvert.DeserializeObject<UserLocationModel> (json);

					SetUserPreference ("UserLocationID", returnnModel.ID.ToString());
					SetUserPreference ("UserLatitude", returnnModel.Latitude.ToString());
					SetUserPreference ("UserLongitude", returnnModel.Longitude.ToString());
				}
			}
		}

		/// <summary>
		/// Removes the user location.
		/// </summary>
		private void RemoveUserLocation()
		{
			// Get the saved user location Id.
			var userLocationID = GetUserPreference ("UserLocationID");

			// Save the users location to the database.
			var uri = _uriCreator.DeleteUserLocations (Resources.GetString (Resource.String.apiurluserlocations), Resources.GetString (Resource.String.apiurldeleteuserlocation),  userLocationID);

			// Perform the database delete.
			HttpResponseMessage response = null;

			try
			{
				response = _restHelper.DeleteAsync (uri);

				if (!response.IsSuccessStatusCode) {
					ShowNotificationBox ("An error occurred!");
				}
			}
			catch(Exception ex) {
				ShowNotificationBox ("An error occurred!");
				// Log error.
			}

			if (response.StatusCode == HttpStatusCode.OK) {
				SetUserPreference ("UserLocationID", string.Empty);
			}
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
			_mapHelper.SetUserMarker (googlePlace.Latitude, googlePlace.Longitude, 16, "You are here!", Resource.Drawable.usermarker, true); 

			SetUserPreference ("CurrentUserLatitude", googlePlace.Latitude.ToString ());
			SetUserPreference ("CurrentUserLongitude", googlePlace.Longitude.ToString ());

			// Set checkin button to 'Hide me'
			BtnCheckin.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.hide,0, 0, 0);
			BtnCheckin.Text = "Hide Me!";

			// Save the location to the database.
			SaveUserLocation (googlePlace);
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
					_viewSwitcher.DisplayedChild = 0;

					break;
				case ((int)TabPosition.ListView):
					_viewSwitcher.DisplayedChild = 1;
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

				// Remove the user location from the database.
				RemoveUserLocation ();

				// Set the name and image on the checkin button.
				_btnCheckin.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.hide, 0, 0, 0);
				_btnCheckin.Text = "Join in";
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

				// Create task to get Google Places.
				var restHelper = new RestHelper();
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
							_alertDialog = null;
							_alertDialog = _uiHelper.BuildAlertDialog (_adapter, true, true, Resource.Layout.NearbyPlaces, Resource.Layout.TextViewItem, this, "Places found near you", Resource.Drawable.places, Resource.Id.placeslist);

							_uiHelper.OnListViewItemClick += ListViewItemClick;

							_uiHelper.OnAlertDialogClosed += PlacesDialogClosed;

							// Add cancel button and event.
							_alertDialog.SetButton ("Cancel", (s, evt) => {
								PlacesDialog_OnCancelClick (s, evt);
							});

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

		void PlacesDialogClosed (object sender, EventArgs e)
		{
			_uiHelper.OnListViewItemClick -= ListViewItemClick;
			_uiHelper.OnAlertDialogClosed -= PlacesDialogClosed;
		}
	}
}

