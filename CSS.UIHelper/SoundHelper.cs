using System;
using Android.Media;
using Android.App;

namespace CSS.Helpers
{
	public class SoundHelper
	{
		private MediaPlayer _player;

		/// <summary>
		/// Plays the sound.
		/// </summary>
		/// <param name="resID">Res I.</param>
		/// <param name="activity">Activity.</param>
		public void PlaySound(int resID, Activity activity)
		{
			_player = MediaPlayer.Create(activity, resID);
			_player.Start();
		}
	}
}

