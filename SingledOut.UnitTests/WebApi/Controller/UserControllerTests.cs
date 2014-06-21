using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SingledOut.Model;
using SingledOut.SearchParameters;

namespace SingledOut.UnitTests.WebApi.Controller
{
    [TestFixture]
    public class Class1
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
                FacebookUserName = "ifflanb",
                UpdateDate = DateTime.UtcNow,
                Email = "ifflanb@yahoo.com",
                Password = CreateHash("testpassword1")
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
            var data = CreateDummyUserModel();
            var response = PostAsync("http://localhost/SingledOut.WebApi/api/users", data);
   
            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        public HttpResponseMessage PostAsync(string uri, object data)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(data);

            HttpContent cont = new StringContent(json);
            cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = null;

            try
            {
                response = httpClient.PostAsync(uri, cont).Result;
            }
            catch (Exception ex)
            {
                throw;
            }

            return response;
        }

        [Test]
        public void Test_That_Login_Is_Successful()
        {
            //
            // Arrange.
            //
            var response = Login("http://localhost/SingledOut.WebApi/api/users/Login", "ifflanb@yahoo.com", "testpassword1");

            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
        }

        public AuthenticationHeaderValue CreateBasicHeader(string username, string password)
        {
            //password = CreateHash(password);
            byte[] byteArray = Encoding.UTF8.GetBytes(username + ":" + password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public string CreateHash(string unHashed)
        {
            var x = new MD5CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(unHashed);
            data = x.ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        public HttpResponseMessage Login(string uri, string username, string password)
        {
            var httpClient = new HttpClient();

            //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(username, password);
			
			var response = httpClient.GetAsync(uri).Result;
            
            return response;
        }

        public HttpResponseMessage GetAsync(string uri, UsersSearchParameters sp)
        {
            var httpClient = new HttpClient();
            
            //HttpContent cont = new ;
            //cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            uri = string.Concat(uri, "/?Facebookvar=", sp.FacebookUserName);
            var response = httpClient.GetAsync(uri).Result;

            return response;
        }
        
    }
}
