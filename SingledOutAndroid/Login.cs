
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
using MobileSpace.Helpers;
using SingledOut.SearchParameters;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Json;
using SingledOut.Model;
using Newtonsoft.Json;

namespace SingledOutAndroid
{
	[Activity (Label = "Login", NoHistory=true, Theme = "@android:style/Theme.NoTitleBar")]			
	public class Login : BaseActivity
	{
		private TextView _lblValidation;
		private ValidationHelper _validationHelper;
		private EditText _txtPassword;
		private EditText _txtEmail;
		private SecurityHelper _securityHelper;
		private TextView _lblForgottenPassword;
		ProgressBar _spinner;

		public Login ()
		{
			_securityHelper = new SecurityHelper ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Login);

			SwipeRightActivity = typeof(SignIn);

			_validationHelper = new ValidationHelper (this, GetValidationWarningDrawable());
			_txtEmail = FindViewById<EditText>(Resource.Id.txtEmailAddress);
			_txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			_lblForgottenPassword = FindViewById<TextView> (Resource.Id.lblForgottenPassword);
			_lblForgottenPassword.Click += ForgottenPasswordClick;

			var btnLogin = (Button)FindViewById (Resource.Id.btnLogin);
			btnLogin.Click += LoginClick;

			// Show welcome back message.
			if (LastActivity == "EmailRetrieved") {
				ShowNotificationBox ("Your password has been emailed to you.", true);
			}
		}

		/// <summary>
		/// Forgottens the password click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void ForgottenPasswordClick(object sender, EventArgs e)
		{
			SwipeRightActivity = typeof(ForgottenPassword);
			SwipeRight ();
		}

		/// <summary>
		/// Logins the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected async void LoginClick(object sender, EventArgs e)
		{
			_lblValidation = FindViewById<TextView>(Resource.Id.lblValidation);
			_validationHelper.SetValidationMessage (_lblValidation, string.Empty);

			// Do validation.
			var isValid = Validate();
			if(!isValid)
			{
				return;
			}

			// Start progress indicator.
			_spinner = (ProgressBar)FindViewById(Resource.Id.progressSpinner);
			_spinner.Visibility = ViewStates.Visible;

			try
			{
				// Create task to login to Singled Out.
				var task = FactoryStartNew (() => LoginToSingledOut (_txtEmail.Text, _txtPassword.Text));
				if(task != null)
				{
					// await so that this task will run in the background.
					await task;

					// Return here after login has completed.
					if(task.Result.StatusCode == HttpStatusCode.Accepted)
					{
						// Get json from response message.
						var result =  task.Result.Content.ReadAsStringAsync().Result;
						var json = JsonObject.Parse(result).ToString().Replace("{{", "{").Replace("}}","}");
						// Deserialize the Json.
						var returnUserModel = JsonConvert.DeserializeObject<UserModel>(json);

						// Save the user preference for the user name and id.
						if (string.IsNullOrEmpty(GetUserPreference ("SingledOutEmail"))) {
							SetUserPreference ("SingledOutEmail", returnUserModel.Email);
							SetUserPreference ("SingledOutUser", json);
						} 				

						SwipeLeftActivity = typeof(CheckIn);
						SwipeLeft("Login");
					}
					else if (task.Result.StatusCode == HttpStatusCode.Unauthorized)
					{
						_validationHelper.SetValidationMessage (_lblValidation, "Email or password does not exist.");
					}
				}
			}
			catch (Exception)
			{
				_validationHelper.SetValidationMessage (_lblValidation, "An unknown error occurred!");			
				_spinner.Visibility = ViewStates.Gone;
			}

			// Stop progress indicator.
			_spinner.Visibility = ViewStates.Gone;	
		}

		/// <summary>
		/// Logins to singled out.
		/// </summary>
		/// <returns>The to singled out.</returns>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		private HttpResponseMessage LoginToSingledOut(string username, string password)
		{
			var uri = string.Concat (Resources.GetString (Resource.String.apiurlaccount),"/", Resources.GetString (Resource.String.apiurllogin));
			var response = RestHelper.Login (uri, username, password);
			return response;
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		public bool Validate()
		{
			var isValid = true;

			// Check email length
			if (!_validationHelper.ValidateEditTextEmailAddress (_txtEmail)) {
				return false;
			}

			// Check password strength.
			if (!_validationHelper.ValidateEditTextRequired (_txtPassword, "Password")) {
				return false;
			}

			return isValid;
		}
	}
}

