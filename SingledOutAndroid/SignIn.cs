using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Facebook;
using Facebook.Android;
using SingledOut.Model;
using System.Threading.Tasks;
using SingledOutAndroid.Helpers;
using CSS.Helpers;

namespace SingledOutAndroid
{
	public class Constants
	{
		public const string ApiUrlRoot = "http://localhost:50883/api/";
		public const string ApiUrlUsers = ApiUrlRoot + "users";
		public const string ApiUrlQuestions = ApiUrlRoot + "questions";
		public const string ApiUrlAnswers = ApiUrlRoot + "answers";
	}

	[Activity (Label = "Sign in or Register",
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class SignIn : BaseActivity
	{
		// Put Facebook App Id here, if you don't have one go to
		// https://developers.facebook.com/apps
		private const string AppId = "732420766779089";	    
		private AnimationHelper _animationHelper;

		public SignIn ()
		{
			
		}
		/// <summary>
		/// Extended permissions is a comma separated list of permissions to ask the user.
		/// </summary>
		/// <remarks>
		/// For extensive list of available extended permissions refer to 
		/// https://developers.facebook.com/docs/reference/api/permissions/
		/// </remarks>
		private const string ExtendedPermissions = "user_about_me,read_stream,publish_stream";

		FacebookClient _fb;
	    readonly string _accessToken;

	    public SignIn(string accessToken, string lastMessageId)
	    {
	        this._accessToken = accessToken;
	    }

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SwipeRightActivity = typeof(Welcome);
			SwipeLeftActivity = typeof(Tutorial1);

			//NavigateRightActivity = 
			// Create your application here
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SignIn);
			_animationHelper = new AnimationHelper ();

			// Get our button from the layout resource,
			// and attach an event to it
			var facebookLogin = FindViewById<ImageButton>(Resource.Id.facebooklogin);					
			facebookLogin.Click += HandleFacebookLogin;

			// Add footer.
			var footerFragment = new FooterLayout ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.signinchildlayout, footerFragment);
			ft.Commit ();

		}

		void HandleFacebookLogin (object sender, EventArgs ea) {
			var webAuth = new Intent (this, typeof (FBWebViewAuthActivity));
			webAuth.PutExtra ("AppId", AppId);
			webAuth.PutExtra ("ExtendedPermissions", ExtendedPermissions);
			StartActivityForResult (webAuth, 0);
		}

	    private static Task SignInAsync(UserModel userModel)
	    {
			Task result = null;// RestHelper.PostAsync(Constants.ApiUrlUsers, userModel);
	        return result;
	    }

	    protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			switch (resultCode) {
			case Result.Ok:
			        
                var userModel = new UserModel {
                    FirstName = "test",
                    Surname = "iffland",
                    Sex = data.GetStringExtra("gender"),
                    CreatedDate = DateTime.UtcNow,
                    FacebookAccessToken = data.GetStringExtra ("AccessToken"),
                    FacebookUserName = data.GetStringExtra ("UserId"),
                    UpdateDate = DateTime.UtcNow
                };

			     var task = SignInAsync(userModel);
                
				
				var error = data.GetStringExtra ("Exception");

				_fb = new FacebookClient (_accessToken);

				//	ImageView imgUser = FindViewById<ImageView> (Resource.Id.imgUser);
				//	TextView txtvUserName = FindViewById<TextView> (Resource.Id.txtvUserName);

				_fb.GetTaskAsync ("me").ContinueWith( t => {
					if (!t.IsFaulted) {

						var result = (IDictionary<string, object>)t.Result;

						// available picture types: square (50x50), small (50xvariable height), large (about 200x variable height) (all size in pixels)
						// for more info visit http://developers.facebook.com/docs/reference/api
//						string profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", userId, "square", accessToken);
//						var bm = BitmapFactory.DecodeStream (new Java.Net.URL(profilePictureUrl).OpenStream());
//						string profileName = (string)result["name"];

//						RunOnUiThread (()=> {
//							imgUser.SetImageBitmap (bm);
//							txtvUserName.Text = profileName;
//						});
					} else {
						//Alert ("Failed to Log In", "Reason: " + error, false, (res) => {} );
					}
				});

				break;

			case Result.Canceled:
				//Alert ("Failed to Log In", "User Cancelled", false, (res) => {} );
				break;
			}
		}

	}
}

