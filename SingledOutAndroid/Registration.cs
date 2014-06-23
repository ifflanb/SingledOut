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
using MobileSpace.Helpers;
using Java.Util.Regex;
using Newtonsoft.Json;

namespace SingledOutAndroid
{
	[Activity (Label = "Register", Theme = "@android:style/Theme.NoTitleBar")]	
	public class Registration : BaseActivity
	{
		ProgressBar _spinner;
		private EditText _txtFirstName;
		private EditText _txtSurname;
		private EditText _txtEmail;
		private EditText _txtPassword;
		private EditText _txtRepeatPassword;
		private RadioGroup _rbGender;
		private TextView _lblValidation;
		private CheckBox _ckTerms;
		private TextView _lblTerms;
		private SecurityHelper _securityHelper;
		private ValidationHelper _validationHelper;

		public Registration ()
		{
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
			_txtRepeatPassword = FindViewById<EditText>(Resource.Id.txtRepeatPassword);
			_rbGender = FindViewById<RadioGroup>(Resource.Id.rbGender);
			_ckTerms = FindViewById<CheckBox>(Resource.Id.chkTermsCondition);
			_lblTerms = FindViewById<TextView> (Resource.Id.lblTermsConditions);
			_lblTerms.Click += TermsClick;
			_lblTerms.MovementMethod = new LinkMovementMethod ();

			// Restore and saved state.
			RestoreState ();

			_txtFirstName.TextChanged += TextChangedRequiredValidation;
			_txtSurname.TextChanged += TextChangedRequiredValidation;
			_txtEmail.TextChanged += TextChangedMinimumLengthValidation;
			_txtPassword.TextChanged += TextChangedPasswordStrength;		
			_txtRepeatPassword.TextChanged += TextChangedPasswordStrength;	
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
				var task = FactoryStartNew (() => SaveSingledOutDetails (userModel));
				if(task != null)
				{
					// await so that this task will run in the background.
					await task;

					// Return here after SaveSingledOutDetails has completed.
					if(task.Result.StatusCode == HttpStatusCode.Created)
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

						SwipeLeftActivity = typeof(Tutorial1);
						SwipeLeft("Registration");
					}
					else if(task.Result.StatusCode == HttpStatusCode.Forbidden)
					{
						// need to update on the main thread to change the border color
						_validationHelper.SetValidationMessage (_lblValidation, task.Result.ReasonPhrase);
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

		public void TermsClick(Object sender, EventArgs e) 
		{
			// Save the registration form state so that when the user returns 
			// the fields are still filled out.
			SaveState ();
			SwipeLeftActivity = typeof(TermsConditions);
			SwipeLeft ();
		} 

		/// <summary>
		/// Saves the state.
		/// </summary>
		private void SaveState() {
			SetUserPreference("Reg_Firstname", !string.IsNullOrEmpty(_txtFirstName.Text)? _txtFirstName.Text:string.Empty);
			SetUserPreference("Reg_Surname", !string.IsNullOrEmpty(_txtSurname.Text)?_txtSurname.Text:string.Empty);
			SetUserPreference("Reg_Email", !string.IsNullOrEmpty(_txtEmail.Text)?_txtEmail.Text:string.Empty);
			SetUserPreference("Reg_Password", !string.IsNullOrEmpty(_txtPassword.Text)?_txtPassword.Text:string.Empty);
			SetUserPreference("Reg_RepeatPassword", !string.IsNullOrEmpty(_txtRepeatPassword.Text)?_txtRepeatPassword.Text:string.Empty);
			SetUserPreference("Reg_GenderMale", _rbGender.CheckedRadioButtonId == Resource.Id.radio_male?"1":"-1");
			SetUserPreference("Reg_GenderFemale", _rbGender.CheckedRadioButtonId == Resource.Id.radio_female?"1":"-1");
		}

		/// <summary>
		/// Clears the state.
		/// </summary>
		private void ClearState() {
			SetUserPreference ("Reg_Firstname", string.Empty);
			SetUserPreference("Reg_Surname", string.Empty);
			SetUserPreference("Reg_Email", string.Empty);
			SetUserPreference("Reg_Password", string.Empty);
			SetUserPreference("Reg_RepeatPassword", string.Empty);
			SetUserPreference("Reg_GenderMale", string.Empty);
			SetUserPreference("Reg_GenderFemale", string.Empty);
		}

		/// <summary>
		/// Restores the state of the instance.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		private void RestoreState()
		{
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_Firstname"))) {
				_txtFirstName.Text = GetUserPreference("Reg_Firstname");
			}
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_Surname"))) {
				_txtSurname.Text = GetUserPreference ("Reg_Surname");
			}
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_Email"))) {
				_txtEmail.Text = GetUserPreference ("Reg_Email");
			}
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_Password"))) {
				_txtPassword.Text = GetUserPreference ("Reg_Password");
			}
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_RepeatPassword"))) {
				_txtRepeatPassword.Text = GetUserPreference ("Reg_RepeatPassword");
			}
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_GenderMale")) && GetUserPreference ("Reg_GenderMale") != "-1") {
				if (GetUserPreference ("Reg_GenderMale") == "1") {
					_rbGender.Check (Resource.Id.radio_male);
				}
			}
			if (!string.IsNullOrEmpty(GetUserPreference ("Reg_GenderFemale")) && GetUserPreference ("Reg_GenderFemale") != "-1") {
				if (GetUserPreference ("Reg_GenderFemale") == "1") {
					_rbGender.Check (Resource.Id.radio_female);
				}
			}

			// Clear all the state fields.
			ClearState ();
		}

		/// <summary>
		/// Saves the singled out details.
		/// </summary>
		/// <param name="user">User.</param>
		private HttpResponseMessage SaveSingledOutDetails(UserModel user)
		{
			var uri = string.Concat (Resources.GetString (Resource.String.apiurlusers));
			return RestHelper.PostAsync(uri , user);
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

		protected void TextChangedRepeatPassword(object sender, Android.Text.TextChangedEventArgs e)
		{
			var repeatPassword = ((EditText)sender);
			var password = _txtPassword;
			if(e.Text.Count() > 0 && repeatPassword.Error != null)
			{
				_validationHelper.ValidateEditTextRepeatPassword (password, repeatPassword);
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

			// Check repeat password.
			if (!_validationHelper.ValidateEditTextRepeatPassword (_txtPassword, _txtRepeatPassword)) {
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

