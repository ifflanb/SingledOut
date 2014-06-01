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

namespace SingledOutAndroid
{
	[Activity (Label = "BaseActivity")]			
	public class BaseActivity : Activity , GestureDetector.IOnGestureListener
	{
		private GestureDetector _gestureDetector;		  
		private const int SWIPE_MIN_DISTANCE = 120;
		private const int SWIPE_MAX_OFF_PATH = 250;
		private const int SWIPE_THRESHOLD_VELOCITY = 200;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_gestureDetector = new GestureDetector(this);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			_gestureDetector.OnTouchEvent(e);
			return false;
		}

		public Type SwipeLeftActivity { get; set;}

		public Type SwipeRightActivity { get; set;}

		public void SwipeLeft()
		{
			StartActivity (SwipeLeftActivity);
		}

		public void SwipeRight()
		{
			StartActivity (SwipeRightActivity);
		}

		public bool OnDown (MotionEvent e)
		{
			return true;
		}

		public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			try
			{
				if (Math.Abs(e1.GetY() - e2.GetY()) > SWIPE_MAX_OFF_PATH)
				{
					return false;
				}
				// swipe to the left.
				if (e1.GetX() - e2.GetX() > SWIPE_MIN_DISTANCE && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY)
				{
					SwipeLeft();
				}
				// Swipe to the right.
				else if (e2. GetX()- e1.GetX() > SWIPE_MIN_DISTANCE && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY)
				{
					SwipeRight();
				}
			}
			catch (Exception)
			{
				// nothing
			}
			return false;
		}

		public void OnLongPress (MotionEvent e)
		{
		}

		public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return true;
		}

		public void OnShowPress (MotionEvent e)
		{
		}

		public bool OnSingleTapUp (MotionEvent e)
		{
			return true;
		}
	}
}

