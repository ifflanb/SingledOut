
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
	[Activity (Label = "Check-In", Theme = "@android:style/Theme.NoTitleBar")]			
	public class CheckIn : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CheckIn);

			SwipeRightActivity = typeof(Tutorial2);
		}
	}
}

