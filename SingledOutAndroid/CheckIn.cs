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
using Android.Gms.Maps;

namespace SingledOutAndroid
{
	[Activity (Label = "Check-In", Theme = "@android:style/Theme.NoTitleBar")]			
	public class CheckIn : Activity//BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CheckIn);

			//SwipeRightActivity = typeof(Tutorial2);

			// Show welcome back message.
//			if (LastActivity == "Login" || LastActivity == "SplashPage") {
//				ShowNotificationBox (string.Concat ("Welcome back ", CurrentUser.FirstName, "!"));
//			}
			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			GoogleMap map = mapFrag.Map;
			if (map != null) {
				// The GoogleMap object is ready to go.
			}
		}


	}
}

