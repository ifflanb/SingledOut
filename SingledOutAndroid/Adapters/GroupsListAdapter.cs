using System;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using SingledOut.Model;
using Android.Views;

namespace SingledOutAndroid.Adapters
{
	public class GroupsListAdapter : BaseAdapter
	{
		Activity context;

		/// <summary>
		/// The items.
		/// </summary>
		public List<UserLocationsFlat> items;

		/// <summary>
		/// Gets or sets the custom list item I.
		/// </summary>
		/// <value>The custom list item I.</value>
		public int CustomListItemID {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the custom list item name I.
		/// </summary>
		/// <value>The custom list item name I.</value>
		public int CustomListItemNameID {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the custom list item image I.
		/// </summary>
		/// <value>The custom list item image I.</value>
		public int CustomListItemPhoto {
			get;
			set;
		}

		public GroupsListAdapter(Activity context) //We need a context to inflate our row view from
			: base()
		{
			this.context = context;
		}

		/// <summary>
		/// How many items are in the data set represented by this Adapter.
		/// </summary>
		/// <value>To be added.</value>
		public override int Count
		{
			get { return items.Count; }
		}

		/// <param name="position">Position of the item whose data we want within the adapter's 
		///  data set.</param>
		/// <summary>
		/// Get the data item associated with the specified position in the data set.
		/// </summary>
		/// <returns>To be added.</returns>
		public override Java.Lang.Object GetItem(int position)
		{
			return position;
		}

		/// <param name="position">The position of the item within the adapter's data set whose row id we want.</param>
		/// <summary>
		/// Get the row id associated with the specified position in the list.
		/// </summary>
		/// <returns>To be added.</returns>
		public override long GetItemId(int position)
		{
			return position;
		}

		/// <param name="position">The position of the item within the adapter's data set of the item whose view
		///  we want.</param>
		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <returns>The view.</returns>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			//Get our object for this position
			var item = items[position];           

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// This gives us some performance gains by not always inflating a new view
			// This will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = (convertView ??
				context.LayoutInflater.Inflate(
					CustomListItemID,
					parent,
					false)) as LinearLayout;

			//Find references to each subview in the list item's view
			var imageItem = view.FindViewById(CustomListItemPhoto) as ImageView;
			var name = view.FindViewById(CustomListItemNameID) as TextView;
			//var latitude = view.FindViewById(CustomListItemLatitudeID) as TextView;
			//var longitude = view.FindViewById(CustomListItemLongitudeID) as TextView;

			//Assign this item's values to the various subviews
//			imageItem.SetImageResource(item.F);
			name.SetText (string.Format("{0} {1}", item.FirstName, item.Surname.Substring(0,1)), TextView.BufferType.Normal);
//			latitude.SetText(item.Latitude.ToString(), TextView.BufferType.Normal);
//			longitude.SetText(item.Longitude.ToString(), TextView.BufferType.Normal);

			//Finally return the view
			return view;
		}

		public UserLocationsFlat GetItemAtPosition(int position)
		{
			return items[position];
		}
	}
}

