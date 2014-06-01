using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserLocationModelFactory: IUserLocationModelFactory
    {
        private readonly UrlHelper _urlHelper;

        public UserLocationModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public IEnumerable<UserLocationModel> Create(IEnumerable<UserLocation> userLocations)
        {
            var userLocationsModel = new Collection<UserLocationModel>();

            foreach (var userLocation in userLocations)
            {
                userLocationsModel.Add(Create(userLocation));
            }
            return userLocationsModel;
        }

        public UserLocationModel Create(UserLocation userLocation)
        {
            return new UserLocationModel
            {
                ID = userLocation.ID,
                Url = _urlHelper.Link("UserLocations", new { id = userLocation.ID }),
                UserID = userLocation.UserID,
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude,
                CreatedDate = userLocation.CreatedDate,
                UpdateDate = userLocation.UpdateDate
            };
        }
    }
}