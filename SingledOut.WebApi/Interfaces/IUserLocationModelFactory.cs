using System.Collections.Generic;
using SingledOut.Data;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserLocationModelFactory
    {
        IEnumerable<UserLocationModel> Create(IEnumerable<UserLocation> userLocations);

        UserLocationModel Create(UserLocation userLocation);

        UserLocation Parse(UserLocationModel model);
    }
}
