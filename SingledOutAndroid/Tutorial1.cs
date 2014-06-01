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
	[Activity (Label = "Tutorial - Step 1")]			
	public class Tutorial1 : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Tutorial1);

			SwipeRightActivity = typeof(SignIn);
		}
	}
}

