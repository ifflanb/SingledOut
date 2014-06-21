using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.FacebookBinding;
using System.Timers;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Views.InputMethods;
using Android.Graphics.Drawables;
using Android.Content;
using Android.Content.Res;
using MobileSpace.Helpers;

namespace SingledOutAndroid
{				
	public class BaseActivity : FragmentActivity , GestureDetector.IOnGestureListener
	{				  
		private const int SWIPE_MIN_DISTANCE = 120;
		private const int SWIPE_MAX_OFF_PATH = 250;
		private const int SWIPE_THRESHOLD_VELOCITY = 200;
		private Timer _timer;
		private GestureDetector _gestureDetector;
		private UriCreator _uriCreator;

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);			 
			this.RequestedOrientation = ScreenOrientation.Portrait;

			_gestureDetector = new GestureDetector(this);
			_uriCreator = new UriCreator (Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));
		}

		/// <summary>
		/// Gets the URI creator.
		/// </summary>
		/// <value>The URI creator.</value>
		protected UriCreator UriCreator {
			get {
				return _uriCreator;
			}
		}
		/// <summary>
		/// Gets the validation warning drawable.
		/// </summary>
		/// <returns>The validation warning drawable.</returns>
		protected Drawable GetValidationWarningDrawable()
		{
			var warning = (Drawable)Resources.GetDrawable(Resource.Drawable.exclamation);
			warning.SetBounds(0, 0, warning.IntrinsicWidth/3, warning.IntrinsicHeight/3);
			return warning;
		}

		/// <summary>
		/// Starts the activity after pause.
		/// </summary>
		/// <param name="activity">Activity.</param>
		protected void StartActivityAfterPause(Type activity)
		{
			// Wait 5 seconds after the animation before opening the next page.
			_timer = new System.Timers.Timer();
			_timer.Interval = 4000;
			_timer.Elapsed +=(object sender, System.Timers.ElapsedEventArgs e) => StopTimerAndStartActivity(activity);
			_timer.Start ();
		}

		/// <summary>
		/// Stops the timer and start activity.
		/// </summary>
		/// <param name="activity">Activity.</param>
		private void StopTimerAndStartActivity(Type activity)
		{
			_timer.Stop ();
			StartActivity (activity);
		}

		/// <param name="e">The touch screen event being processed.</param>
		/// <summary>
		/// Called when a touch screen event was not handled by any of the views
		///  under it.
		/// </summary>
		/// <returns>To be added.</returns>
		public override bool OnTouchEvent(MotionEvent e)
		{
			_gestureDetector.OnTouchEvent(e);
			return false;
		}

		/// <summary>
		/// Gets or sets the swipe left activity.
		/// </summary>
		/// <value>The swipe left activity.</value>
		public Type SwipeLeftActivity { get; set;}

		/// <summary>
		/// Gets or sets the swipe right activity.
		/// </summary>
		/// <value>The swipe right activity.</value>
		public Type SwipeRightActivity { get; set;}

		/// <summary>
		/// Called when the activity has detected the user's press of the back
		///  key.
		/// </summary>
		public override void OnBackPressed(){
			//base.OnBackPressed();
			if (SwipeRightActivity != null) {
				SwipeRight ();
			}
		}

		/// <summary>
		/// Swipes the left.
		/// </summary>
		public void SwipeLeft()
		{
			StartActivity(new Intent(ApplicationContext, SwipeLeftActivity));
			OverridePendingTransition (Resource.Drawable.slide_in_left, Resource.Drawable.slide_out_left);
		}

		/// <summary>
		/// Swipes the right.
		/// </summary>
		public void SwipeRight()
		{
			StartActivity(new Intent(ApplicationContext, SwipeRightActivity));
			OverridePendingTransition (Resource.Drawable.slide_out_left, Resource.Drawable.slide_in_left);
			OverridePendingTransition (Resource.Drawable.slide_out_left, Resource.Drawable.slide_in_left);
		}

		/// <summary>
		/// Raises the down event.
		/// </summary>
		/// <param name="e">E.</param>
		public bool OnDown (MotionEvent e)
		{
			return true;
		}

		/// <summary>
		/// Raises the fling event.
		/// </summary>
		/// <param name="e1">E1.</param>
		/// <param name="e2">E2.</param>
		/// <param name="velocityX">Velocity x.</param>
		/// <param name="velocityY">Velocity y.</param>
		public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			try
			{
				if (Math.Abs(e1.GetY() - e2.GetY()) > SWIPE_MAX_OFF_PATH)
				{
					return false;
				}
				// swipe to the left.
				if (e1.GetX() - e2.GetX() > SWIPE_MIN_DISTANCE && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY)
				{
					SwipeLeft();
				}
				// Swipe to the right.
				else if (e2. GetX()- e1.GetX() > SWIPE_MIN_DISTANCE && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY)
				{
					SwipeRight();
				}
			}
			catch (Exception)
			{
				// nothing
			}
			return false;
		}

		/// <summary>
		/// Raises the scroll event.
		/// </summary>
		/// <param name="e1">E1.</param>
		/// <param name="e2">E2.</param>
		/// <param name="distanceX">Distance x.</param>
		/// <param name="distanceY">Distance y.</param>
		public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return true;
		}

		public void OnLongPress (MotionEvent e)
		{

		}

		public void OnShowPress (MotionEvent e)
		{

		}

		/// <summary>
		/// Raises the single tap up event.
		/// </summary>
		/// <param name="e">E.</param>
		public bool OnSingleTapUp (MotionEvent e)
		{
			return true;
		}

		/// <summary>
		/// Sets the user preference.
		/// </summary>
		/// <param name="Key">Key.</param>
		/// <param name="value">Value.</param>
		protected void SetUserPreference(string Key, string value)
		{
			// Store.
			var prefs = Application.Context.GetSharedPreferences(this.Resources.GetString(Resource.String.app_name), FileCreationMode.Private);
			var prefEditor = prefs.Edit();
			prefEditor.PutString(Key, value);
			prefEditor.Commit();
		}

		/// <summary>
		/// Gets the user preference.
		/// </summary>
		/// <returns>The user preference.</returns>
		/// <param name="Key">Key.</param>
		protected string GetUserPreference(string Key)
		{
			// Retreive 
			var prefs = Application.Context.GetSharedPreferences(this.Resources.GetString(Resource.String.app_name), FileCreationMode.Private);              
			var userPref = prefs.GetString(Key, null);
			return userPref;
		}
	}
}

