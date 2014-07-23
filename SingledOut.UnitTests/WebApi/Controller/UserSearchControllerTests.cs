using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using SingledOut.Model;
using SingledOut.SearchParameters;

namespace SingledOut.UnitTests.WebApi.Controller
{
    [TestFixture]
    public class UserSearchControllerTests
    {
        [Test]
        public void Test_That_GET_Users_Is_Successful()
        {
            //
            // Arrange.
            //
            var sp = new UsersSearchParameters
            {
                AgeFrom = 40,
                AgeTo = 50,
                Distance = 1000,
                Sex = GenderEnum.Both
            };

            var uri = new Uri("http://localhost/SingledOut.WebApi/api/userssearch?AgeFrom=40&AgeTo=50&Distance=1000&Sex=3&UserLatitude=-43.544839&UserLongitude=172.567498");

            var response = GetAsync(uri.ToString());

            //
            // Assert.
            //
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public HttpResponseMessage GetAsync(string uri)
        {
            var httpClient = new HttpClient();

            var response = httpClient.GetAsync(uri).Result;

            return response;
        }


    }
}
