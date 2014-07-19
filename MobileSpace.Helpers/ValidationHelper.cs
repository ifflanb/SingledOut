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
using Android;
using Android.Content.Res;

namespace MobileSpace.Helpers
{
	public class ValidationHelper
	{
		private UIHelper _uiHelper;
		private Activity _activity;
		private Drawable _warning;

		public ValidationHelper (Activity activity, Drawable warning)
		{
			_uiHelper = new UIHelper ();
			_activity = activity;
			_warning = warning;
		}
		/// <summary>
		/// Validates the edit text email address.
		/// </summary>
		/// <returns><c>true</c>, if edit text email address was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		/// <param name="editText">Edit text.</param>
		public bool ValidateEditTextEmailAddress(EditText editText) 
		{
			var expression = "^[\\w\\.-]+@([\\w\\-]+\\.)+[A-Z]{2,4}$";
			var inputStr = editText.Text;
			var pattern = Java.Util.Regex.Pattern.Compile(expression, Java.Util.Regex.RegexOptions.CaseInsensitive);
			var matcher = pattern.Matcher(inputStr);
			if (!matcher.Matches()) 
			{
				editText.SetError ("Invalid Email Address.", _warning);
				editText.RequestFocus();
				_uiHelper.ShowKeyboard (editText, _activity);
				return false;
			}
			else {
				editText.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates the edit text repeat password.
		/// </summary>
		/// <returns><c>true</c>, if edit text repeat password was validated, <c>false</c> otherwise.</returns>
		/// <param name="password">Password.</param>
		/// <param name="repeatPassword">Repeat password.</param>
		public bool ValidateEditTextRepeatPassword(EditText password, EditText repeatPassword)
		{
			// Check first name is not empty.
			if (password.Text != repeatPassword.Text) {
				repeatPassword.SetError ("Re-entered password must match password.", _warning);
				repeatPassword.RequestFocus ();
				_uiHelper.ShowKeyboard (repeatPassword, _activity);
				return false;
			} else {
				repeatPassword.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates the edit text password strength.
		/// </summary>
		/// <returns><c>true</c>, if edit text password strength was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		/// <param name="editText">Edit text.</param>
		public bool ValidateEditTextPasswordStrength(EditText editText)
		{
			// Check first name is not empty.
			if (editText.Text.Length < 6 || !editText.Text.Any(char.IsDigit)) {
				editText.SetError ("Password must contain minimum of 6 letters and one number.", _warning);
				editText.RequestFocus ();
				_uiHelper.ShowKeyboard (editText, _activity);
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
		public bool ValidateRadioButtonSelectionRequired(RadioGroup radioGroup, string name, int radioBtnToSetErrorOn)
		{
			// Check first name is not empty.
			var radioButton = _activity.FindViewById<RadioButton> (radioBtnToSetErrorOn);
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
		/// Validates the checkbox selection required.
		/// </summary>
		/// <returns><c>true</c>, if checkbox selection required was validated, <c>false</c> otherwise.</returns>
		/// <param name="checkBox">Check box.</param>
		/// <param name="name">Name.</param>
		/// <param name="controlToSetErrorOn">Control to set error on.</param>
		public bool ValidateCheckboxSelectionRequired(CheckBox checkBox, string name, int controlToSetErrorOn)
		{
			// Check first name is not empty.
			var errorControl = _activity.FindViewById<TextView> (controlToSetErrorOn);
			if (!checkBox.Checked) {
				var errorMessage = string.Format("{0} is required",name);
				errorControl.SetError (errorMessage, _warning);
				return false;
			} else {
				errorControl.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates the spinner required.
		/// </summary>
		/// <returns><c>true</c>, if spinner required was validated, <c>false</c> otherwise.</returns>
		/// <param name="spinner">Spinner.</param>
		/// <param name="name">Name.</param>
		/// <param name="controlToSetErrorOn">Control to set error on.</param>
		public bool ValidateSpinnerRequired(Spinner spinner, string name, int controlToSetErrorOn)
		{
			// Check first name is not empty.
			var errorControl = _activity.FindViewById<Spinner> (controlToSetErrorOn);
			if (spinner.SelectedItemPosition <= 0) {
				var errorMessage = string.Format("{0} is required",name);
				//errorControl.SetError (errorMessage, _warning);
				return false;
			} else {
				//errorControl.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Validates an Edit Text.
		/// </summary>
		/// <returns><c>true</c>, if first name was validated, <c>false</c> otherwise.</returns>
		/// <param name="warning">Warning.</param>
		public bool ValidateEditTextRequired(EditText editText, string editTextName)
		{
			// Check first name is not empty.
			if (string.IsNullOrEmpty (editText.Text)) {
				editText.SetError (string.Format("{0} is required.", editTextName), _warning);
				editText.RequestFocus();
				_uiHelper.ShowKeyboard (editText, _activity);
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
		public bool ValidateEditTextMinimumLength(EditText editText, int minLength, string editTextName)
		{
			// Check first name is not empty.
			if (editText.Text.Length < minLength) {
				editText.SetError (string.Format("{0} must be at least {1} letters.", editTextName, minLength), _warning);
				editText.RequestFocus();
				_uiHelper.ShowKeyboard (editText, _activity);
				return false;
			} else {
				editText.Error = null;
				return true;
			}
		}

		/// <summary>
		/// Sets the validation message.
		/// </summary>
		/// <param name="message">Message.</param>
		public void SetValidationMessage(TextView validationMessageControl, string message)
		{
			validationMessageControl.Visibility = string.IsNullOrEmpty(message) ? ViewStates.Gone : ViewStates.Visible;
			validationMessageControl.Text = message;
		}
	}
}

