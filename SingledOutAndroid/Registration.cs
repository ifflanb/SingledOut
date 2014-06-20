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
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Method;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using SingledOut.Model;
using CSS.Helpers;
using Java.Util.Regex;

namespace SingledOutAndroid
{
	[Activity (NoHistory=true, Theme = "@android:style/Theme.NoTitleBar")]			
	public class Registration : BaseActivity
	{
		ProgressBar _spinner;
		private EditText _txtFirstName;
		private EditText _txtSurname;
		private EditText _txtEmail;
		private EditText _txtPassword;
		private RadioGroup _rbGender;
		private TextView _lblValidation;
		private CheckBox _ckTerms;
		private TextView _lblTerms;
		private RestHelper _restHelper;
		private SecurityHelper _securityHelper;
		private ValidationHelper _validationHelper;

		public Registration ()
		{
			_restHelper = new RestHelper ();
			_securityHelper = new SecurityHelper ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Registration);

			SwipeRightActivity = typeof(SignIn);

			Button btnCreateAccount = (Button)FindViewById (Resource.Id.btnCreateAccount);
			btnCreateAccount.Click += CreateAccountClick;

			_validationHelper = new ValidationHelper (this, GetValidationWarningDrawable());
			_txtFirstName = FindViewById<EditText>(Resource.Id.txtFirstName);
			_txtSurname = FindViewById<EditText>(Resource.Id.txtSurname);
			_txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			_txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			_rbGender = FindViewById<RadioGroup>(Resource.Id.rbGender);
			_ckTerms = FindViewById<CheckBox>(Resource.Id.chkTermsCondition);
			_lblTerms = FindViewById<TextView> (Resource.Id.lblTermsConditions);
			_lblTerms.Click += TermsClick;
			_lblTerms.MovementMethod = new LinkMovementMethod ();

			_txtFirstName.TextChanged += TextChangedRequiredValidation;
			_txtSurname.TextChanged += TextChangedRequiredValidation;
			_txtEmail.TextChanged += TextChangedMinimumLengthValidation;
			_txtPassword.TextChanged += TextChangedPasswordStrength;				
			_rbGender.CheckedChange += CheckChangedRequired;
			_ckTerms.CheckedChange += CheckboxCheckChangedRequired;
		}

		/// <summary>
		/// Creates the account click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void CreateAccountClick (object sender, EventArgs e)
		{
			_lblValidation = FindViewById<TextView>(Resource.Id.lblValidation);
			_validationHelper.SetValidationMessage (_lblValidation, string.Empty);

			// Do validation.
			var isValid = Validate();
			if(!isValid)
			{
				return;
			}

			// Returns an integer which represents the selected radio button's ID
			int selected = _rbGender.CheckedRadioButtonId;
			// Gets a reference to the "selected" radio button
			RadioButton rb = (RadioButton)FindViewById(selected);

			// Create a UserModel from the form fields.
			var userModel = new UserModel {
				FirstName =  _txtFirstName.Text,
				Surname = _txtSurname.Text,
				Sex = rb.Text,
				CreatedDate = DateTime.UtcNow,
				Email = _txtEmail.Text,
				Password = !string.IsNullOrEmpty(_txtPassword.Text) ? _securityHelper.CreateHash(_txtPassword.Text) : string.Empty,
				UpdateDate = DateTime.UtcNow
			};	

			// Start progress indicator.
			_spinner = (ProgressBar)FindViewById(Resource.Id.progressSpinner);
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
					if (string.IsNullOrEmpty(GetUserPreference ("SingledOutEmail"))) {
						SetUserPreference ("SingledOutEmail", returnUserModel.Email);
						SetUserPreference ("SingledOutID", returnUserModel.ID.ToString());
					} 

					var toast = Toast.MakeText (this, "Account Created!", ToastLength.Long);
					toast.SetGravity (GravityFlags.Center | GravityFlags.Center, 0, 0);
					toast.Show ();

					SwipeLeftActivity = typeof(Tutorial1);
					SwipeLeft();
				}
				else if(task.Result.StatusCode == HttpStatusCode.Forbidden)
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

		public void TermsClick(Object sender, EventArgs e) 
		{
			SwipeLeftActivity = typeof(TermsConditions);
			SwipeLeft ();
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

		protected void CheckboxCheckChangedRequired(object sender, CheckBox.CheckedChangeEventArgs e)
		{
			_validationHelper.ValidateCheckboxSelectionRequired (((CheckBox)sender), "Terms and Conditions", Resource.Id.lblTermsConditions);
		}

		protected void CheckChangedRequired(object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			_validationHelper.ValidateRadioButtonSelectionRequired (((RadioGroup)sender), "Gender", Resource.Id.radio_female);
		}

		protected void TextChangedPasswordStrength(object sender, Android.Text.TextChangedEventArgs e)
		{
			var editText = ((EditText)sender);
			if(e.Text.Count() > 0 && editText.Error != null)
			{
				_validationHelper.ValidateEditTextPasswordStrength (editText);
			}
		}

		protected void TextChangedMinimumLengthValidation(object sender, Android.Text.TextChangedEventArgs e)
		{
			var editText = ((EditText)sender);
			if(e.Text.Count() > 0 && editText.Error != null)
			{
				_validationHelper.ValidateEditTextMinimumLength (editText, 6, editText.Hint);
			}
		}

		protected void TextChangedRequiredValidation(object sender, Android.Text.TextChangedEventArgs e)
		{
			var editText = ((EditText)sender);
			if(e.Text.Count() > 0 && editText.Error != null)
			{
				_validationHelper.ValidateEditTextRequired(editText, editText.Hint);
			}
		}


		/// <summary>
		/// Validate this instance.
		/// </summary>
		public bool Validate()
		{
			var isValid = true;

			if (!_validationHelper.ValidateEditTextRequired (_txtFirstName, "First name")) {
				return false;
			}

			// Check surname name is not empty.
			if (!_validationHelper.ValidateEditTextRequired (_txtSurname, "Surname")) {
				return false;
			}

			// Check email length
			if (!_validationHelper.ValidateEditTextEmailAddress (_txtEmail)) {
				return false;
			}

			// Check password strength.
			if (!_validationHelper.ValidateEditTextPasswordStrength (_txtPassword)) {
				return false;
			}

			// Check gender is selected.
			if (!_validationHelper.ValidateRadioButtonSelectionRequired (_rbGender, "Gender", Resource.Id.radio_female)) {
				return false;
			}

			if (!_validationHelper.ValidateCheckboxSelectionRequired (_ckTerms, "Terms and Conditions", Resource.Id.lblTermsConditions)) {
				return false;
			}

			return isValid;
		}
	}
}

