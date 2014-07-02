using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data.Entities;
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

            //foreach (var userLocation in userLocations)
            //{
            //    userLocationsModel.Add(Create(userLocation));
            //}
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
                    Longitude = model.Longitude
                };

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public UserLocationModel Create(UserLocation userLocation, HttpRequestMessage request)
        {
            if (request != null)
            {
                _urlHelper = new UrlHelper(request);
            }

            return new UserLocationModel
            {
                ID = userLocation.ID,
                Url = _urlHelper != null ? _urlHelper.Link("UserLocations", new { controller = "UserLocations", id = userLocation.ID }) : string.Empty,
                UserID = userLocation.UserID,
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude,
                CreatedDate = userLocation.CreatedDate,
                UpdateDate = userLocation.UpdateDate
            };
        }
    }
}