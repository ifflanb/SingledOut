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
using Android.Gms.Maps;
using Android.Locations;
using Android.Gms.Maps.Model;

namespace SingledOutAndroid
{
	[Activity (Label = "Check-In", Theme = "@android:style/Theme.NoTitleBar")]			
	public class CheckIn : Activity, ILocationListener//BaseActivity
	{
		Location _currentLocation;
		LocationManager _locationManager;
		String _locationProvider;
		Button _btnCheckin;
		GoogleMap _map;
		MarkerOptions _currentUserMarker;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CheckIn);

			_btnCheckin = (Button)FindViewById (Resource.Id.btnCheckin);
			_btnCheckin.Click += btnCheckin_OnClick;

			//SwipeRightActivity = typeof(Tutorial2);

			// Show welcome back message.
//			if (LastActivity == "Login" || LastActivity == "SplashPage") {
//				ShowNotificationBox (string.Concat ("Welcome back ", CurrentUser.FirstName, "!"));
//			}
			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			_map = mapFrag.Map;
			if (_map != null) {
				// The GoogleMap object is ready to go.
				_map.UiSettings.ZoomControlsEnabled = true;
				_map.UiSettings.CompassEnabled = true;
			}
			InitializeLocationManager();
		}

		async void btnCheckin_OnClick(object sender, EventArgs eventArgs)
		{

			if (_currentLocation == null)
			{
				//_addressText.Text = "Can't determine the current address.";
				return;
			}

			Geocoder geocoder = new Geocoder(this);
			IList<Address> addressList = await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

			Address address = addressList.FirstOrDefault();
			if (address != null)
			{
				StringBuilder deviceAddress = new StringBuilder();
				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.Append(address.GetAddressLine(i))
						.AppendLine(",");
				}
				//_addressText.Text = deviceAddress.ToString();
			}
			else
			{
				//_addressText.Text = "Unable to determine the address.";
			}
		}

		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;
			if (_currentLocation == null)
			{
				//_locationText.Text = "Unable to determine your location.";
			}
			else
			{
				if (_currentUserMarker == null) {
					SetMarker (_currentLocation.Latitude, _currentLocation.Longitude);
				}
			}
		}

		private void SetMarker(double latitude, double longitude)
		{
			_currentUserMarker = new MarkerOptions();
			_currentUserMarker.SetPosition(new LatLng(latitude, longitude));
			_currentUserMarker.SetTitle("You are here!");
			_currentUserMarker.InvokeIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.logopindialog));
			_map.AddMarker(_currentUserMarker);

			LatLngBounds.Builder builder = new LatLngBounds.Builder();
			builder.Include(_currentUserMarker.Position);
			LatLngBounds bounds = builder.Build();

			var cameraPositionBuilder = new CameraPosition.Builder();
			cameraPositionBuilder.Zoom (16);
			cameraPositionBuilder.Target(new LatLng (latitude, longitude));

			int padding = 0; // offset from edges of the map in pixels
			var cu = CameraUpdateFactory.NewLatLngBounds(bounds, padding);
			var cu2 = CameraUpdateFactory.NewCameraPosition (cameraPositionBuilder.Build ());

			_map.MoveCamera(cu);
			_map.AnimateCamera(cu);

			_map.MoveCamera(cu2);
			_map.AnimateCamera(cu2);

			//_map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPositionBuilder.Build()));
		}

		protected override void OnResume()
		{
			base.OnResume();
			_locationManager.RequestLocationUpdates(_locationProvider, 5000, 10, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}

		public void OnProviderDisabled(string provider) {}

		public void OnProviderEnabled(string provider) {}

		public void OnStatusChanged(string provider, Availability status, Bundle extras) {}


		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = String.Empty;
			}
		}

	}
}

