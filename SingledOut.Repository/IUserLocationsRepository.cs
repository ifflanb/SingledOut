using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public interface IUserLocationsRepository
    {
        IQueryable<UserLocation> GetAllUserLocations();

        UserLocation GetUserLocation(int userLocationID);

        int Insert(UserLocation userLocation);

        int Update(UserLocation originalUserLocation, UserLocation updatedUserLocation);

        int DeleteUserLocation(int id);
    }
}
