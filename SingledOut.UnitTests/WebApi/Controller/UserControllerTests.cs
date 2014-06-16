using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                UpdateDate = DateTime.UtcNow
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

            var response = httpClient.PostAsync(uri, cont).Result;
           

            return response;
        }

        public HttpResponseMessage GetAsync(string uri, UsersSearchParameters sp)
        {
            var httpClient = new HttpClient();
            
            //HttpContent cont = new ;
            //cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            uri = string.Concat(uri, "/?FacebookUserName=", sp.FacebookUserName);
            var response = httpClient.GetAsync(uri).Result;

            return response;
        }
        
    }
}
