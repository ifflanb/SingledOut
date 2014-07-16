using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SingledOut.UnitTests.WebApi.Controller
{
    [TestFixture]
    public class UserLocationControllerTests
    {
        [Test]
        public void Test_That_POST_User_Is_Successful()
        {
            //
            // Arrange.
            //
            var response = DeleteAsync("http://localhost/SingledOut.WebApi/api/userLocations/DeleteUserLocation/26");

            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public HttpResponseMessage DeleteAsync(string uri)
        {
            var httpClient = new HttpClient();

            HttpResponseMessage response = null;

            try
            {
                response = httpClient.DeleteAsync(uri).Result;
            }
            catch (Exception)
            {
                
            }
            



            return response;
        }

    }
}
