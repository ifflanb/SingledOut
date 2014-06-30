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
using System.Net.Http;
using System.Net;

namespace SingledOutAndroid
{
	[Activity (Label = "Forgotten Password", Theme = "@android:style/Theme.NoTitleBar")]			
	public class ForgottenPassword : BaseActivity
	{
		private TextView _lblValidation;
		private EditText _txtEmail;
		private ValidationHelper _validationHelper;
		ProgressBar _spinner;
		private UriCreator _uriCreator;
		private RestHelper _restHelper;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ForgottenPassword);

			_restHelper = new RestHelper ();
			_uriCreator = new UriCreator(Resources.GetString(Resource.String.apihost), Resources.GetString(Resource.String.apipath));
			_validationHelper = new ValidationHelper (this, GetValidationWarningDrawable());
			_txtEmail = FindViewById<EditText>(Resource.Id.txtEmailAddress);
			var btnForgottenPassword = FindViewById<Button> (Resource.Id.btnForgottenPassword);
			btnForgottenPassword.Click += ForgottenPasswordClick;

		}

		protected async void ForgottenPasswordClick(object sender, EventArgs e)
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
				// Create task to get lost password.
				var task = FactoryStartNew (() => RetrieveLostPassword (_txtEmail.Text));
				if(task != null)
				{
					// await so that this task will run in the background.
					await task;

					// Return here after login has completed.
					if(task.Result.StatusCode == HttpStatusCode.OK)
					{										
						SwipeLeftActivity = typeof(Login);
						SwipeLeft("EmailRetrieved");
					}
					else
					{
						_validationHelper.SetValidationMessage (_lblValidation, "Email does not exist.");
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
		/// Retrieves the lost password.
		/// </summary>
		/// <returns>The lost password.</returns>
		/// <param name="email">Email.</param>
		private HttpResponseMessage RetrieveLostPassword(string email)
		{
			var retrievePasswordUri = string.Concat (Resources.GetString (Resource.String.apiurlaccount),"/", Resources.GetString (Resource.String.apiurllostpassword));
			var uri = _uriCreator.RetrievePassword (retrievePasswordUri, email);
			var response = _restHelper.RetrievePassword (uri);
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

			return isValid;
		}
	}
}

