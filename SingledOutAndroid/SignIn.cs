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
using MobileSpace.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Json;
using System.Net;
using Java.Util;

using System.Globalization;
using Newtonsoft.Json.Linq;
using Org.Json;
using Java.Net;

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
		private ValidationHelper ValidationHelper;

		private	enum PendingAction
		{	NONE,
			POST_PHOTO,
			POST_STATUS_UPDATE
		}

		/// <summary>
		/// Gets or sets the URI creator.
		/// </summary>
		/// <value>The URI creator.</value>
		public UriCreator UriCreator {
			get;
			set;
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

			UriCreator = new UriCreator (Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));

			uiHelper = new UiLifecycleHelper (this, callback);
			uiHelper.OnCreate (savedInstanceState);

			if (savedInstanceState != null) {
				string name = savedInstanceState.GetString (PENDING_ACTION_BUNDLE_KEY);
				pendingAction = (PendingAction)Enum.Parse (typeof(PendingAction), name);
			}

			SetContentView (Resource.Layout.SignIn);

			ValidationHelper = new ValidationHelper (this, GetValidationWarningDrawable());

			// Start of Facebook login stuff

			facebookLoginButton = (LoginButton)FindViewById (Resource.Id.facebooklogin);
			facebookLoginButton.UserInfoChangedCallback = new MyUserInfoChangedCallback (this);
			facebookLoginButton.SetBackgroundResource(Resource.Drawable.facebookloginrounded);
			facebookLoginButton.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0); 

			var perms = new List<string> {
				"email",
				"read_stream",
				"user_birthday"
			};

			facebookLoginButton.SetReadPermissions (perms);

			controlsContainer = (ViewGroup)FindViewById (Resource.Id.signincontainerinnerlayout);

			FragmentManager fm = FragmentManager;

			fm.BackStackChanged += delegate {
				if (fm.BackStackEntryCount == 0) {
					// We need to re-show our UI.
					controlsContainer.Visibility = ViewStates.Visible;
				}
			};

			// End of Facebook login stuff

			// Start of Singled Out login stuff

			var singledOutRegisterButtn =  (Button)FindViewById (Resource.Id.singledoutregister);
			singledOutRegisterButtn.Click += SingledOutRegistration;

			var singledOutLoginButtn =  (Button)FindViewById (Resource.Id.singledoutlogin);
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

		/// <summary>
		/// Backgrounds the state of the control.
		/// </summary>
		/// <param name="display">If set to <c>true</c> display.</param>
		protected void BackgroundControlState(bool display)
		{
			var signincontainerlayout = (RelativeLayout)FindViewById(Resource.Id.signincontainerlayout);
			var signincontainerinnerlayout = (RelativeLayout)FindViewById(Resource.Id.signincontainerinnerlayout);

			if (!display) {
				signincontainerlayout.SetBackgroundColor (Color.White);
				signincontainerinnerlayout.Visibility = ViewStates.Invisible;
			} else {
				signincontainerlayout.SetBackgroundColor (Color.ParseColor("#97C7DE"));
				signincontainerinnerlayout.Visibility = ViewStates.Visible;
			}
		}

		/// <summary>
		/// Singleds the out login.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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
				if (state == SessionState.Opened || state == SessionState.Opening) {
					owner.BackgroundControlState (false);
				}
			}
		}

		class MyUserInfoChangedCallback : Java.Lang.Object, LoginButton.IUserInfoChangedCallback
		{
			SignIn owner;
			ProgressDialog _dialog;
			UIHelper uiHelper;

			public MyUserInfoChangedCallback (SignIn owner)
			{
				this.owner = owner;
				uiHelper = new UIHelper ();
			}

			/// <summary>
			/// Gets the facebook photo.
			/// </summary>
			/// <returns>The facebook photo.</returns>
			/// <param name="facebookUserID">Facebook user I.</param>
			/// <param name="width">Width.</param>
			/// <param name="height">Height.</param>
			private HttpResponseMessage GetFacebookPhoto(string facebookUserID, int width, int height)
			{
				// Get the users photo.
				var uri = owner.UriCreator.FacebookPictureUrl (owner.Resources.GetString (Resource.String.facebookgraphuri),
					owner.Resources.GetString (Resource.String.facebookpicture),
					facebookUserID,
					width,
					height);

				var restHelper = new RestHelper ();

				// Save the details.
				var response = restHelper.GetAsync(uri);
				return response;
			}

			/// <summary>
			/// Raises the user info fetched event.
			/// </summary>
			/// <param name="user">User.</param>
			public async void OnUserInfoFetched (IGraphUser user)
			{
				owner.user = user;

				if (user != null) {
					if (Session.ActiveSession.IsOpened) 
					{					
						uiHelper.DisplayProgressDialog(owner, Resource.Style.CustomDialogTheme, "Logging in", "Please wait ...");

						var accessToken = Session.ActiveSession.AccessToken;
						var jsonFacebook = user.InnerJSONObject;
						var facebookAccessToken = owner.GetUserPreference ("FacebookAccessToken");
						var facebookPhotoUrl = string.Empty;

						// Get the facebook photo.
						var facebookPhotoTask = owner.FactoryStartNew<HttpResponseMessage> (() => GetFacebookPhoto(user.Id, 200, 200));

						if (facebookPhotoTask != null) {
							await facebookPhotoTask;

							if (facebookPhotoTask.Result.StatusCode == HttpStatusCode.OK) {
								// Get json from response message.
								var result = facebookPhotoTask.Result.Content.ReadAsStringAsync ().Result;
								JSONObject jsonPhoto = new JSONObject(result);
								if (jsonPhoto.Get("data") != null) {
									var jsonData = new JSONObject(jsonPhoto.Get("data").ToString());
									facebookPhotoUrl = jsonData.Get ("url").ToString();
								}
							}
						}				


						if (!string.IsNullOrEmpty (facebookAccessToken)) {
							// Change to Tutorial page.
							owner.SwipeLeftActivity = typeof(Tutorial1);
							owner.SwipeLeft ("SignIn");
						}

						var task = owner.FactoryStartNew<HttpResponseMessage> (() => SaveFacebookDetails (jsonFacebook, accessToken, facebookPhotoUrl));

						if (task != null) {
							// await so that this task will run in the background.
							await task;
							//var jsonUser = SaveFacebookDetails (json, accessToken);

							if (task.Result.StatusCode == HttpStatusCode.Created) {
								// Get json from response message.
								var result = task.Result.Content.ReadAsStringAsync ().Result;
								var json = JsonObject.Parse (result).ToString ().Replace ("{{", "{").Replace ("}}", "}");
								// Deserialize the Json.
								var returnUserModel = JsonConvert.DeserializeObject<UserModel> (json);

								if (string.IsNullOrEmpty (owner.GetUserPreference ("FacebookAccessToken"))) {
									owner.SetUserPreference ("FacebookAccessToken", accessToken);
									owner.SetUserPreference ("FacebookUsername", jsonFacebook.GetString ("id"));
									owner.SetUserPreference ("SingledOutUser", json);
									owner.AuthenticationToken = returnUserModel.AuthToken.ToString();
									owner.SetUserPreference ("FacebookPhoto", returnUserModel.FacebookPhotoUrl);
									owner.SetUserPreference ("UserID", returnUserModel.ID.ToString ());
									owner.AuthenticationToken = returnUserModel.AuthToken.ToString ();
								} 

								// Change to Tutorial page.
								owner.SwipeLeftActivity = typeof(Tutorial1);
								owner.SwipeLeft ("SignIn");
							} 
							else if (task.Result.StatusCode == HttpStatusCode.Forbidden) {
								owner.BackgroundControlState (true);
								// Logout from Facebook as the user already exists in the
								// database as a Singled Out user.
								Session.ActiveSession.CloseAndClearTokenInformation ();
								var lblValidation = (TextView)owner.FindViewById (Resource.Id.lblValidation);
								owner.ValidationHelper.SetValidationMessage (lblValidation, task.Result.ReasonPhrase);	
							
							}
							else {
								owner.BackgroundControlState (true);
								var lblValidation = (TextView)owner.FindViewById (Resource.Id.lblValidation);
								owner.ValidationHelper.SetValidationMessage (lblValidation, "Sorry, an error occurred!");					
							}
						}
					}
					// Stop progress indicator.
					uiHelper.HideProgressDialog ();
				}
			}

			/// <summary>
			/// Calculates the age.
			/// </summary>
			/// <returns>The age.</returns>
			/// <param name="dateOfBirth">Date of birth.</param>
			private int CalculateAge(DateTime dateOfBirth) 
			{ 
				int age = 0;
				age = DateTime.Now.Year - dateOfBirth.Year;
				if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear) 
					age = age - 1;

				return age;
			}

			/// <summary>
			/// Saves the facebook details.
			/// </summary>
			/// <param name="user">User.</param>
			/// <param name="facebookAccessToken">Facebook access token.</param>
			private HttpResponseMessage SaveFacebookDetails(Org.Json.JSONObject user, string facebookAccessToken, string facebookPhotoUrl)
			{
				var birthday = user.GetString ("birthday");
				int? age = null;
				if (!string.IsNullOrEmpty (birthday)) {
					var birthdate = DateTime.Parse (birthday, CultureInfo.InvariantCulture);
					age = CalculateAge (birthdate);
				}

				var userModel = new UserModel {
					FirstName = user.GetString("first_name"),
					Surname = user.GetString("last_name"),
					Sex = user.GetString("gender").ToLower(),
					CreatedDate = DateTime.UtcNow,
					FacebookAccessToken = facebookAccessToken,
					FacebookUserName = user.GetString("id"),
					UpdateDate = DateTime.UtcNow,
					Age = age > 0 ? (int)age : (int?)null,
					Email = user.GetString("email"),
					FacebookPhotoUrl = Android.Net.Uri.Encode(facebookPhotoUrl)
				};					

				// Instantiate a Uri Creator.
				var uri = owner.UriCreator.User (owner.Resources.GetString (Resource.String.apiurlusers));
				var restHelper = new RestHelper ();

				// Save the details.
				var response = restHelper.PostAsync(uri , userModel);
				return response;
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
