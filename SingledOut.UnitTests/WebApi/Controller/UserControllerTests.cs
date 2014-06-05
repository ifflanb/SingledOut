using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
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
            PostAsync("http://localhost/SingledOut.WebApi/api/users", data);
   
            //
            // Assert.
            //
            //Assert.That(response.Result.IsSuccessStatusCode, Is.EqualTo(true));
        }

        public HttpResponseMessage PostAsync(string uri, object data)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(data);

            HttpContent cont = new StringContent(json);
            cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = httpClient.PostAsync(uri, cont).Result;

            response.EnsureSuccessStatusCode();

            return response;
        }

        //public HttpResponseMessage UploadNewClass(UserModel newClass, string url)
        //{
        //    try
        //    {
        //        // Serialize new class object to a Json string
        //        var json = JsonConvert.SerializeObject(newClass, Formatting.Indented);

        //        // Setup web client
        //        var client = new WebClient {Encoding = Encoding.ASCII};
        //        client.Headers.Add(HttpRequestHeader.ContentType, "application/json"); // tell the API we want Json returned

        //        // Upload Json string via POST method and return bytes
        //        var returnData = client.UploadData(url, "POST", Encoding.Default.GetBytes(json));

        //        // Return string data as boolean
        //        return returnData;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Rest Exception: " + ex.Message);
        //        return false;
        //    }
        //}
    }
}
