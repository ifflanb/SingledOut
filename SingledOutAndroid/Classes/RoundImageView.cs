using System;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Util;

namespace SingledOutAndroid
{
	public class RoundImageView : ImageView 
	{
		public RoundImageView(Context context) 
		: base(context)
		{
		}

		public RoundImageView(Context context, IAttributeSet attrs) 
		: base (context, attrs)
		{
		}

		public RoundImageView(Context context, IAttributeSet attrs, int defStyle) 
		: base (context, attrs, defStyle)
		{
		}

		protected override void OnDraw (Canvas canvas) {

			Drawable drawable = Drawable;

			if (drawable == null) {
				return;
			}

			if (Width == 0 || Height == 0) {
				return; 
			}
			Bitmap b =  ((BitmapDrawable)drawable).Bitmap;
			Bitmap bitmap = b.Copy(Bitmap.Config.Argb8888, true);

			int w = Width, h = Height;

			Bitmap roundBitmap =  getCroppedBitmap(bitmap, w);
			canvas.DrawBitmap(roundBitmap, 0,0, null);

		}

		public static Bitmap getCroppedBitmap(Bitmap bmp, int radius) {
			Bitmap sbmp;
			if(bmp.Width != radius || bmp.Height != radius)
				sbmp = Bitmap.CreateScaledBitmap(bmp, radius, radius, false);
			else
				sbmp = bmp;
			Bitmap output = Bitmap.CreateBitmap(sbmp.Width,
				sbmp.Height, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(output);

			//const int color = 0xffa19774;
			Paint paint = new Paint();
			Rect rect = new Rect(0, 0, sbmp.Width, sbmp.Height);

			paint.AntiAlias = true;
			paint.FilterBitmap = true;
			paint.Dither = true;

			canvas.DrawARGB(0, 0, 0, 0);
			paint.Color = Color.ParseColor("#BAB399");
			canvas.DrawCircle(sbmp.Width / 2+0.7f, sbmp.Height / 2+0.7f,
				sbmp.Width / 2+0.1f, paint);
			paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
			canvas.DrawBitmap(sbmp, rect, rect, paint);

			return output;
		}
	}
}
