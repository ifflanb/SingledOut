using System;
using Android.App;
using Android.Views.Animations;
using Android.Views;

namespace SingledOutAndroid
{
	public class Animations
	{
		private Activity _activity;

		public Animations (Activity activity)
		{
			_activity = activity;
		}

		/// <summary>
		/// Shakes the view.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="view">View.</param>
		public void ShakeView(int viewID)
		{
			Animation shake = AnimationUtils.LoadAnimation(_activity, Resource.Drawable.shake);
			_activity.FindViewById(viewID).StartAnimation(shake);
		}
	}
}

