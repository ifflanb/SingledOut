using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.WebApi.Filters;
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

        [SingledOutAuthorization]
        [HttpGet]
        public IEnumerable<UserLocationModel> Get()
        {
            var query = _userLocationsRepository.GetAllUserLocations();

            var results = query.ToList().Select(s => _userLocationModelFactory.Create(s, Request));

            return results;
        }

        [SingledOutAuthorization]
        [HttpGet]
        public HttpResponseMessage GetUserLocationByID(int id)
        {
            try
            {
                var userLocation = _userLocationsRepository.GetUserLocation(id);
                if (userLocation != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userLocationModelFactory.Create(userLocation, Request));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [SingledOutAuthorization]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] UserLocationModel userLocationModel)
        {
            try
            {
                var entity = _userLocationModelFactory.Parse(userLocationModel);

                if (entity == null) Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read user location from body");

                var result = _userLocationsRepository.Insert(entity);
                if (result > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, _userLocationModelFactory.Create(entity, Request));
                }
                
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save user location to the database.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [SingledOutAuthorization]
        [HttpPut]
        public int UpdateUserLocation(UserLocation originalUserLocation, UserLocation updatedUserLocation)
        {
            return _userLocationsRepository.Update(originalUserLocation, updatedUserLocation);
        }

        [SingledOutAuthorization]
        [HttpDelete]
        public HttpResponseMessage DeleteUserLocation([FromUri] int id)
        {
            try
            {
                var result = _userLocationsRepository.DeleteUserLocation(id);
                if (result == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);
                }
                
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not delete user location from the database.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        
    }
}
