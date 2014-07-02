using System.Collections.Generic;
using System.Net.Http;
using SingledOut.Data.Entities;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserLocationModelFactory
    {
        IEnumerable<UserLocationModel> Create(IEnumerable<UserLocation> userLocations);

        UserLocationModel Create(UserLocation userLocation, HttpRequestMessage request);

        UserLocation Parse(UserLocationModel model);
    }
}
