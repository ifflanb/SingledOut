using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Xamarin.FacebookBinding;
using Xamarin.FacebookBinding.Widget;
using Xamarin.FacebookBinding.Model;
using Android.Widget;
using SingledOut.Model;
using CSS.Helpers;

[assembly:Permission (Name = Android.Manifest.Permission.Internet)]
[assembly:Permission (Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/app_id")]
namespace SingledOutAndroid
{
	[Activity (Label = "Sign In", Theme = "@android:style/Theme.NoTitleBar")]	
	public class SignIn : BaseActivity
	{
		private static readonly string[] PERMISSIONS = new String [] { "publish_actions" };
		readonly String PENDING_ACTION_BUNDLE_KEY = "com.facebook.samples.singledout:PendingAction";
		private LoginButton facebookLoginButton;
		private PendingAction pendingAction = PendingAction.NONE;
		private ViewGroup controlsContainer;
		private IGraphUser user;
		private UiLifecycleHelper uiHelper;
		private Session.IStatusCallback callback;


		private	enum PendingAction
		{	NONE,
			POST_PHOTO,
			POST_STATUS_UPDATE
		}

		public SignIn ()
		{
			callback = new MyStatusCallback (this);
		}

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			uiHelper = new UiLifecycleHelper (this, callback);
			uiHelper.OnCreate (savedInstanceState);

			if (savedInstanceState != null) {
				string name = savedInstanceState.GetString (PENDING_ACTION_BUNDLE_KEY);
				pendingAction = (PendingAction)Enum.Parse (typeof(PendingAction), name);
			}

			SetContentView (Resource.Layout.SignIn);

			// Start of Facebook login stuff

			facebookLoginButton = (LoginButton)FindViewById (Resource.Id.facebooklogin);
			facebookLoginButton.UserInfoChangedCallback = new MyUserInfoChangedCallback (this);
			facebookLoginButton.SetBackgroundResource(Resource.Drawable.facebooklogin);
			facebookLoginButton.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0); 

			controlsContainer = (ViewGroup)FindViewById (Resource.Id.signinchildlayout);

			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;

			fm.BackStackChanged += delegate {
				if (fm.BackStackEntryCount == 0) {
					// We need to re-show our UI.
					controlsContainer.Visibility = ViewStates.Visible;
				}
			};

			// End of Facebook login stuff

			// Start of Singled Out login stuff

			var singledOutRegisterButtn =  (ImageButton)FindViewById (Resource.Id.singledoutregister);
			singledOutRegisterButtn.Click += SingledOutRegistration;

			var singledOutLoginButtn =  (ImageButton)FindViewById (Resource.Id.singledoutlogin);
			singledOutLoginButtn.Click += SingledOutLogin;
			// End of Singled Out login stuff	
		}

		/// <summary>
		/// Singleds the out registration.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SingledOutRegistration(object sender, EventArgs e)
		{	
			SwipeLeftActivity = typeof(Registration);
			SwipeLeft();
		}

		private void SingledOutLogin(object sender, EventArgs e)
		{	
			SwipeLeftActivity = typeof(Login);
			SwipeLeft();
		}


		class MyStatusCallback : Java.Lang.Object, Session.IStatusCallback
		{
			SignIn owner;

			public MyStatusCallback (SignIn owner)
			{
				this.owner = owner;
			}

			public void Call (Session session, SessionState state, Java.Lang.Exception exception)
			{
				owner.OnSessionStateChange (session, state, exception);
			}
		}

		class MyUserInfoChangedCallback : Java.Lang.Object, LoginButton.IUserInfoChangedCallback
		{
			SignIn owner;

			public MyUserInfoChangedCallback (SignIn owner)
			{
				this.owner = owner;
			}

			/// <summary>
			/// Raises the user info fetched event.
			/// </summary>
			/// <param name="user">User.</param>
			public void OnUserInfoFetched (IGraphUser user)
			{
				owner.user = user;

				if (user != null) {
					var accessToken = "";
					var session = Session.ActiveSession;
					if (session.IsOpened) {
						accessToken = session.AccessToken;
					}

					var jsonUser = user.InnerJSONObject;
					SaveFacebookDetails (jsonUser, accessToken);

					if (string.IsNullOrEmpty(owner.GetUserPreference ("FacebookAccessToken"))) {
						owner.SetUserPreference ("FacebookAccessToken", accessToken);
						owner.SetUserPreference ("FacebookUsername", jsonUser.GetString("id"));
					} 
				}
			}

			/// <summary>
			/// Saves the facebook details.
			/// </summary>
			/// <param name="user">User.</param>
			/// <param name="facebookAccessToken">Facebook access token.</param>
			private void SaveFacebookDetails(Org.Json.JSONObject user, string facebookAccessToken)
			{
				var userModel = new UserModel {
					FirstName = user.GetString("first_name"),
					Surname = user.GetString("last_name"),
					Sex = user.GetString("gender"),
					CreatedDate = DateTime.UtcNow,
					FacebookAccessToken = facebookAccessToken,
					FacebookUserName = user.GetString("id"),
					UpdateDate = DateTime.UtcNow
				};					

				var restHelper = new RestHelper ();
				var url = string.Concat(owner.GetString(Resource.String.apihost), owner.GetString(Resource.String.apipath),owner.GetString(Resource.String.apiurlusers));
				restHelper.PostAsync(url , userModel);
			}
		}

		/// <summary>
		/// Raises the resume event.
		/// </summary>
		protected override void OnResume ()
		{
			base.OnResume ();
			uiHelper.OnResume ();

			UpdateUI ();
		}

		/// <param name="outState">Bundle in which to place your saved state.</param>
		/// <summary>
		/// Raises the save instance state event.
		/// </summary>
		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			uiHelper.OnSaveInstanceState (outState);

			outState.PutString (PENDING_ACTION_BUNDLE_KEY, pendingAction.ToString ());
		}

		/// <param name="requestCode">The integer request code originally supplied to
		///  startActivityForResult(), allowing you to identify who this
		///  result came from.</param>
		/// <param name="resultCode">The integer result code returned by the child activity
		///  through its setResult().</param>
		/// <param name="data">An Intent, which can return result data to the caller
		///  (various data can be attached to Intent "extras").</param>
		/// <summary>
		/// Called when an activity you launched exits, giving you the requestCode
		///  you started it with, the resultCode it returned, and any additional
		///  data from it.
		/// </summary>
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			uiHelper.OnActivityResult (requestCode, (int)resultCode, data);
			if (Session.ActiveSession.IsOpened) {
				StartActivity(typeof(Tutorial1));
				OverridePendingTransition (Resource.Drawable.slide_in_left, Resource.Drawable.slide_out_left);
			}
		}

		/// <summary>
		/// Called as part of the activity lifecycle when an activity is going into
		///  the background, but has not (yet) been killed.
		/// </summary>
		protected override void OnPause ()
		{
			base.OnPause ();
			uiHelper.OnPause ();
		}

		/// <summary>
		/// Perform any final cleanup before an activity is destroyed.
		/// </summary>
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			uiHelper.OnDestroy ();
		}

		/// <summary>
		/// Raises the session state change event.
		/// </summary>
		/// <param name="session">Session.</param>
		/// <param name="state">State.</param>
		/// <param name="exception">Exception.</param>
		private void OnSessionStateChange (Session session, SessionState state, Exception exception)
		{
			if (pendingAction != PendingAction.NONE &&
				(exception is FacebookOperationCanceledException ||
				exception is FacebookAuthorizationException)) {
				new AlertDialog.Builder (this)
					.SetTitle (Resource.String.cancelled)
						.SetMessage (Resource.String.permission_not_granted)
						.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
						.Show ();
				pendingAction = PendingAction.NONE;
			} else if (state == SessionState.OpenedTokenUpdated) {
				HandlePendingAction ();
			}
			UpdateUI ();
		}

		/// <summary>
		/// Updates the U.
		/// </summary>
		private void UpdateUI ()
		{
			Session session = Session.ActiveSession;
		}

		/// <summary>
		/// Handles the pending action.
		/// </summary>
		private void HandlePendingAction ()
		{
			PendingAction previouslyPendingAction = pendingAction;
			// These actions may re-set pendingAction if they are still pending, but we assume they
			// will succeed.
			pendingAction = PendingAction.NONE;
		}

		class RequestCallback : Java.Lang.Object, Request.ICallback
		{
			Action<Response> action;

			public RequestCallback (Action<Response> action)
			{
				this.action = action;
			}

			public void OnCompleted (Response response)
			{
				action (response);
			}
		}
	}
}
