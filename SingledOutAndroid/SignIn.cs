using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Facebook;
using SingledOut.Model;
using System.Threading.Tasks;
using CSS.Helpers;
using Xamarin.Auth;
using System.Json;

namespace SingledOutAndroid
{
	public class Constants
	{
		public const string ApiUrlRoot = "http://10.1.1.10/SingledOut.WebApi/api/";
		public const string ApiUrlUsers = ApiUrlRoot + "users";
		public const string ApiUrlQuestions = ApiUrlRoot + "questions";
		public const string ApiUrlAnswers = ApiUrlRoot + "answers";
	}

	[Activity (Label = "Sign in or Register", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class SignIn : BaseActivity
	{		    
		private AnimationHelper _animationHelper;
		private RestHelper<UserModel> _restHelper;

		public SignIn ()
		{
			_restHelper = new RestHelper<UserModel>();
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
			facebookLogin.Click += delegate { LoginToFacebook(true); };

			// Add footer.
			var footerFragment = new FooterLayout ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.signinchildlayout, footerFragment);
			ft.Commit ();
		}


		// Put Facebook App Id here, if you don't have one go to
		// https://developers.facebook.com/apps
		private const string AppId = "732420766779089";	

		/// <summary>
		/// Extended permissions is a comma separated list of permissions to ask the user.
		/// </summary>
		/// <remarks>
		/// For extensive list of available extended permissions refer to 
		/// https://developers.facebook.com/docs/reference/api/permissions/
		/// </remarks>
		private const string ExtendedPermissions = "user_about_me,read_stream,publish_stream";

		public void LoginToFacebook (bool allowCancel)
		{
			var auth = new OAuth2Authenticator (
				clientId: AppId,
				scope: ExtendedPermissions,
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

			auth.AllowCancel = allowCancel;

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += (s, ee) => {
				if (!ee.IsAuthenticated) {
					var builder = new AlertDialog.Builder (this);
					builder.SetMessage ("Not Authenticated");
					builder.SetPositiveButton ("Ok", (o, e) => { });
					builder.Create().Show();
					return;
				}

				// Now that we're logged in, make a OAuth2 request to get the user's info.
				var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, ee.Account);
				request.GetResponseAsync().ContinueWith (t => {
					var builder = new AlertDialog.Builder (this);
					if (t.IsFaulted) {
						builder.SetTitle ("Error");
						builder.SetMessage (t.Exception.Flatten().InnerException.ToString());
					} 
					else if (t.IsCanceled)
					{
						builder.SetTitle ("Task Canceled");
					}
					else 
					{
						var obj = JsonValue.Parse (t.Result.GetResponseText());

						var userModel = new UserModel {
							FirstName = obj["first_name"],
							Surname = obj["last_name"],
							Sex = obj["gender"],
		                    CreatedDate = DateTime.UtcNow,
							FacebookAccessToken = t.Result.ResponseUri.Query.Replace("?access_token=",string.Empty),
							FacebookUserName = obj["id"],
		                    UpdateDate = DateTime.UtcNow
		                };					

						_restHelper.PostAsync(Constants.ApiUrlUsers, userModel);
					}

					builder.SetPositiveButton ("Ok", (o, e) => { });
					builder.Create().Show();
				}, UIScheduler);
			};

			var intent = auth.GetUI (this);
			StartActivity (intent);
		}

		private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}

