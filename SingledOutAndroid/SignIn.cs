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
using Android.Widget;
using Xamarin.FacebookBinding;
using Xamarin.FacebookBinding.Model;
using Xamarin.FacebookBinding.Widget;
using Android.Content.PM;
using Java.Security;
using System.Json;
using SingledOut.Model;
using CSS.Helpers;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Text.RegularExpressions;
using Android.Graphics.Drawables;
using System.Linq;

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
		private CSS.Helpers.RestHelper _restHelper;
		private PendingAction pendingAction = PendingAction.NONE;
		private ViewGroup controlsContainer;
		private IGraphUser user;
		private UiLifecycleHelper uiHelper;
		private Session.IStatusCallback callback;
		private SecurityHelper _securityHelper;
		AlertDialog _dialog;
		ProgressBar _spinner;
		private EditText _txtFirstName;
		private EditText _txtSurname;
		private EditText _txtUsername;
		private EditText _txtPassword;
		private RadioGroup _rbGender;
		private Drawable _warning;
		private TextView _lblValidation;

		private	enum PendingAction
		{	NONE,
			POST_PHOTO,
			POST_STATUS_UPDATE
		}

		public SignIn ()
		{
			callback = new MyStatusCallback (this);
			_restHelper = new CSS.Helpers.RestHelper ();
			_securityHelper = new SecurityHelper ();
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

			var singledOutLoginButtn =  (ImageButton)FindViewById (Resource.Id.singledoutlogin);
			singledOutLoginButtn.Click += SingledOutRegistration;

			// End of Singled Out login stuff	

		}

		/// <summary>
		/// Validates an Edit Text.
		/// </summary>
		/// <returns><c>true</c>, if first name was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		private bool ValidateEditTextRequired(Drawable warning, EditText editText, string editTextName)
		{
			// Check first name is not empty.
			if (string.IsNullOrEmpty (editText.Text)) {
				editText.SetError (string.Format("{0} is required.", editTextName), warning);
				editText.RequestFocus();
				ShowKeyboard (editText);
				return false;
			} else {
				editText.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates an Edit Text Minimum Length.
		/// </summary>
		/// <returns><c>true</c>, if first name was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		private bool ValidateEditTextMinimumLength(Drawable warning, EditText editText, int minLength, string editTextName)
		{
			// Check first name is not empty.
			if (editText.Text.Length < minLength) {
				editText.SetError (string.Format("{0} must be at least {1} letters.", editTextName, minLength), warning);
				editText.RequestFocus();
				ShowKeyboard (editText);
				return false;
			} else {
				editText.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates the edit text password strength.
		/// </summary>
		/// <returns><c>true</c>, if edit text password strength was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		/// <param name="editText">Edit text.</param>
		private bool ValidateEditTextPasswordStrength(Drawable warning, EditText editText)
		{
			// Check first name is not empty.
			if (editText.Text.Length < 6 || !editText.Text.Any(char.IsDigit)) {
				editText.SetError ("Password must contain minimum of 6 letters and one number.", warning);
				editText.RequestFocus ();
				ShowKeyboard (editText);
				return false;
			} else {
				editText.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates an Edit Text Password.
		/// </summary>
		/// <returns><c>true</c>, if first name was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		private bool ValidateRadioButtonSelectionRequired(Drawable warning, RadioGroup radioGroup, string name, int radioBtnToSetErrorOn)
		{
			// Check first name is not empty.
			var radioButton = _dialog.FindViewById<RadioButton> (radioBtnToSetErrorOn);
			if (radioGroup.CheckedRadioButtonId == -1) {
				var errorMessage = string.Format("{0} is required",name);
				radioButton.SetError (errorMessage, _warning);
				return false;
			} else {
				radioButton.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		public bool Validate()
		{
			var isValid = true;

			if (!ValidateEditTextRequired (_warning, _txtFirstName, "First name")) {
				return false;
			}

			// Check surname name is not empty.
			if (!ValidateEditTextRequired (_warning, _txtSurname, "Surname")) {
				return false;
			}

			// Check username length
			if (!ValidateEditTextMinimumLength (_warning, _txtUsername, 6, "User name")) {
				return false;
			}

			// Check password strength.
			if (!ValidateEditTextPasswordStrength (_warning, _txtPassword)) {
				return false;
			}

			// Check gender is selected.
			if (!ValidateRadioButtonSelectionRequired (_warning, _rbGender, "Gender", Resource.Id.radio_female)) {
				return false;
			}

			return isValid;
		}

		/// <summary>
		/// Singleds the out registration.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SingledOutRegistration(object sender, EventArgs e)
		{		
			var registerDialog = new AlertDialog.Builder(this);
			using (var alert = new AlertDialog.Builder (this)) {
				alert.SetView (LayoutInflater.Inflate(Resource.Layout.RegistrationDialog, null));
				alert.SetCancelable (false);
				_dialog = alert.Create ();
				_dialog.SetTitle("Singled Out Registration");
				_dialog.SetIcon(Resource.Drawable.logopindialog);
				_dialog.SetCanceledOnTouchOutside(true);
				_dialog.Show();

				Button btnCreateAccount = (Button)_dialog.FindViewById (Resource.Id.btnCreateAccount);
				btnCreateAccount.Click += CreateAccountClick;

				// Wire up validation control listeners.
				_warning = (Drawable)Resources.GetDrawable(Resource.Drawable.exclamation);
				_warning.SetBounds(0, 0, _warning.IntrinsicWidth/3, _warning.IntrinsicHeight/3);

				_txtFirstName = _dialog.FindViewById<EditText>(Resource.Id.txtFirstName);
				_txtSurname = _dialog.FindViewById<EditText>(Resource.Id.txtSurname);
				_txtUsername = _dialog.FindViewById<EditText>(Resource.Id.txtUsername);
				_txtPassword = _dialog.FindViewById<EditText>(Resource.Id.txtPassword);
				_rbGender = _dialog.FindViewById<RadioGroup>(Resource.Id.rbGender);

				_txtFirstName.TextChanged += TextChangedRequiredValidation;
				_txtSurname.TextChanged += TextChangedRequiredValidation;
				_txtUsername.TextChanged += TextChangedMinimumLengthValidation;
				_txtPassword.TextChanged += TextChangedPasswordStrength;				
				_rbGender.CheckedChange += CheckChangedRequired;
			}
		}

		protected void CheckChangedRequired(object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			ValidateRadioButtonSelectionRequired (_warning, ((RadioGroup)sender), "Gender", Resource.Id.radio_female);
		}

		protected void TextChangedPasswordStrength(object sender, Android.Text.TextChangedEventArgs e)
		{
			var editText = ((EditText)sender);
			if(e.Text.Count() > 0 && editText.Error != null)
			{
				ValidateEditTextPasswordStrength (_warning, editText);
			}
		}

		protected void TextChangedMinimumLengthValidation(object sender, Android.Text.TextChangedEventArgs e)
		{
			var editText = ((EditText)sender);
			if(e.Text.Count() > 0 && editText.Error != null)
			{
				ValidateEditTextMinimumLength (_warning, editText, 6, editText.Hint);
			}
		}

		protected void TextChangedRequiredValidation(object sender, Android.Text.TextChangedEventArgs e)
		{
			var editText = ((EditText)sender);
			if(e.Text.Count() > 0 && editText.Error != null)
			{
				ValidateEditTextRequired(_warning, editText, editText.Hint);
			}
		}

		/// <summary>
		/// Creates the account click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void CreateAccountClick (object sender, EventArgs e)
		{
			_lblValidation = _dialog.FindViewById<TextView>(Resource.Id.lblValidation);
			SetValidationMessage (string.Empty);

			// Do validation.
			var isValid = Validate();
			if(!isValid)
			{
				return;
			}

			// Returns an integer which represents the selected radio button's ID
			int selected = _rbGender.CheckedRadioButtonId;
			// Gets a reference to the "selected" radio button
			RadioButton rb = (RadioButton)_dialog.FindViewById(selected);

			// Create a UserModel from the form fields.
			var userModel = new UserModel {
				FirstName =  _txtFirstName.Text,
				Surname = _txtSurname.Text,
				Sex = rb.Text,
				CreatedDate = DateTime.UtcNow,
				Username = _txtUsername.Text,
				Password = !string.IsNullOrEmpty(_txtPassword.Text) ? _securityHelper.CreateHash(_txtPassword.Text) : string.Empty,
				UpdateDate = DateTime.UtcNow
			};	

			// Start progress indicator.
			_spinner = (ProgressBar)_dialog.FindViewById(Resource.Id.progressSpinner);
			_spinner.Visibility = ViewStates.Visible;

			try
			{
				// Create task to Save Singled Out Details.
				var task = Task<HttpResponseMessage>.Factory.StartNew (() => SaveSingledOutDetails (userModel));
				// await so that this task will run in the background.
				await task;
			
				// Return here after SaveSingledOutDetails has completed.
				if(task.Result.StatusCode == HttpStatusCode.Created)
				{
					// Get json from response message.
					var result =  task.Result.Content.ReadAsStringAsync().Result;
					var json = JsonObject.Parse(result).ToString().Replace("{{", "{").Replace("}}","}");
					// Deserialize the Json.
					var returnUserModel = _restHelper.DeserializeObject<UserModel>(json);

					// Save the user preference for the user name and id.
					if (string.IsNullOrEmpty(GetUserPreference ("SingledOutUsername"))) {
						SetUserPreference ("SingledOutUsername", returnUserModel.Username);
						SetUserPreference ("SingledOutID", returnUserModel.ID.ToString());
					} 

					var toast = Toast.MakeText (this, "Account Created!", ToastLength.Long);
					toast.SetGravity (GravityFlags.Center | GravityFlags.Center, 0, 0);
					toast.Show ();
					_dialog.Dismiss ();
					StartActivity (typeof(Tutorial1));
					OverridePendingTransition (Resource.Drawable.slide_in_left, Resource.Drawable.slide_out_left);
				}
				else if(task.Result.StatusCode == HttpStatusCode.Forbidden)
				{
					// need to update on the main thread to change the border color
					SetValidationMessage (task.Result.ReasonPhrase);
				}
			}
			catch (Exception)
			{
				SetValidationMessage ("An unknown error occurred!");			
				_spinner.Visibility = ViewStates.Gone;
			}

			// Stop progress indicator.
			_spinner.Visibility = ViewStates.Gone;
		}

		/// <summary>
		/// Sets the validation message.
		/// </summary>
		/// <param name="message">Message.</param>
		private void SetValidationMessage(string message)
		{
			_lblValidation.Visibility = string.IsNullOrEmpty(message) ? ViewStates.Gone : ViewStates.Visible;
			_lblValidation.Text = message;
		}

		/// <summary>
		/// Saves the singled out details.
		/// </summary>
		/// <param name="user">User.</param>
		private HttpResponseMessage SaveSingledOutDetails(UserModel user)
		{
			var restHelper = new CSS.Helpers.RestHelper ();
			var url = string.Concat(this.GetString(Resource.String.apihost), this.GetString(Resource.String.apipath), this.GetString(Resource.String.apiurlusers));
			return restHelper.PostAsync(url , user);
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

				var restHelper = new CSS.Helpers.RestHelper ();
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
