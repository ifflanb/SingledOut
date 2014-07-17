using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using SingledOut.Model;

namespace SingledOut.UnitTests.WebApi.Controller
{
    [TestFixture]
    public class AccountControllerTests
    {
        private UserModel CreateDummyUserModel()
        {
            var userModel = new UserModel
            {
                FirstName = "test",
                Surname = "iffland",
                Sex = "M",
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Email = "ifflanb2@yahoo.com",
                Password = CreateHash("testpassword1")
            };
            return userModel;
        }

        public string CreateHash(string unHashed)
        {
            var x = new MD5CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(unHashed);
            data = x.ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        [Test]
        public void Test_That_Register_Account_Is_Successful()
        {
            //
            // Arrange.
            //
            var data = CreateDummyUserModel();
            var response = PostAsync("http://localhost/SingledOut.WebApi/api/account/register", data);

            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        public HttpResponseMessage PostAsync(string uri, object data)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(data);

            var cont = new StringContent(json);
            cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = httpClient.PostAsync(uri, cont).Result;

            return response;
        }
    }
}
