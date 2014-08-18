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
using SingledOutAndroid.Adapters;
using Java.Net;
using Android.Database;
using Android.Graphics.Drawables;
using Java.IO;
using System.IO;

namespace SingledOutAndroid
{
	[Activity (Label = "Check-In")]
	public class CheckIn : BaseActivity
	{
		private LocationManager _locationManager;
		private Button _btnCheckin;
		private MapHelper _mapHelper;
		private UIHelper _uiHelper;
		private ActionBar.Tab _mapTab;
		private ActionBar.Tab _listViewTab;
		private ActionBar.Tab _individualTab;
		private ActionBar.Tab _profileTab;
		private Location _currentLocation;
		private UriCreator _googleApiUriCreator;
		private RestHelper _restHelper;
		private GooglePlacesResponse _placesFound;
		private CustomListAdapter _adapter;
		private GroupsListAdapter _groupsAdapter;
		private AlertDialog _alertDialog;
		private AlertDialog _groupsAlertDialog;
		private UriCreator _uriCreator;
		private RangeSliderView _ageSlider;
		private SeekBar _distanceSlider;
		private AnimationHelper _animationHelper;
		private ViewFlipper _viewFlipper;
		private bool isStartingUp = false;
		private RoundImageView _profilePhoto;

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

		private enum TabPosition
		{
			Map = 0,
			ListView = 1,
			Individual = 2
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

			_viewFlipper = (ViewFlipper)FindViewById (Resource.Id.viewFlipper);

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

			// Set the map marker click event.
			_mapHelper.Map.MarkerClick += MapMarkerClick;
		}

		/// <summary>
		/// Maps the marker click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void MapMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
		{
			if (_mapHelper.IsGroupMarker (CurrentUser.ID, e.P0)) {
				ShowMarkerGroups (e.P0);
			}
			else
			{
				var user = _mapHelper.GetUsersForMarker (CurrentUser.ID, e.P0).SingleOrDefault();

				if (user != null) {
					// Add individual tab.
					AddIndividalTabAndSelect (user);
				} else { // If it gets here it should be the logged on user.
					e.P0.ShowInfoWindow ();
				}
			}
		}

		/// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <returns>To be added.</returns>
		public override bool OnOptionsItemSelected(IMenuItem item) {
			var itemSelected = base.OnOptionsItemSelected(item);

			if (item.TitleFormatted != null) {
				switch (item.TitleFormatted.ToString ().ToLower ()) {
				case "profile":
					AddProfileTabAndSelect ();
					break;

				case "map":
					_mapTab.Select ();
					break;

				case "list view":
					itemSelected = true;
					_listViewTab.Select ();
					break;

				case "settings":

					break;

				case "help":

					break;

				case "about":

					break;
				}
			}
			return itemSelected;
		}

		/// <summary>
		/// Adds the individal tab and select.
		/// </summary>
		private void AddIndividalTabAndSelect(UserLocationsFlat user = null)
		{
			// Add individual tab.
			if (_individualTab == null) {
				_individualTab = _uiHelper.AddActionBarTab (this, Resource.String.individual, Resource.Drawable.individual);
			}
			_individualTab.SetTag (new JavaLangHolder<UserLocationsFlat>(user));

			// Select the tab.
			_individualTab.Select ();
		}

		/// <summary>
		/// Adds the profile tab and select.
		/// </summary>
		private void AddProfileTabAndSelect()
		{
			// Add profile tab.
			if (_profileTab == null) {
				_profileTab = _uiHelper.AddActionBarTab (this, Resource.String.profile, Resource.Drawable.individual);
			}
			// Select the tab.
			_profileTab.Select ();
		}

		/// <summary>
		/// Shows the users within the marker group.
		/// </summary>
		/// <param name="marker">Marker.</param>
		private void ShowMarkerGroups(Marker marker)
		{
			var users = _mapHelper.GetUsersForMarker (CurrentUser.ID, marker);

			//Create our adapter and populate with list of Google place objects.
			_groupsAdapter = new GroupsListAdapter(this){
				CustomListItemID = Resource.Layout.GroupUserItem,
				CustomListItemNameID = Resource.Id.itemname,
				CustomListItemPhoto = Resource.Id.userphoto,
				CustomListItemAgeID = Resource.Id.age,
				CustomListItemDistanceID = Resource.Id.distance,
				items = users};

			var placeName = users.First().PlaceName;

			// Add dialog with places found list.
			_groupsAlertDialog = null;
			_groupsAlertDialog = _uiHelper.BuildAlertDialog (_groupsAdapter, true, true, Resource.Layout.groupusers, Resource.Layout.TextViewItem, this, string.Format("Users at {0}", placeName), Resource.Drawable.individual, Resource.Id.groupsuserslist);

			// Add cancel button and event.
			_groupsAlertDialog.SetButton ("Cancel", (s, evt) => {
				GroupsDialog_OnCancelClick (s, evt);
			});

			// Add item click event.
			_uiHelper.OnListViewItemClick += GroupsListViewItemClick;

			// Add dialog closed event.
			_uiHelper.OnAlertDialogClosed += GroupsDialogClosed;

			// Show groups dialog.
			_groupsAlertDialog.Show ();
		}

		void GroupsDialogClosed (object sender, EventArgs e)
		{
			_uiHelper.OnListViewItemClick -= GroupsListViewItemClick;
			_uiHelper.OnAlertDialogClosed -= GroupsDialogClosed;
		}

		/// <summary>
		/// Groups the dialog_ on cancel click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void GroupsDialog_OnCancelClick(object sender, EventArgs eventArgs)
		{
			_groupsAlertDialog.Dismiss ();
		}

		/// <summary>
		/// List view item click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void GroupsListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			e.View.SetBackgroundColor (Color.AliceBlue);

			// Get the user from the selected tab.
			var user = _groupsAdapter.GetItemAtPosition (e.Position);

			// Add individual tab.
			AddIndividalTabAndSelect (user);

			// Close the dialog.
			_groupsAlertDialog.Dismiss ();
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
					int currentUserID = int.Parse (GetUserPreference ("UserID"));

					_mapHelper.SetOtherUserMarkers (this, userModelList, 16, currentUserLatitude, currentUserLongitude, currentUserID);

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

			var response = FactoryStartNew<HttpResponseMessage> (() => _restHelper.GetAsync (uri));
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
			var response = FactoryStartNew<HttpResponseMessage> (() => _restHelper.PostAsync (uri, userLocationModel));
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
					SetUserPreference ("UserPlaceName", returnnModel.PlaceName);
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

			// Get Uri to delete the users location from the database.
			var uri = _uriCreator.DeleteUserLocations (Resources.GetString (Resource.String.apiurluserlocations), Resources.GetString (Resource.String.apiurldeleteuserlocation),  userLocationID);

			HttpResponseMessage response = null;

			try
			{
				// Perform the database delete.
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
				PlaceName = googlePlace.Name,
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
			_mapHelper.SetUserMarker (this, googlePlace.Latitude, googlePlace.Longitude, googlePlace.Name, CurrentUser.ID); 

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
		/// Removes the dynamic tabs.
		/// </summary>
		private void RemoveDynamicTabs()
		{
			if (_individualTab != null) {
				ActionBar.RemoveTab (_individualTab);
				_individualTab = null;
			}
			if (_profileTab != null) {
				ActionBar.RemoveTab (_profileTab);
				_profileTab = null;
			}
		}

		/// <summary>
		/// Raises the tab selected event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void OnTabSelected (object sender, ActionBar.TabEventArgs e)
		{
			switch (((ActionBar.Tab)sender).Text.ToLower()) {
			case "map":
				_viewFlipper.DisplayedChild = 0;
				RemoveDynamicTabs ();
				break;
			case "list":
				_viewFlipper.DisplayedChild = 1;
				RemoveDynamicTabs ();
				break;
			case "user":
				_viewFlipper.DisplayedChild = 2;

				if (_profileTab != null) {
					ActionBar.RemoveTab (_profileTab);
				}

				var user = ((JavaLangHolder<UserLocationsFlat>)_individualTab.Tag).Value; 
				if (user != null) 
				{
					var individualName = (TextView)this.FindViewById (Resource.Id.individualName);
					if (individualName != null) {
						individualName.SetText (string.Concat (user.FirstName, " ", user.Surname.Substring (0, 1)), TextView.BufferType.Normal);							
					}
					var individualAge = (TextView)this.FindViewById (Resource.Id.individualAge);
					if (individualAge != null) {
						individualAge.SetText (user.Age.ToString (), TextView.BufferType.Normal);							
					}
					var individualGender = (TextView)this.FindViewById (Resource.Id.individualGender);
					if (individualGender != null) {
						individualGender.SetText (user.Sex, TextView.BufferType.Normal);							
					}
					var individualDistance = (TextView)this.FindViewById (Resource.Id.individualDistance);
					if (individualDistance != null) {
						individualDistance.SetText ("200m", TextView.BufferType.Normal);							
					}
					var individualInterests = (TextView)this.FindViewById (Resource.Id.individualInterests);
					if (individualInterests != null) {
						individualInterests.SetText (user.Interests, TextView.BufferType.Normal);							
					}
					var individualPhoto = (RoundImageView)this.FindViewById (Resource.Id.individualPhoto);
					if (individualPhoto != null) {
						if (!string.IsNullOrEmpty (user.ProfilePicture)) {
							var task = FactoryStartNew<Bitmap> (() => GetImageFromUrl (user.ProfilePicture));
							if (task != null) {
								// await so that this task will run in the background.
								await task;
								individualPhoto.SetImageBitmap (task.Result);
							}
						} else {
							individualPhoto.SetImageResource (Resource.Drawable.blankperson);
						}
						individualPhoto.BringToFront ();
					}
				}
				break;

			case "profile": 
				var btnSaveProfile = (Button)this.FindViewById (Resource.Id.btnSaveProfile);
				btnSaveProfile.Click += OnProfileSaveClick;

				_viewFlipper.DisplayedChild = 3;

				if (_individualTab != null) {
					ActionBar.RemoveTab (_individualTab);
				}

					if (CurrentUser != null) 
					{
						var isFacebookUser = !string.IsNullOrEmpty(CurrentUser.FacebookUserName);
						isFacebookUser = false;

						var profilePhotoEdit = (ImageView)this.FindViewById (Resource.Id.profileEditPhoto);
						if (profilePhotoEdit != null && !isFacebookUser) {
							profilePhotoEdit.Visibility = ViewStates.Visible;
							profilePhotoEdit.Click += ProfilePhotoEditOnClick;
						}
						var profileName = (TextView)this.FindViewById (Resource.Id.profileName);
						if (profileName != null) {
							profileName.SetText (string.Concat (CurrentUser.FirstName, " ", CurrentUser.Surname), TextView.BufferType.Normal);							
						}
						var profileNameEdit = (ImageView)this.FindViewById (Resource.Id.profileEditName);
						if (profileNameEdit != null && !isFacebookUser) {
							profileNameEdit.Visibility = ViewStates.Visible;
						}
						var profileAge = (TextView)this.FindViewById (Resource.Id.profileAge);
						if (profileAge != null) {
							profileAge.SetText (CurrentUser.Age.ToString (), TextView.BufferType.Normal);							
						}
						var profileEditAge = (ImageView)this.FindViewById (Resource.Id.profileEditAge);
						if (profileEditAge != null && !isFacebookUser) {
							profileEditAge.Visibility = ViewStates.Visible;
						}
						var profileGender = (TextView)this.FindViewById (Resource.Id.profileGender);
						if (profileGender != null) {
							profileGender.SetText (CurrentUser.Sex, TextView.BufferType.Normal);							
						}
						var profileEditGender = (ImageView)this.FindViewById (Resource.Id.profileEditGender);
						if (profileEditGender != null && !isFacebookUser) {
							profileEditGender.Visibility = ViewStates.Visible;
						}
						var profileInterests = (TextView)this.FindViewById (Resource.Id.profileInterests);
						if (profileInterests != null) {
							profileInterests.SetText (CurrentUser.Interests, TextView.BufferType.Normal);							
						}	
						var profileInterestsEdit = (ImageView)this.FindViewById (Resource.Id.profileInterestsEdit);
						if (profileInterestsEdit != null && !isFacebookUser) {
							profileInterestsEdit.Visibility = ViewStates.Visible;
						}
						_profilePhoto = (RoundImageView)this.FindViewById (Resource.Id.profilePhoto);
						if (_profilePhoto != null) {
							if (!string.IsNullOrEmpty (CurrentUser.FacebookPhotoUrl)) {
								var task = FactoryStartNew<Bitmap> (() => GetImageFromUrl (CurrentUser.FacebookPhotoUrl));

								if (task != null) {
									// await so that this task will run in the background.
									await task;
									_profilePhoto.SetImageBitmap (task.Result);
								}
							} else {
								_profilePhoto.SetImageResource (Resource.Drawable.blankperson);
							}
							_profilePhoto.BringToFront ();
						}
					}
					break;					
			}
		}

		/// <summary>
		/// Raises the profile save click event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected async void OnProfileSaveClick(object sender, EventArgs e)
		{
			// Get the current user.
			var user = CurrentUser;
			var isDirty = false;

			// Get the profile picture currently selected.
			var profilePhoto = (RoundImageView)this.FindViewById (Resource.Id.profilePhoto);
			profilePhoto.BuildDrawingCache(true);
			Bitmap bitmap = profilePhoto.GetDrawingCache(true);  
			var bos = new MemoryStream();  
			bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg,100,bos); 
			byte[] byteArray = bos.ToArray(); 
			profilePhoto.DrawingCacheEnabled = false;

			// Perform sequential comparison and if different set profile picture to new one.
			if (user.ProfilePicture == null || user.ProfilePicture.SequenceEqual (byteArray)) {
				user.ProfilePicture = byteArray;
				isDirty = true;
			}

			if (isDirty) {
				// Start progress indicator.
				_uiHelper.DisplayProgressDialog (this, Resource.Style.CustomDialogTheme, "Saving user profile", "Please wait ...");

				try {
					// Create task to login to Singled Out.
					var task = FactoryStartNew<HttpResponseMessage> (() => UpdateProfile (user));
					if (task != null) {
						// await so that this task will run in the background.
						await task;

						// Return here after login has completed.
						if (task.Result.StatusCode == HttpStatusCode.OK) {
							// Get json from response message.
							var result = task.Result.Content.ReadAsStringAsync ().Result;
							var json = JsonObject.Parse (result).ToString ().Replace ("{{", "{").Replace ("}}", "}");
							// Deserialize the Json.
							var returnUserModel = JsonConvert.DeserializeObject<UserModel> (json);

							// Save the updated user to the preference.
							SetUserPreference ("SingledOutUser", json);
						}
					} else if (task.Result.StatusCode == HttpStatusCode.Unauthorized) {
						ShowNotificationBox ("Profile was not updated.");
					}
				} catch (Exception ex) {
					ShowNotificationBox (GetString (Resource.String.exceptionUnknown));
				}

				_uiHelper.HideProgressDialog ();
			}
		}

		/// <summary>
		/// Updates the profile.
		/// </summary>
		/// <returns>The profile.</returns>
		/// <param name="user">User.</param>
		private HttpResponseMessage UpdateProfile(UserModel user)
		{
			var uriCreator = new UriCreator (Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));
			var uri = uriCreator.User (Resources.GetString (Resource.String.apiurlusers));
			var response = _restHelper.PutAsync (uri, user);
			return response;
		}

		public static readonly int PickImageId = 1000;
		/// <summary>
		/// Profiles the photo edit on click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void ProfilePhotoEditOnClick(object sender, EventArgs e)
		{
			Intent = new Intent();
			Intent.SetType("image/*");
			Intent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(Intent, "Select Profile Picture"), PickImageId);
		}

		/// <param name="requestCode">The integer request code originally supplied to
		///  startActivityForResult(), allowing you to identify who this
		///  result came from.</param>
		/// <param name="resultCode">The integer result code returned by the child activity
		///  through its setResult().</param>
		/// <param name="data">An Intent, which can return result data to the caller
		///  (various data can be attached to Intent "extras").</param>
		/// <summary>
		/// Called when an activity you launched exits, giving you the requestCode
		///  you started it with, the resultCode it returned, and any additional
		///  data from it.
		/// </summary>
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
			{
				Android.Net.Uri uri = data.Data;
				_profilePhoto.SetImageURI(uri);			
			}
		}

		/// <summary>
		/// Gets the image from URL.
		/// </summary>
		/// <returns>The image from URL.</returns>
		/// <param name="url">URL.</param>
		private Bitmap GetImageFromUrl(string url)
		{
			using(var client = new HttpClient())
			{
				var msg = client.GetAsync(url);
				if (msg.Result.IsSuccessStatusCode)
				{
					using(var stream = msg.Result.Content.ReadAsStreamAsync())
					{
						﻿var bitmap = BitmapFactory.DecodeStreamAsync(stream.Result);
						return bitmap.Result;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Checkin on click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void btnCheckin_OnClick(object sender, EventArgs eventArgs)
		{
			if (!_mapHelper.UserHasMarker(CurrentUser.ID)) {
				// Start progress indicator.
				_uiHelper.DisplayProgressDialog (this, Resource.Style.CustomDialogTheme, "Finding places near you", "Please wait ...");

				_btnCheckin.Enabled = false;
				// Start the location manager.
				_locationManager = _mapHelper.InitializeLocationManager (true, 2000, 10);
			} 
			else {
				_mapHelper.RemoveMarker (this, CurrentUser.ID);

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
				var response = FactoryStartNew<HttpResponseMessage> (() => _restHelper.GetAsync (uri.ToString()));
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
			// Hide progress dialog.
			_uiHelper.HideProgressDialog ();
		}

		void PlacesDialogClosed (object sender, EventArgs e)
		{
			_uiHelper.OnListViewItemClick -= ListViewItemClick;
			_uiHelper.OnAlertDialogClosed -= PlacesDialogClosed;
		}
	}

	public class JavaLangHolder<T> : Java.Lang.Object { 
		public readonly T Value; 

		public JavaLangHolder (T value) 
		{ 
			this.Value = value; 
		} 
	} 

}

