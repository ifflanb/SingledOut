
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
	[Activity (Label = "Tutorial - Step 2", Theme = "@android:style/Theme.NoTitleBar")]					
	public class Tutorial2 : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Tutorial2);

			SwipeRightActivity = typeof(Tutorial1);
			SwipeLeftActivity = typeof(CheckIn);

			// Add footer.
			var footerFragment = new FooterLayout ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.tutorial2childlayout, footerFragment);
			ft.Commit ();
		}
	}
}

