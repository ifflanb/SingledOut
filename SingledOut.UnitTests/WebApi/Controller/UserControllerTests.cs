using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using SingledOut.Model;
using SingledOut.SearchParameters;

namespace SingledOut.UnitTests.WebApi.Controller
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserModel CreateDummyUserModel()
        {
            var userModel = new UserModel
            {
                FirstName = "test",
                Surname = "iffland",
                Sex = "M",
                CreatedDate = DateTime.UtcNow,
                FacebookAccessToken = "1234567890",
                FacebookUserName = "ifflanb2",
                UpdateDate = DateTime.UtcNow,
                Email = "ifflanb2@yahoo.com",
                Password = "testpassword1"
            };
            return userModel;
        }

       [Test]
        public void Test_That_GET_Users_Is_Successful()
        {
            //
            // Arrange.
            //
            var sp = new UsersSearchParameters
            {
                FacebookUserName = "10152201036186137"
            };

            var response = GetAsync("http://localhost/SingledOut.WebApi/api/userssearch", sp);

            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void Test_That_POST_User_Is_Successful()
        {
            //
            // Arrange.
            //
            var userModel = UnitTestDataHelper.CreateDefaultUser();
            var response = PostAsync("http://localhost/SingledOut.WebApi/api/users", userModel);
   
            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        /// <summary>
        /// Posts an Async request.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public HttpResponseMessage PostAsync(string uri, object data)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(data);

            HttpContent cont = new StringContent(json);
            cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = httpClient.PostAsync(uri, cont).Result;

            return response;
        }

        [Test]
        public void Test_That_Login_Is_Successful()
        {
            //
            // Arrange.
            //
            UnitTestDataHelper.CreateDefaultUser();
            var response = Login("http://localhost/SingledOut.WebApi/api/users/Login", "jo.bloggs@test.com", "testpassword1");

            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
        }

        public AuthenticationHeaderValue CreateBasicHeader(string username, string password)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(username + ":" + password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public HttpResponseMessage Login(string uri, string username, string password)
        {
            var httpClient = new HttpClient();
            
            httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);
			
			var response = httpClient.GetAsync(uri).Result;
            
            return response;
        }


        public HttpResponseMessage GetAsync(string uri, UsersSearchParameters sp)
        {
            var httpClient = new HttpClient();
           
            uri = string.Concat(uri, "/?Facebookvar=", sp.FacebookUserName);
            var response = httpClient.GetAsync(uri).Result;

            return response;
        }
        
    }
}
