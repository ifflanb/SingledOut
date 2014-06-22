using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SingledOutAndroid
{
	[Activity (Label = "Welcome", Theme = "@android:style/Theme.NoTitleBar")]
	public class Welcome : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SwipeLeftActivity = typeof(SignIn);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Welcome);

			var footerFragment = new FooterLayout ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.welcomechildlayout, footerFragment);
			ft.Commit ();

			// Set that they have visited now so we don't show this page again.
			SetUserPreference ("Visits", "1");
		}
	}
}


