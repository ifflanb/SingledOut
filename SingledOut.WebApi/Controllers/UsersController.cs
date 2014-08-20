﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.WebApi.Filters;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserModelFactory _userModelFactory;

        public UsersController(IUserRepository userRepository,
            IUserModelFactory userModelFactory)
        {
            _userRepository = userRepository;
            _userModelFactory = userModelFactory;
         
        }

        [SingledOutAuthorization]
        public IEnumerable<UserModel> Get()
        {
            var query = _userRepository.GetAllUsers();

            var results = query.ToList().Select(s => _userModelFactory.Create(s));

            return results;
        }

        [SingledOutAuthorization]
        public HttpResponseMessage GetUser(int id)
        {
            try
            {
                var user = _userRepository.GetUser(id);
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userModelFactory.Create(user));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        
        public HttpResponseMessage Post([FromBody] UserModel userModel)
        {
            try
            {
                var entity = _userModelFactory.Parse(userModel);

                if (entity == null) Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read user from body");

                var result = _userRepository.Insert(entity);
                if (result.ID > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, _userModelFactory.Create(result));
                }
                if (result.ID == -1) // account already exists.
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden)
                                                {
                                                    Content = new StringContent("This user name already exists."),
                                                    ReasonPhrase = "User name already exists"
                                                };
                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the database.");
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [SingledOutAuthorization]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage Put([FromBody] UserModel userModel)
        {
            try
            {
                // Get user to update.
                if (userModel == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read user from body");
                }

                var originalUser = _userRepository.GetUser(userModel.ID);

                if (originalUser == null || originalUser.ID != userModel.ID)
                {
                    return Request.CreateResponse(HttpStatusCode.NotModified, "User is not found");
                }

                var updatedUser = _userModelFactory.ParseUpdate(originalUser, userModel);
                
                if(_userRepository.Update(originalUser, updatedUser) > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userModelFactory.Create(updatedUser));
                }
                return Request.CreateResponse(HttpStatusCode.NotModified);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [SingledOutAuthorization]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var user = _userRepository.GetUser(id);

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                //if (user.UserQuestions != null && user.UserQuestions.Count > 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can not delete user, user has user questions.");
                //}

                //if (user.UserAnswer != null && user.UserAnswer.Count > 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can not delete user, user has user answers.");
                //}

                if (user.UserLocation != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can not delete user, user has user locations.");
                }

                if (_userRepository.DeleteUser(id) > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
       
    }
}
