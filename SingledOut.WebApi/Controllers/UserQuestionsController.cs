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
    public class UserQuestionsController : ApiController
    {
        private readonly IUserQuestionRepository _userQuestionRepository;
        private readonly IUserQuestionModelFactory _userQuestionModelFactory;

        public UserQuestionsController(
            IUserQuestionRepository userQuestionRepository,
            IUserQuestionModelFactory userQuestionModelFactory)
        {
            _userQuestionRepository = userQuestionRepository;
            _userQuestionModelFactory = userQuestionModelFactory;
        }

        public IEnumerable<UserQuestionModel> Get()
        {
            var query = _userQuestionRepository.GetAllUserQuestions();

            var results = query.ToList().Select(s => _userQuestionModelFactory.Create(s));

            return results;
        }

        public HttpResponseMessage GetUserQuestionByID(int id)
        {
            try
            {
                var userQuestion = _userQuestionRepository.GetUserQuestion(id);
                if (userQuestion != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userQuestionModelFactory.Create(userQuestion));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public int AddUserQuestion(UserQuestion user)
        {
            return _userQuestionRepository.Insert(user);
        }

        public int UpdateUserQuestion(UserQuestion originalUserQuestion, UserQuestion updatedUserQuestion)
        {
            return _userQuestionRepository.Update(originalUserQuestion, updatedUserQuestion);
        }
    }
}
