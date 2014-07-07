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

namespace SingledOutAndroid
{
	public class MapHelper : Activity,  Android.Locations.ILocationListener
	{
		LocationManager _locationManager;
		Activity _activity;
		private GoogleMap _map;
		public delegate void LocationUpdated(object sender, LocationUpdatedEventArgs e);
		public event LocationUpdated OnLocationUpdated;

		public Marker UserMarker {
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether the user location is set.
		/// </summary>
		/// <value><c>true</c> if this instance is user location set; otherwise, <c>false</c>.</value>
		public bool IsUserLocationSet { 
			get {
				return UserMarker != null;
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
		public void RemoveMarker()
		{
			if (UserMarker != null) {
				UserMarker.Remove ();
				UserMarker = null;
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

			Criteria myCriteria = new Criteria();
			myCriteria.Accuracy = Accuracy.Medium; // Set this to medium to use non GPS provider.
			myCriteria.PowerRequirement = Power.Low; // Set this to low to use non GPS provider.
			// let Android select the right location provider for you
			String locationProvider = _locationManager.GetBestProvider(myCriteria, true);

			// Request location updates.
			_locationManager.RequestLocationUpdates(locationProvider, updatesTime, updatesWithinDistance, this);
			return _locationManager;
		}

		/// <summary>
		/// Sets the marker.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public void SetMarker(
			double latitude, 
			double longitude, 
			int zoomLevel, 
			string markerTitle, 
			int markerIconID, 
			bool centerOnMarker)
		{
			var markerOptions = new MarkerOptions();
			markerOptions.SetPosition(new LatLng(latitude, longitude));
			if (!string.IsNullOrEmpty (markerTitle)) {
				markerOptions.SetTitle (markerTitle);
			}
			if (markerIconID > 0) {
				markerOptions.InvokeIcon (BitmapDescriptorFactory.FromResource (markerIconID));
			}

			if (UserMarker != null) {
				RemoveMarker();
			} 
			UserMarker = _map.AddMarker(markerOptions);

			var builder = new LatLngBounds.Builder();
			builder.Include(markerOptions.Position);
			var bounds = builder.Build();

			var cameraPositionBuilder = new CameraPosition.Builder();
			cameraPositionBuilder.Zoom (zoomLevel);
			cameraPositionBuilder.Target(new LatLng (latitude, longitude));

			var padding = 0; // offset from edges of the map in pixels
			var cu = CameraUpdateFactory.NewLatLngBounds(bounds, padding);
			var cu2 = CameraUpdateFactory.NewCameraPosition (cameraPositionBuilder.Build ());

			if (centerOnMarker) {
				_map.MoveCamera (cu);
				_map.AnimateCamera (cu);

				_map.MoveCamera (cu2);
				_map.AnimateCamera (cu2);
			}
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

