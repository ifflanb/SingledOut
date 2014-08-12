using System;
using Android.Gms.Maps.Model;

namespace SingledOutAndroid
{
	/// <summary>
	/// User locations flat.
	/// </summary>
	public class UserLocationsFlat
	{
		public bool IsLoggedOnUser { get; set; }

		public string FirstName { get; set; }

		public string Surname { get; set; }

		public int UserID { get; set; }

		public int? Age { get; set; }

		public string Sex { get; set; }

		public double? Latitude { get; set; }

		public double? Longitude { get; set; }

		public string PlaceName { get; set; }

		public string ProfilePicture { get; set; }

		public Marker MapMarker { get; set; }
	}
}

