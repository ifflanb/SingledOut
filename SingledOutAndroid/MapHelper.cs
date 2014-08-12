using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Collections.Generic;
using Android.Runtime;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Graphics;
using Android.Views.Animations;
using Java.Lang;
using SingledOut.Model;
using Java.Util;
using MobileSpace.Helpers;
using Java.Net;

namespace SingledOutAndroid
{
	public class MapHelper : Activity,  Android.Locations.ILocationListener
	{
		LocationManager _locationManager;
		Activity _activity;
		private GoogleMap _map;
		private const int _userZoomLevel = 16;
		private const string _userMarkerTitle = "You are at {0}";

		public delegate void LocationUpdated(object sender, LocationUpdatedEventArgs e);
		public event LocationUpdated OnLocationUpdated;

		private List<Marker> MapMarkers { get; set; }

		/// <summary>
		/// Whether the given on user has a marker.
		/// </summary>
		/// <returns><c>true</c>, if has marker was usered, <c>false</c> otherwise.</returns>
		/// <param name="userID">User I.</param>
		public bool UserHasMarker (int userID)
		{
			return MapUserData.Any (o => o.UserID == userID && o.MapMarker != null); 
		}

		/// <summary>
		/// Gets or sets the map user data.
		/// </summary>
		/// <value>The map user data.</value>
		public List<UserLocationsFlat> MapUserData { get; set; }

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public GoogleMap Map { 
			get {
				return _map;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}

		public MapHelper (Activity activity)
		{
			_activity = activity;
		}

		/// <summary>
		/// Removes the marker.
		/// </summary>
		public void RemoveMarker(Context context, int userID)
		{
			if (UserHasMarker(userID)) 
			{
				// Get the logged in users marker.
				var marker = MapUserData.SingleOrDefault (o => o.UserID == userID).MapMarker;
				if (marker != null) {

					// Get the marker coordinates.
					var latitude = System.Math.Round(marker.Position.Latitude, 6);
					var longitude = System.Math.Round(marker.Position.Longitude, 6);

					// First find out if there is more than one person at this location.
					var users = MapUserData
						.Where (o => o.Latitude == latitude && o.Longitude == longitude)
						.Select (o => o)
						.ToList ();

					if (users.Any ()) {
						var markerOptions = new MarkerOptions ();
						var latLng = new LatLng (latitude, longitude);
						markerOptions.SetPosition (latLng);
						var countUsersInLocation = (users.Count - 1).ToString ();
						var user = users.Where (o => o.UserID != userID).Select(o => o).ToList();

						if (user != null) {
							var place = Truncate (user.First ().PlaceName, 10);
							if (int.Parse (countUsersInLocation) > 1) {
								markerOptions.SetTitle (string.Format ("{0} people are at {1}", countUsersInLocation, place));
							} else {
								markerOptions.SetTitle (string.Format ("{0} {1} is at {2}", user.First().FirstName, user.First().Surname.Substring (0, 1), place));
							}
						} else {
							markerOptions.SetTitle (string.Format ("unknown user is here"));
						}

						// Set the marker title.
						var markerIconID = GetMarkerIcon (false, latitude, longitude);
						if (int.Parse (countUsersInLocation) > 1) {
							markerOptions.InvokeIcon (BitmapDescriptorFactory.FromBitmap (WriteTextOnDrawable (context, markerIconID, countUsersInLocation)));
						} else {
							markerOptions.InvokeIcon (BitmapDescriptorFactory.FromResource (markerIconID));
						}

						// Set marker to null for logged in user
						MapUserData.SingleOrDefault (o => o.UserID == userID).MapMarker = null;
						// Remove actual map marker.
						marker.Remove ();
						// Add new marker.
						marker = _map.AddMarker (markerOptions);
					} else {
						// Remove actual map marker.
						marker.Remove ();
						MapUserData.SingleOrDefault (o => o.UserID == userID).MapMarker = null;
					}
				}				
			}
		}

		/// <summary>
		/// Removes the marker.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public void RemoveMarker(double latitude, double longitude)
		{
			var markers = MapUserData.Where (o => o.Latitude == latitude && o.Longitude == longitude)
				.Select(o => o.MapMarker)
				.ToList();

			if (markers != null) {
				foreach (var m in markers) {
					if (m != null) {
						m.Remove ();
					}
				}
			}
		}

		/// <summary>
		/// Stops the location listener.
		/// </summary>
		public void StopLocationListener()
		{
			_map.SetOnMyLocationChangeListener (null);
		}

		/// <summary>
		/// Sets all gestures enabled.
		/// </summary>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public void SetAllGesturesEnabled(bool enabled)
		{
			_map.UiSettings.SetAllGesturesEnabled (enabled);
		}

		/// <summary>
		/// Shows the map.
		/// </summary>
		public void ShowMap(MapFragment mapFragment, bool zoomControls, bool compass)
		{
			_map = mapFragment.Map;		

			if (_map != null) {
				// The GoogleMap object is ready to go.
				if (zoomControls) {
					_map.UiSettings.ZoomControlsEnabled = true;
				}
				if (compass) {
					_map.UiSettings.CompassEnabled = true;
				}
			}
		}

		/// <summary>
		/// Initializes the location manager.
		/// </summary>
		public LocationManager InitializeLocationManager(bool requestLocationUpdates, long updatesTime, float updatesWithinDistance)
		{
			// Get the location service.
			_locationManager = (LocationManager)_activity.GetSystemService("location");

			var myCriteria = new Criteria();
			myCriteria.Accuracy = Accuracy.Medium; // Set this to medium to use non GPS provider.
			myCriteria.PowerRequirement = Power.Low; // Set this to low to use non GPS provider.
			// let Android select the right location provider for you
			System.String locationProvider = _locationManager.GetBestProvider(myCriteria, true);

			// Request location updates.
			_locationManager.RequestLocationUpdates(locationProvider, updatesTime, updatesWithinDistance, this);
			return _locationManager;
		}

		/// <summary>
		/// Converts to pixels.
		/// </summary>
		/// <returns>The to pixels.</returns>
		/// <param name="context">Context.</param>
		/// <param name="nDP">N D.</param>
		public int ConvertToPixels(Context context, int nDP)
		{
			double conversionScale = context.Resources.DisplayMetrics.Density;
			return (int) ((nDP * conversionScale) + 0.5f) ;
		}

		/// <summary>
		/// Writes the text on drawable.
		/// </summary>
		/// <returns>The text on drawable.</returns>
		/// <param name="drawableId">Drawable identifier.</param>
		/// <param name="text">Text.</param>
		public Bitmap WriteTextOnDrawable(Context context, int drawableId, System.String text) {

			Bitmap bm = BitmapFactory.DecodeResource(context.Resources, drawableId)
				.Copy(Bitmap.Config.Argb8888, true);

			Typeface tf = Typeface.Create("Helvetica", TypefaceStyle.Bold);

			Paint paint = new Paint();
			paint.SetStyle(Android.Graphics.Paint.Style.Fill);
			paint.Color = Color.Black;
			paint.SetTypeface(tf);
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = ConvertToPixels(context, 16);

			Rect textRect = new Rect();
			paint.GetTextBounds(text, 0, text.Length, textRect);

			Canvas canvas = new Canvas(bm);

			//If the text is bigger than the canvas , reduce the font size
			if (textRect.Width() >= (canvas.Width - 4)) {
				//the padding on either sides is considered as 4, so as to appropriately fit in the text
				paint.TextSize = ConvertToPixels (context, 7);        //Scaling needs to be used for different dpi's
			}

			//Calculate the positions
			int xPos = (canvas.Width / 2) + 2 ;     //-2 is for regulating the x position offset

			//"- ((paint.descent() + paint.ascent()) / 2)" is the distance from the baseline to the center.
			int yPos = (int)((canvas.Height / 2)) - 8;// - ((paint.Descent() + paint.Ascent()) / 2)) ;  

			canvas.DrawText(text, xPos, yPos, paint);

			return  bm;
		}

		/// <summary>
		/// Adds the map marker to the map marker store.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		/// <param name="marker">Marker.</param>
		private void AddMapMarker(double latitude, double longitude, Marker marker, int? userID)
		{
			if (userID.HasValue) {
				var loggedInUser = MapUserData.SingleOrDefault (o => o.UserID == userID);
				if (loggedInUser != null) {
					loggedInUser.Latitude = latitude;
					loggedInUser.Longitude = longitude;
				}
			}
			var markers = MapUserData.Where (o => o.Latitude == latitude && o.Longitude == longitude).ToList ();

			foreach (var m in markers) {
				if (m.MapMarker == null) {
					m.MapMarker = marker;
				}
			}
		}

		/// <summary>
		/// Adds the user map marker.
		/// </summary>
		/// <param name="userID">User I.</param>
		/// <param name="marker">Marker.</param>
		private void AddUserMapMarker(int userID, Marker marker)
		{
			var userMarker = MapUserData.SingleOrDefault (o => o.UserID == userID);

			if(userMarker != null)
			{
				userMarker.MapMarker = marker;
			}
		}

		/// <summary>
		/// Truncate the specified str and truncateTo.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="truncateTo">Truncate to.</param>
		private string Truncate(string str, int truncateTo)
		{
			var truncatedStr = str;

			if (str.Length > truncateTo) {
				truncatedStr = string.Concat(str.Substring (0, truncateTo), "...");
			}
			return truncatedStr;
		}

		/// <summary>
		/// Sets the other user markers.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="users">Users.</param>
		/// <param name="zoomLevel">Zoom level.</param>
		/// <param name="currentUserLatitude">Current user latitude.</param>
		/// <param name="currentUserLongitude">Current user longitude.</param>
		public void SetOtherUserMarkers(
			Context context,
			List<UserModel> users,			 
			int? zoomLevel,
			double? currentUserLatitude,
			double? currentUserLongitude,
			int currentUserID
			)
		{
			// Reset the map to clear markers.
			_map.Clear ();

			var builder = new LatLngBounds.Builder ();

			// Check if there are other users.
			if (users != null) {
				// Store the user data in a public property.
				MapUserData = (from c in users
				               select new UserLocationsFlat {
					IsLoggedOnUser = c.ID == currentUserID ? true : false,
					FirstName = c.FirstName,
					Surname = c.Surname,
					UserID = c.ID,
					Age = c.Age,
					Sex = c.Sex,
					ProfilePicture = Android.Net.Uri.Decode(c.FacebookPhotoUrl),
					Latitude = c.UserLocation != null ? c.UserLocation.Latitude : (double?)null,
					Longitude = c.UserLocation != null ? c.UserLocation.Longitude : (double?)null,
					PlaceName = c.UserLocation != null ? (!string.IsNullOrEmpty(c.UserLocation.PlaceName) ? Truncate(c.UserLocation.PlaceName, 10) : "an unknown place")  : "an unknown place"
				}).ToList ();

				var allUsersExceptLoggedInUser = (from o in MapUserData where o.UserID != currentUserID select o);

				// Group the users by lat long.
				var groupedUserLocations = (from t in allUsersExceptLoggedInUser
				group t by new {t.Latitude, t.Longitude, t.PlaceName}
					into grp
					select new
					{						
						grp.Key.Latitude,
						grp.Key.Longitude,
						grp.Key.PlaceName,
						CountOf = grp.Count()
					}).ToList(); 

				// First add markers for the ones that have more that 1 person in the same location.
				foreach (var grpUsr in groupedUserLocations.Where(o => o.CountOf > 1)) {
					var markerOptions = new MarkerOptions ();
					var latLng = new LatLng ((double)grpUsr.Latitude, (double)grpUsr.Longitude);
					markerOptions.SetPosition (latLng);
					markerOptions.SetTitle (string.Format("{0} people are at {1}", grpUsr.CountOf, Truncate(grpUsr.PlaceName, 10)));
					// Set the marker title.
					var markerIconID = GetMarkerIcon (false, (double)grpUsr.Latitude, (double)grpUsr.Longitude);
					markerOptions.InvokeIcon (BitmapDescriptorFactory.FromBitmap (WriteTextOnDrawable (context, markerIconID, grpUsr.CountOf.ToString ())));
					// include the bounds in the camera area.
					builder.Include (latLng);

					var marker = _map.AddMarker (markerOptions);

					// Remove the grouped users from the users list so we don't add them as individuals.
					users.RemoveAll (o => o.UserLocation!= null && o.UserLocation.Latitude == (double)grpUsr.Latitude && o.UserLocation.Longitude == (double)grpUsr.Longitude);

					// Update the store of users with the map marker ID for those in a group.
					AddMapMarker ((double)grpUsr.Latitude, (double)grpUsr.Longitude, marker, null);
				}

				// Add the individuals (not in the same location as others).
				foreach (var user in users.Where(o => o.ID != currentUserID)) {
					if (user.UserLocation != null) {
						var markerOptions = new MarkerOptions ();
						var latLng = new LatLng (user.UserLocation.Latitude, user.UserLocation.Longitude);
						markerOptions.SetPosition (latLng);

						// Set the marker title.
						markerOptions.SetTitle (string.Format ("{0} {1} is at {2}", user.FirstName, user.Surname.Substring(0, 1), Truncate(user.UserLocation.PlaceName, 10)));
						// Set the marker icon colour for male/female.
						var markerIconID = GetMarkerIcon (false, user.UserLocation.Latitude, user.UserLocation.Longitude);
						markerOptions.InvokeIcon (BitmapDescriptorFactory.FromResource(markerIconID));					

						var marker = _map.AddMarker (markerOptions);

						// Update the store of users with the map marker ID for this user.
						AddUserMapMarker (user.ID, marker);

						// include the bounds in the camera area.
						builder.Include (latLng);
					}
				}
			}

			// Current user is available.
			if (currentUserLatitude.HasValue && currentUserLongitude.HasValue) {
				var placeName = " at an unknown location";
				var loggedInUser = users.SingleOrDefault (o => o.ID == currentUserID);
				if (loggedInUser != null && loggedInUser.UserLocation != null) {
					placeName = Truncate(loggedInUser.UserLocation.PlaceName, 10);
				}

				SetUserMarker (context, currentUserLatitude.Value, currentUserLongitude.Value, placeName, currentUserID);
			} 
			else 
			{
				// Other users or current user or both exist.
				if ((currentUserLatitude.HasValue && currentUserLongitude.HasValue) || users != null) {
					var bounds = builder.Build ();

					var padding = 80; // offset from edges of the map in pixels
					var cu = CameraUpdateFactory.NewLatLngBounds (bounds, padding);

					_map.MoveCamera (cu);
					_map.AnimateCamera (cu);
				}
			}
		}

		/// <summary>
		/// Gets the users for marker.
		/// </summary>
		/// <returns>The users for marker.</returns>
		/// <param name="marker">Marker.</param>
		public List<UserLocationsFlat> GetUsersForMarker(int loggedInUserID, Marker marker)
		{
			var latitude = System.Math.Round(marker.Position.Latitude, 6);
			var longitude = System.Math.Round(marker.Position.Longitude, 6);

			var users = MapUserData.Where(o => o.Latitude == latitude && o.Longitude == longitude && o.UserID != loggedInUserID)
				.Select(o => o).ToList(); 

			return users;
		}

		/// <summary>
		/// Determines whether this instance is a group marker.
		/// </summary>
		/// <returns><c>true</c> if this instance is group marker the specified marker; otherwise, <c>false</c>.</returns>
		/// <param name="marker">Marker.</param>
		public bool IsGroupMarker(int loggedInUserID, Marker marker)
		{
			var latitude = System.Math.Round(marker.Position.Latitude, 6);
			var longitude = System.Math.Round(marker.Position.Longitude, 6);

			var count = MapUserData.Count(o => o.Latitude == latitude && o.Longitude == longitude && o.UserID != loggedInUserID);
			return count > 1;
		}

		/// <summary>
		/// Gets the marker icon.
		/// </summary>
		/// <returns>The marker icon.</returns>
		/// <param name="isUserMarker">If set to <c>true</c> is user marker.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		private int GetMarkerIcon(bool isUserMarker, double latitude, double longitude)
		{
			var mapMarkerDrawableID = Resource.Drawable.bothmarker;

			var usersAtLocation = MapUserData.Where (o => o.Latitude == latitude && o.Longitude == longitude).ToList ();
			if (!isUserMarker) {
				usersAtLocation = (from o in usersAtLocation where o.IsLoggedOnUser == false select o).ToList(); 
			}

			if (usersAtLocation.Count == 1 && !isUserMarker) {
				mapMarkerDrawableID = usersAtLocation.First().Sex.ToLower () == "female" ?
					Resource.Drawable.femalemarker : 
					Resource.Drawable.malemarker;
			}

			if (usersAtLocation.Count >= 0 && isUserMarker) {
				mapMarkerDrawableID = Resource.Drawable.usermarker; 
			}

			if (usersAtLocation.Count > 1 && !isUserMarker) {
				// Are all people at the location female?
				if (usersAtLocation.Where (o => o.Sex == "female").Count () == usersAtLocation.Count ()) {
					mapMarkerDrawableID = Resource.Drawable.femalemarker;
				}
				// Are all people at the location male?
				else if (usersAtLocation.Where (o => o.Sex == "male").Count () == usersAtLocation.Count ()) {
					mapMarkerDrawableID = Resource.Drawable.femalemarker;
				} 
			}

			return mapMarkerDrawableID;
		}


		/// <summary>
		/// Sets the marker.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public void SetUserMarker(
			Context context,
			double latitude,
			double longitude,
			string placeName,
			int userID)
		{
			var markerOptions = new MarkerOptions();
	
			markerOptions.SetPosition(new LatLng(latitude, longitude));
			var markerIconID = GetMarkerIcon (true, latitude, longitude);

			// Find out if there is already a map pin at the users location (other users).
			var count = MapUserData.Count (o => o.Latitude == latitude && o.Longitude == longitude && o.UserID != userID);
			if (count > 0) {
				// Remove the existing marker.
				RemoveMarker (latitude, longitude);

				var totalPeopleAtLocation = (count + 1).ToString(); // current users plus logged in user.
				markerOptions.SetTitle (string.Format ("You are at {0} with {1} other people", Truncate(placeName, 10), count));
				// Set the marker icon with number of users there.
				markerOptions.InvokeIcon (BitmapDescriptorFactory.FromBitmap (WriteTextOnDrawable (context, markerIconID , totalPeopleAtLocation)));

				// Add the marker to the map.
				var marker = _map.AddMarker (markerOptions);
			
				// Update the store of users with the map marker ID for those in a group.
				AddMapMarker (latitude, longitude, marker, userID);
			} 
			else 
			{				
				// Set the map marker title.
				markerOptions.SetTitle (string.Format(_userMarkerTitle, Truncate(placeName, 10)));				

				// Set icon to logged in user icon.
				markerOptions.InvokeIcon(BitmapDescriptorFactory.FromResource(markerIconID));

				// If the user already has a pin on the map remove it.
				RemoveMarker(context, userID);

				// Add new pin to map and store the marker in variable.
				var marker = _map.AddMarker (markerOptions);

				// Update the store of users with the map marker ID for this user.
				AddUserMapMarker (userID, marker);
			}


			var padding = 80; // offset from edges of the map in pixels
			
			var builder = new LatLngBounds.Builder();
			builder.Include(markerOptions.Position);
			var bounds = builder.Build();

			var cameraPositionBuilder = new CameraPosition.Builder();				
			cameraPositionBuilder.Zoom (_userZoomLevel);				

			cameraPositionBuilder.Target(new LatLng (latitude, longitude));

			var cu = CameraUpdateFactory.NewLatLngBounds(bounds, padding);
			var cu2 = CameraUpdateFactory.NewCameraPosition (cameraPositionBuilder.Build ());

			_map.MoveCamera (cu);
			_map.AnimateCamera (cu);

			_map.MoveCamera (cu2);
			_map.AnimateCamera (cu2);			
		}


		/// <param name="location">The new location, as a Location object.</param>
		/// <summary>
		/// Called when the location has changed.
		/// </summary>
		public void OnLocationChanged (Location location)
		{
			if(location != null)
			{
				var args = new LocationUpdatedEventArgs ();
				args.Location = location;
				OnLocationUpdated (null, args);				
			}
		}

		public void OnProviderDisabled(string provider) {}

		public void OnProviderEnabled(string provider) {}

		public void OnStatusChanged(string provider, Availability status, Bundle extras) {}
	}

	/// <summary>
	/// Location updated event arguments.
	/// </summary>
	public class LocationUpdatedEventArgs: EventArgs
	{
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		public Location Location { get;	set; }
	}	
}

