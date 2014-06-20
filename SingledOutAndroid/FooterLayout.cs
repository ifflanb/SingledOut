
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android;

namespace SingledOutAndroid
{
	public class FooterLayout : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.Footer_Layout, container, false);
			var imageView = view.FindViewById<ImageView> (Resource.Id.ellipse);
			if (container.Id == Resource.Id.welcomechildlayout) {
				imageView.SetImageResource (Resource.Drawable.ellipse1);
			}
			if (container.Id == Resource.Id.signincontainerinnerlayout) {
				imageView.SetImageResource (Resource.Drawable.ellipse2);
			}
			if (container.Id == Resource.Id.tutorial1childlayout) {
				imageView.SetImageResource (Resource.Drawable.ellipse3);
			}

			return view;
		}
	}
}

