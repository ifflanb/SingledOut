using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SingledOutAndroid
{
	[Activity (Label = "Welcome", 
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class Welcome : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SwipeLeftActivity = typeof(SignIn);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Welcome);

			var welcomebutton = FindViewById<Button> (Resource.Id.welcomebutton);
			welcomebutton.Click += (sender, e) => {
				StartActivity(typeof(SignIn));
			};
		}
	}
}


