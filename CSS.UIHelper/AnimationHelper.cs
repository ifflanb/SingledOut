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
using Android.Views.Animations;
using Android.Util;

namespace CSS.Helpers
{			
	public class AnimationHelper : Activity
	{
		/// <summary>
		/// Sets the height of the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="height">Height.</param>
		public void SetLayoutHeight(LinearLayout layout, int height)
		{
			var param = layout.LayoutParameters;
			param.Height = height;
			layout.LayoutParameters = param; // Set the layout size to be half of the screen size.
		}

		/// <summary>
		/// Creates the view animation.
		/// </summary>
		/// <returns>The view animate.</returns>
		/// <param name="view">View.</param>
		/// <param name="xStart">X start.</param>
		/// <param name="xEnd">X end.</param>
		/// <param name="yStart">Y start.</param>
		/// <param name="yEnd">Y end.</param>
		/// <param name="duration">Duration.</param>
		public TranslateAnimation CreateViewAnimation(View view, int xStart, int xEnd, int yStart, int yEnd, int duration)
		{
			TranslateAnimation anim = new TranslateAnimation(xStart, xEnd, yStart, yEnd);
			anim.Duration = duration;
			anim.FillAfter = true;
			return anim;
		}

		/// <summary>
		/// Gets the screen height without status bar.
		/// </summary>
		/// <returns>The screen height without status bar.</returns>
		public int GetScreenHeightWithoutStatusBar(RelativeLayout layout, Activity activity)
		{
			var dm = new DisplayMetrics ();
			activity.WindowManager.DefaultDisplay.GetMetrics (dm);
			return dm.HeightPixels - layout.MeasuredHeight;
		}

		/// <summary>
		/// Gets the width of the screen.
		/// </summary>
		/// <returns>The screen width.</returns>
		public int GetScreenWidth(Activity activity)
		{
			DisplayMetrics dm = new DisplayMetrics ();
			activity.WindowManager.DefaultDisplay.GetMetrics (dm);
			return dm.WidthPixels;
		}
	}
}
