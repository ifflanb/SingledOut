using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.WebApi.Filters;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.Controllers
{
    public class UserPreferencesController : ApiController
    {
        private readonly IUserPreferencesRepository _userPreferencesRepository;
        private readonly IUserPreferenceModelFactory _userPreferenceModelFactory;

        public UserPreferencesController(
            IUserPreferencesRepository userPreferencesRepository,
            IUserPreferenceModelFactory userPreferenceModelFactory)
        {
            _userPreferencesRepository = userPreferencesRepository;
            _userPreferenceModelFactory = userPreferenceModelFactory;
        }

        [SingledOutAuthorization]
        [HttpGet]
        public IEnumerable<UserPreferenceModel> Get()
        {
            var query = _userPreferencesRepository.GetAllUserPreferences();

            var results = query.ToList().Select(s => _userPreferenceModelFactory.Create(s));

            return results;
        }

        [SingledOutAuthorization]
        [HttpGet]
        public HttpResponseMessage GetUserPreferenceByID(int id)
        {
            try
            {
                var userPreference = _userPreferencesRepository.GetUserPreference(id);
                if (userPreference != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userPreferenceModelFactory.Create(userPreference));
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
        public HttpResponseMessage Post([FromBody] UserPreferenceModel userPreferenceModel)
        {
            try
            {
                var entity = _userPreferenceModelFactory.Parse(userPreferenceModel);

                if (entity == null) Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read user preference from body");

                var result = _userPreferencesRepository.Insert(entity);
                if (result > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, _userPreferenceModelFactory.Create(entity));
                }
                
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save user preference to the database.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [SingledOutAuthorization]
        [HttpPut]
        public int UpdateUserPreference(UserPreference originalUserPreference, UserPreference updatedUserPreference)
        {
            return _userPreferencesRepository.Update(originalUserPreference, updatedUserPreference);
        }
    }
}
