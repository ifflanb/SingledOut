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
using MobileSpace.Helpers;

namespace MobileSpace.Helpers
{
	public class CustomListAdapter : BaseAdapter
	{
		Activity context;

		/// <summary>
		/// The items.
		/// </summary>
		public List<GooglePlace> items;

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
		/// Gets or sets the custom list item latitude I.
		/// </summary>
		/// <value>The custom list item latitude I.</value>
		public int CustomListItemLatitudeID {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the custom list item longitude I.
		/// </summary>
		/// <value>The custom list item longitude I.</value>
		public int CustomListItemLongitudeID {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the custom list item image I.
		/// </summary>
		/// <value>The custom list item image I.</value>
		public int CustomListItemImageID {
			get;
			set;
		}

		public CustomListAdapter(Activity context) //We need a context to inflate our row view from
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
			var imageItem = view.FindViewById(CustomListItemImageID) as ImageView;
			var name = view.FindViewById(CustomListItemNameID) as TextView;
			var latitude = view.FindViewById(CustomListItemLatitudeID) as TextView;
			var longitude = view.FindViewById(CustomListItemLongitudeID) as TextView;

			//Assign this item's values to the various subviews
			imageItem.SetImageResource(item.Image);
			name.SetText (item.Name, TextView.BufferType.Normal);
			latitude.SetText(item.Latitude.ToString(), TextView.BufferType.Normal);
			longitude.SetText(item.Longitude.ToString(), TextView.BufferType.Normal);

			//Finally return the view
			return view;
		}

		public GooglePlace GetItemAtPosition(int position)
		{
			return items[position];
		}
	}
}