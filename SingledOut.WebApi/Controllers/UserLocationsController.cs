using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.Controllers
{
    public class UserLocationsController : ApiController
    {
        private readonly IUserLocationsRepository _userLocationsRepository;
        private readonly IUserLocationModelFactory _userLocationModelFactory;

        public UserLocationsController(
            IUserLocationsRepository userLocationsRepository, 
            IUserLocationModelFactory userLocationModelFactory)
        {
            _userLocationsRepository = userLocationsRepository;
            _userLocationModelFactory = userLocationModelFactory;
        }

        public IEnumerable<UserLocationModel> Get()
        {
            var query = _userLocationsRepository.GetAllUserLocations();

            var results = query.ToList().Select(s => _userLocationModelFactory.Create(s));

            return results;
        }

        public HttpResponseMessage GetUserLocationByID(int id)
        {
            try
            {
                var userLocation = _userLocationsRepository.GetUserLocation(id);
                if (userLocation != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userLocationModelFactory.Create(userLocation));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public int AddUserQuestion(UserLocation user)
        {
            return _userLocationsRepository.Insert(user);
        }

        public int UpdateUserQuestion(UserLocation originalUserLocation, UserLocation updatedUserLocation)
        {
            return _userLocationsRepository.Update(originalUserLocation, updatedUserLocation);
        }
    }
}
