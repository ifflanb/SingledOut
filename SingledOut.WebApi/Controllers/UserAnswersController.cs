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
    public class UserAnswersController : ApiController
    {
        private readonly IUserAnswersRepository _userAnswersRepository;
        private readonly IUserAnswerModelFactory _userAnswerModelFactory;

        public UserAnswersController(
            IUserAnswersRepository userAnswersRepository,
            IUserAnswerModelFactory userAnswerModelFactory
            )
        {
            _userAnswersRepository = userAnswersRepository;
            _userAnswerModelFactory = userAnswerModelFactory;
        }
 
        public IEnumerable<UserAnswerModel> Get()
        {
            var query = _userAnswersRepository.GetAllUserAnswers();

            var results = query.ToList().Select(s => _userAnswerModelFactory.Create(s));

            return results;
        }

        public HttpResponseMessage GetUserAnswerByID(int id)
        {
            try
            {
                var userAnswer = _userAnswersRepository.GetUserAnswer(id);
                if (userAnswer != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userAnswerModelFactory.Create(userAnswer));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public int AddUserAnswer(UserAnswer user)
        {
            return _userAnswersRepository.Insert(user);
        }

        public int UpdateUserAnswer(UserAnswer originalUserAnswer, UserAnswer updatedUserAnswer)
        {
            return _userAnswersRepository.Update(originalUserAnswer, updatedUserAnswer);
        }
    }
}
