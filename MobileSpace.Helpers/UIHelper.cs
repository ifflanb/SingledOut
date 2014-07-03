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
		public delegate void ListViewItemClick(object sender, AdapterView.ItemClickEventArgs e);
		public event ListViewItemClick OnListViewItemClick;

		/// <summary>
		/// Gets or sets the dialog view.
		/// </summary>
		/// <value>The dialog view.</value>
		public View DialogView {
			get;
			set;
		}

		public void ShowKeyboard(EditText editText, Activity activity)
		{
			InputMethodManager imm = (InputMethodManager) activity.GetSystemService(Context.InputMethodService);
			if (imm != null) {
				// only will trigger it if no physical keyboard is open
				imm.ShowSoftInput(editText, 0);
			}
		}

		/// <summary>
		/// Builds the alert dialog.
		/// </summary>
		/// <returns>The alert dialog.</returns>
		/// <param name="activity">Activity.</param>
		/// <param name="title">Title.</param>
		/// <param name="iconResourceID">Icon resource I.</param>
		/// <param name="okButton">If set to <c>true</c> ok button.</param>
		/// <param name="cancelButton">If set to <c>true</c> cancel button.</param>
		public AlertDialog BuildAlertDialog(CustomListAdapter adapter, bool useFastSearchForListView, bool cancelableOnTouchOutside, int layoutID, int textItemID, Activity activity, string title, int iconResourceID,int listViewID)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder (activity);

			DialogView = activity.LayoutInflater.Inflate (layoutID, null);
			builder.SetView(DialogView);
		
			AlertDialog dialog = builder.Create ();
			dialog.SetTitle (title);
			dialog.SetIcon (iconResourceID);
			dialog.SetCanceledOnTouchOutside (false);

			var listView = DialogView.FindViewById<ListView> (listViewID);
			if (listView != null) {
				listView.Adapter = adapter;
				listView.FastScrollEnabled = useFastSearchForListView;
				listView.FastScrollAlwaysVisible = true;
				listView.DividerHeight = 4;
				listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
				{
					OnListViewItemClick(sender, e);
				};

			}

			return dialog;
		}

	}
}

