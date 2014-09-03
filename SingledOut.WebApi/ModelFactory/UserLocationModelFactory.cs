using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Http.Routing;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserLocationModelFactory: IUserLocationModelFactory
    {
        private UrlHelper _urlHelper;
        private UserLocationsRepository _repository ;

        public UserLocationModelFactory(UserLocationsRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<UserLocationModel> Create(IEnumerable<UserLocation> userLocations)
        {
            var userLocationsModel = new Collection<UserLocationModel>();
            
            return userLocationsModel;
        }

        public UserLocationModel Create(UserLocation userLocation)
        {
            var userLocationsModel = new UserLocationModel
            {
                ID = userLocation.ID,
                CreatedDate = userLocation.CreatedDate,
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude,
                PlaceName = userLocation.PlaceName,
                UpdateDate = userLocation.UpdateDate,
                UserID = userLocation.UserID
            };

            return userLocationsModel;
        }

        public UserLocation Parse(UserLocationModel model)
        {
            try
            {
                var user = new UserLocation
                {
                    UserID = model.UserID,
                    ID = model.ID,
                    CreatedDate = model.CreatedDate,
                    UpdateDate = model.UpdateDate,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    PlaceName = model.PlaceName
                };

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}