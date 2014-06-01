using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SingledOut.Globals;
using SingledOut.Model;

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
        public void Test_That_POST_User_Is_Successful()
        {
            //
            // Arrange.
            //
            var data = CreateDummyUserModel();

            var json = JsonConvert.SerializeObject(data);
            Task<HttpResponseMessage> response;

            //
            // Act.
            //
            using (var httpClient = new HttpClient())
            {
                var postBody = json;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = httpClient.PostAsync(Constants.ApiUrlUsers, new StringContent(postBody, Encoding.UTF8, "application/json"));
            }

            //
            // Assert.
            //
            Assert.That(response.Result.IsSuccessStatusCode, Is.EqualTo(true));
        }
    }
}
