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
using Android.Views.InputMethods;
using Android.Graphics.Drawables;
using Android.Content;
using Android.Content.Res;
using MobileSpace.Helpers;
using Newtonsoft.Json;
using SingledOut.Model;
using Android.Views.Animations;
using Android.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Android.Support.V4.App;
using Android.Graphics;
using Android.Text;
using System.Security.Principal;
using System.Threading;
using Java.Net;
using System.Net.NetworkInformation;

namespace SingledOutAndroid
{				
	public class BaseActivity : Activity,  GestureDetector.IOnGestureListener
	{				  
		private const int SWIPE_MIN_DISTANCE = 120;
		private const int SWIPE_MAX_OFF_PATH = 250;
		private const int SWIPE_THRESHOLD_VELOCITY = 200;
		private System.Timers.Timer _timer;
		private GestureDetector _gestureDetector;
		public string GoogleApiKey; 

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);			 
			this.RequestedOrientation = ScreenOrientation.Portrait;

			// Set Google Api Key value;
			GoogleApiKey = Resources.GetString(Resource.String.googleapikey);

			_gestureDetector = new GestureDetector(this);
		}

		public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu (menu, v, menuInfo);

			MenuInflater menuInflater = new MenuInflater (this);
			menuInflater.Inflate (Resource.Menu.actionbarmenu, menu);
		}

		private bool _isActionBarVisible;
		/// <summary>
		/// Gets or sets my property.
		/// </summary>
		/// <value>My property.</value>
		public bool IsActionBarVisible {
			get {
				return _isActionBarVisible;
			}
			set 
			{
				_isActionBarVisible = value;
				RequestWindowFeature (WindowFeatures.ActionBar);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SingledOutAndroid.BaseActivity"/> show action bar tabs.
		/// </summary>
		/// <value><c>true</c> if show action bar tabs; otherwise, <c>false</c>.</value>
		public bool ShowActionBarTabs { get; set; }

		/// <summary>
		/// Shows the action bar.
		/// </summary>
		public void ShowActionBar() { 
			ActionBar.NavigationMode = ShowActionBarTabs ? ActionBarNavigationMode.Tabs : ActionBarNavigationMode.Standard;
			if (ShowActionBarTabs) {
				ActionBar.SetStackedBackgroundDrawable (new ColorDrawable (Color.ParseColor ("#007FFF")));
			}

			ActionBar.SetTitle(Resource.String.actionbartitlecheckin);
			ActionBar.SetSubtitle(Resource.String.actionbarsubtitlecheckin);
			ActionBar.SetDisplayShowTitleEnabled (true);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			var colorDrawable = new ColorDrawable(Color.ParseColor("#ffffff"));
			ActionBar.SetBackgroundDrawable(colorDrawable); 
			int actionBarTitleId = Resources.GetIdentifier("action_bar_title", "id", "android");
			if (actionBarTitleId > 0) {
				TextView title = (TextView) FindViewById(actionBarTitleId);
				if (title != null) {
					title.SetTextColor(Color.Gray);
				}
			}
		}

		/*
		 * attach the menu to the menu button of the device
		 * for this activity
		 */
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			if (IsActionBarVisible) {
				ShowActionBar ();

				var inflater = this.MenuInflater;
				inflater.Inflate (Resource.Menu.actionbarmenu, menu);
			}
			return true;
		}

		/// <summary>
		/// Factories the start new.
		/// </summary>
		/// <returns>The start new.</returns>
		/// <param name="func">Func.</param>
		public Task<HttpResponseMessage> FactoryStartNew(Func<HttpResponseMessage> func)
		{
			Task<HttpResponseMessage> httpResponseMessage = null;

			if (IsNetworkAvailable ()) {
				httpResponseMessage = Task<HttpResponseMessage>.Factory.StartNew (func);
			}
			else 
			{
				var dialogBuilder = new AlertDialog.Builder (this);						
				dialogBuilder.SetTitle ("No Internet Connection");
				dialogBuilder.SetIcon (Resource.Drawable.erroricon);
				dialogBuilder.SetMessage ("No Internet Connection. Please connect and try again.");
				dialogBuilder.SetPositiveButton("OK", delegate { dialogBuilder.Dispose(); });
				var dialog = dialogBuilder.Create();
				dialog.Show ();	
				httpResponseMessage = null;
			}
			return httpResponseMessage;
		}

		/// <summary>
		/// Gets or sets the authentication token.
		/// </summary>
		/// <value>The authentication token.</value>
		public string AuthenticationToken {
			get 
			{
				var token = string.Empty;
				if (IsAuthenticated) 
				{
					token = GetUserPreference ("AuthToken");
				}
				return token;
			}
			set 
			{
				SetUserPreference ("AuthToken", value);
			}
		}

		/// <summary>
		/// Gets or sets the user ID.
		/// </summary>
		/// <value>The user ID.</value>
		public int? UserID {
			get 
			{
				int? userID = null;
				var id = GetUserPreference ("UserID");
				if(!string.IsNullOrEmpty(id))
					{
						userID = int.Parse(id);
					}
				return userID;
			}
			set 
			{
				SetUserPreference ("UserID", value.ToString());
			}
		}

		/// <summary>
		/// Determines whether this instance is network available.
		/// </summary>
		/// <returns><c>true</c> if this instance is network available; otherwise, <c>false</c>.</returns>
		private bool IsNetworkAvailable() 
		{
			var isAvailalble = false; 
			var connectionDetector = new ConnectionDetector(ApplicationContext);
			isAvailalble =  connectionDetector.IsConnectedToInternet();

			if (isAvailalble)
			{

				var uri = Resources.GetString (Resource.String.apihost);
				if (!string.IsNullOrEmpty (uri)) {
					uri = uri.Remove (uri.Length - 1, 1);
				}
				Ping ping = new Ping ();
				PingReply pingReply = null;

				try
				{
					pingReply = ping.Send (uri);
				}
				catch(Exception ex) {
					isAvailalble = false;
				}

				if (pingReply.Status != IPStatus.Success)  //Successful response.
				{
					isAvailalble = false;
				} 
			}

			return isAvailalble;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is first visit.
		/// </summary>
		/// <value><c>true</c> if this instance is first visit; otherwise, <c>false</c>.</value>
		public bool IsFirstVisit {
			get {
				var isFirstVisit = false;
				if (GetUserPreference ("Visits") == null || GetUserPreference ("Visits") != "1") {
					isFirstVisit = true;
				}
				return isFirstVisit;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the user is authenticated.
		/// </summary>
		/// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
		public bool IsAuthenticated {
			get {
				var isAutheticated = false;
				if (!string.IsNullOrEmpty (GetUserPreference ("FacebookAccessToken")) ||
					!string.IsNullOrEmpty (GetUserPreference ("SingledOutEmail"))) {
					isAutheticated = true;
				}
				return isAutheticated;
			}
		}

		/// <summary>
		/// Gets the current user.
		/// </summary>
		/// <value>The current user.</value>
		public UserModel CurrentUser {
			get {
				UserModel userModel = null; 
				if (IsAuthenticated) {
					var json = GetUserPreference ("SingledOutUser");
					if (json != null) {
						userModel = JsonConvert.DeserializeObject<UserModel> (json);
					}
				}
				return userModel;
			}		
		}

		/// <summary>
		/// Gets the ignore pages for authentication.
		/// </summary>
		/// <value>The ignore pages for authentication.</value>
		public List<Type> IgnorePagesForAuthentication
		{
			get {
				var typesToIgnore = new List<Type> {
					typeof(Login) ,
					typeof(Registration),
					typeof(SignIn),
					typeof(Welcome),
					typeof(TermsConditions),
					typeof(ForgottenPassword)
				};
				return typesToIgnore;
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
		protected void StartActivityAfterPause(Type activity, string callingActivity = "")
		{
			// Wait 5 seconds after the animation before opening the next page.
			_timer = new System.Timers.Timer();
			_timer.Interval = 4000;
			_timer.Elapsed +=(object sender, System.Timers.ElapsedEventArgs e) => StopTimerAndStartActivity(activity, callingActivity);
			_timer.Start ();
		}

		/// <summary>
		/// Stops the timer and start activity.
		/// </summary>
		/// <param name="activity">Activity.</param>
		private void StopTimerAndStartActivity(Type activity, string callingActivity = "")
		{
			_timer.Stop ();
			SwipeLeftActivity = activity;
			SwipeLeft (callingActivity);
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
			if (IsAuthenticated || IgnorePagesForAuthentication.Contains(SwipeRightActivity)) {
				if (SwipeRightActivity != null) {
					SwipeRight ();
				}
			} 
			else
			{
				StartActivity(new Intent(ApplicationContext, typeof(SignIn)));
			}
		}

		/// <summary>
		/// Return the name of the activity that invoked this activity.
		/// </summary>
		/// <value>To be added.</value>
		public string LastActivity {
			get {
				var callingActivity = Intent.GetStringExtra("CallingActivity");
				return callingActivity;
			}
		}

		/// <summary>
		/// Swipes the left.
		/// </summary>
		public void SwipeLeft(string callingActivity = "")
		{
			if (IsAuthenticated || IgnorePagesForAuthentication.Contains(SwipeLeftActivity)){
				var intent = new Intent (ApplicationContext, SwipeLeftActivity);
				intent.PutExtra ("CallingActivity", callingActivity);
				StartActivity (intent);
				OverridePendingTransition (Resource.Drawable.slide_in_left, Resource.Drawable.slide_out_left);
			}
			else
			{
				StartActivity(new Intent(ApplicationContext, typeof(SignIn)));
			}
		}

		/// <summary>
		/// Swipes the right.
		/// </summary>
		public void SwipeRight()
		{
			if (IsAuthenticated || IgnorePagesForAuthentication.Contains(SwipeRightActivity)) {
				StartActivity (new Intent (ApplicationContext, SwipeRightActivity));
				OverridePendingTransition (Resource.Drawable.slide_out_left, Resource.Drawable.slide_in_left);
			}
			else
			{
				StartActivity(new Intent(ApplicationContext, typeof(SignIn)));
			}
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
		public string GetUserPreference(string Key)
		{
			// Retreive 
			var prefs = Application.Context.GetSharedPreferences(this.Resources.GetString(Resource.String.app_name), FileCreationMode.Private);              
			var userPref = prefs.GetString(Key, null);
			return userPref;
		}

		/// <summary>
		/// Shows the notification box.
		/// </summary>
		/// <param name="message">Message.</param>
		public void ShowNotificationBox(string message, bool bringtofront = false)
		{
			var notification = FindViewById<TextView> (Resource.Id.notification);			
			// Load the welcome back animation.
			notification.Text = message;
			if (bringtofront) {
				notification.BringToFront ();
			}
			var slideDown = AnimationUtils.LoadAnimation(ApplicationContext, Resource.Drawable.SlideDownAnimation);
			var slideUp = AnimationUtils.LoadAnimation(ApplicationContext, Resource.Drawable.SlideUpAnimation);
			slideUp.StartOffset = 4000;
			slideUp.AnimationEnd += delegate {
				notification.Visibility = ViewStates.Invisible;
			};
			slideDown.AnimationEnd += delegate {
				notification.StartAnimation(slideUp);
			};
			notification.Visibility = ViewStates.Visible;
			notification.StartAnimation(slideDown);
		}
	}
}

