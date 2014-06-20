
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
using CSS.Helpers;
using SingledOut.SearchParameters;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Json;
using SingledOut.Model;

namespace SingledOutAndroid
{
	[Activity (NoHistory=true, Theme = "@android:style/Theme.NoTitleBar")]			
	public class Login : BaseActivity
	{
		private TextView _lblValidation;
		private RestHelper _restHelper;
		private ValidationHelper _validationHelper;
		private EditText _txtPassword;
		private EditText _txtEmail;
		private SecurityHelper _securityHelper;
		ProgressBar _spinner;

		public Login ()
		{
			_restHelper = new RestHelper();
			_securityHelper = new SecurityHelper ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Login);

			SwipeRightActivity = typeof(SignIn);

			_validationHelper = new ValidationHelper (this, GetValidationWarningDrawable());
			_txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			_txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);

			var btnLogin = (Button)FindViewById (Resource.Id.btnLogin);
			btnLogin.Click += LoginClick;
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

			var sp = new UsersSearchParameters {
				Email = _txtEmail.Text,
				Password = _securityHelper.CreateHash(_txtPassword.Text)
			};


			// Start progress indicator.
			_spinner = (ProgressBar)FindViewById(Resource.Id.progressSpinner);
			_spinner.Visibility = ViewStates.Visible;

			try
			{
				// Create task to login to Singled Out.
				var task = Task<HttpResponseMessage>.Factory.StartNew (() => LoginToSingledOut (sp));
				// await so that this task will run in the background.
				await task;

				// Return here after login has completed.
				if(task.Result.StatusCode == HttpStatusCode.OK)
				{
					// Get json from response message.
					var result =  task.Result.Content.ReadAsStringAsync().Result;
					var json = JsonObject.Parse(result).ToString().Replace("{{", "{").Replace("}}","}");
					// Deserialize the Json.
					var returnUserModel = _restHelper.DeserializeObject<UserModel>(json);

					// Save the user preference for the user name and id.
					if (string.IsNullOrEmpty(GetUserPreference ("SingledOutEmail"))) {
						SetUserPreference ("SingledOutEmail", returnUserModel.Email);
						SetUserPreference ("SingledOutID", returnUserModel.ID.ToString());
					} 

					var toast = Toast.MakeText (this, "Login Successful!", ToastLength.Long);
					toast.SetGravity (GravityFlags.Center | GravityFlags.Center, 0, 0);
					toast.Show ();

					SwipeLeftActivity = typeof(CheckIn);
					SwipeLeft();
				}
				else 
				{
					// need to update on the main thread to change the border color
					_validationHelper.SetValidationMessage (_lblValidation, task.Result.ReasonPhrase);
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

		private HttpResponseMessage LoginToSingledOut(UsersSearchParameters sp)
		{
			var uri = UriCreator.Build(sp).ToString();
			var response = _restHelper.SearchAsync (uri);
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

