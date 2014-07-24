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
using Android.Graphics.Drawables;
using Android.Graphics;

namespace MobileSpace.Helpers
{
	public class UIHelper
	{
		public delegate void ListViewItemClick(object sender, AdapterView.ItemClickEventArgs e);
		public event ListViewItemClick OnListViewItemClick;

		public delegate void AlertDialogClosed(object sender, EventArgs e);
		public event AlertDialogClosed OnAlertDialogClosed;

		public delegate void TabSelectedClick(object sender, ActionBar.TabEventArgs e);
		public event TabSelectedClick OnTabSelectedClick;

		/// <summary>
		/// Gets or sets the dialog view.
		/// </summary>
		/// <value>The dialog view.</value>
		public View DialogView {
			get;
			set;
		}

		private ProgressDialog _progressDialog;

		/// <summary>
		/// Displays the progress dialog.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="theme">Theme.</param>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		public void DisplayProgressDialog(Activity activity, int theme, string title, string message)
		{
			_progressDialog = new ProgressDialog (activity, theme);
			_progressDialog.SetTitle (title);
			_progressDialog.SetMessage (message);
			_progressDialog.Show();
		}

		/// <summary>
		/// Hides the progress dialog.
		/// </summary>
		public void HideProgressDialog()
		{
			if (_progressDialog != null) {
				_progressDialog.Hide ();
			}
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
		/// Adds the action bar tab.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="tabNameResId">Tab name res identifier.</param>
		/// <param name="tabIconDrawResId">Tab icon draw res identifier.</param>
		public ActionBar.Tab AddActionBarTab(Activity activity, int tabNameResId, int tabIconDrawResId)
		{
			ActionBar.Tab tab = activity.ActionBar.NewTab();
			tab.SetText(activity.Resources.GetString(tabNameResId));
			tab.SetIcon(tabIconDrawResId);
			tab.TabSelected += (object sender, ActionBar.TabEventArgs e) => 
			{
				OnTabSelectedClick(sender, e);
			};
			activity.ActionBar.AddTab(tab);
			return tab;
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
			dialog.DismissEvent += DialogClosed;

			var listView = DialogView.FindViewById<ListView> (listViewID);
			if (listView != null) {
				listView.Adapter = adapter;
				listView.FastScrollEnabled = useFastSearchForListView;
				listView.FastScrollAlwaysVisible = true;
				listView.DividerHeight = 4;
				listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					OnListViewItemClick (sender, e);
				};
			}

			return dialog;
		}
		protected void DialogClosed(object sender, EventArgs e)
		{
			DialogView = null;
			OnAlertDialogClosed (sender, e);
		}
	}
}

