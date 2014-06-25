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

namespace SingledOutAndroid
{
	[Activity (Label = "Tutorial - Step 1", Theme = "@android:style/Theme.NoTitleBar", NoHistory=true)]			
	public class Tutorial1 : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Tutorial1);

			SwipeLeftActivity = typeof(Tutorial2);

			// Add footer.
			var footerFragment = new FooterLayout ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.tutorial1childlayout, footerFragment);
			ft.Commit ();

			// Show welcome back message.
			if (LastActivity == "Registration" || LastActivity == "SignIn") {
				ShowNotificationBox (string.Concat ("Welcome to Singled Out ", CurrentUser.FirstName, "!"));
			}
		}
	}
}

