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
using Android.Views.InputMethods;
using Android.Views.Animations;

namespace MobileSpace.Helpers
{
	public class UIHelper
	{
		public void ShowKeyboard(EditText editText, Activity activity)
		{
			InputMethodManager imm = (InputMethodManager) activity.GetSystemService(Context.InputMethodService);
			if (imm != null) {
				// only will trigger it if no physical keyboard is open
				imm.ShowSoftInput(editText, 0);
			}
		}
	}
}

