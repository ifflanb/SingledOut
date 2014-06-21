
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
using Android.Webkit;

namespace SingledOutAndroid
{
	[Activity (Label = "TermsConditions", Theme = "@android:style/Theme.NoTitleBar")]			
	public class TermsConditions : BaseActivity
	{
		private WebView _webView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.TermsConditions);
			SwipeRightActivity = typeof(Registration);
			LoadWebView ();
		}

		private void LoadWebView()
		{
			_webView = new WebView (this);
			SetContentView(_webView);
			_webView.LoadUrl("file:///android_asset/Terms.html");   
		}
	}
}

