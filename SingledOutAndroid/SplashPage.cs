using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Views.Animations;
using Android.Util;
using System.Threading;
using System.Threading.Tasks;
using CSS.Helpers;

namespace SingledOutAndroid
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, 
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, 
		NoHistory = true)]			
	public class SplashPage : Activity
	{
		private AnimationHelper _animationHelper;
		private SoundHelper _soundHelper;
		private System.Timers.Timer _timer;

		protected override void OnCreate (Bundle bundle)
		{
			_animationHelper = new AnimationHelper ();
			_soundHelper = new SoundHelper ();

			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SplashPage);

			// Start the animation.
			RunAnimation();
			// Wait 5 seconds after the animation before opening the next page.
			_timer = new System.Timers.Timer();
			_timer.Interval = 4000;
			_timer.Elapsed +=(object sender, System.Timers.ElapsedEventArgs e) => StartWelcomeActivity();
			_timer.Start ();	
		}

		private void StartWelcomeActivity()
		{
			_timer.Stop ();
			StartActivity (typeof(Welcome));
		}

		/// <summary>
		/// Runs the animation.
		/// </summary>
		private void RunAnimation()
		{
			ImageView logonopin = (ImageView)FindViewById<ImageView> (Resource.Id.logonopin);
			ImageView pin = (ImageView)FindViewById<ImageView> (Resource.Id.pin);
			LinearLayout childLayout = (LinearLayout)FindViewById (Resource.Id.childlayout1);
			LinearLayout containerlayout = (LinearLayout)FindViewById (Resource.Id.containerlayout);

			var pinStartPos = (int)(logonopin.LayoutParameters.Width * 0.35); // .35 is a guess...may not work for all screeens.
			var screenHeight = _animationHelper.GetScreenHeightWithoutStatusBar (containerlayout, this);
			var halfWayUpScreen = (screenHeight - logonopin.LayoutParameters.Height) / 2;

			var toYPosition = halfWayUpScreen - (logonopin.LayoutParameters.Height);
			_animationHelper.SetLayoutHeight (childLayout, halfWayUpScreen + 100); // Set the height of the layout to half screen size.

			// Creates the logo slide up animation.
			var animateLogo = _animationHelper.CreateViewAnimation (logonopin, 0, 0, halfWayUpScreen, toYPosition, 1000);

			// Event for playing sound during logo slide up animation.
			animateLogo.AnimationStart += (object sender, Android.Views.Animations.Animation.AnimationStartEventArgs evt) => {
				_soundHelper.PlaySound (Resource.Raw.logoarrives, this);
			};

			// Event for starting pin slide down animation after logo slide up animation ends.
			animateLogo.AnimationEnd += (object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e) => {
				var yoffset = (int)(logonopin.LayoutParameters.Height * 0.30);
				var animatePin = _animationHelper.CreateViewAnimation (pin, -pinStartPos, -pinStartPos, 0, toYPosition + yoffset, 800);
				// Event for playing sound after the pin has slid down.
				animatePin.AnimationEnd += (object send, Android.Views.Animations.Animation.AnimationEndEventArgs ev) => {
				_soundHelper.PlaySound (Resource.Raw.pindropsound, this);
				};
				pin.Visibility = ViewStates.Visible;
				pin.StartAnimation (animatePin); // start pin slide down animation.
			};

			logonopin.Visibility = ViewStates.Visible;
			logonopin.StartAnimation (animateLogo); // start logo slide up animation.
		}
	}
}

