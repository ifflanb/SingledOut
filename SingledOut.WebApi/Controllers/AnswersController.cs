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
    public class AnswersController : ApiController
    {
        private readonly IAnswerModelFactory _answerModelFactory;
        private readonly IAnswerRepository _answerRepository;

        public AnswersController(
            IAnswerRepository answerRepository,
            IAnswerModelFactory answerModelFactory)
        {
            _answerModelFactory = answerModelFactory;
            _answerRepository = answerRepository;
        }

        public IEnumerable<AnswerModel> Get()
        {
            var query = _answerRepository.GetAllAnswers();

            var results = query
            .ToList()
            .Select(s => _answerModelFactory.Create(s));

            return results;
        }

        public HttpResponseMessage GetAnswer(int id)
        {
            try
            {
                var answer = _answerRepository.GetAnswerByID(id);
                if (answer != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _answerModelFactory.Create(answer));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public int AddAnswer(Answer user)
        {
            return _answerRepository.Insert(user);
        }

        public int UpdateAnswer(Answer originalAnswer, Answer updatedAnswer)
        {
            return _answerRepository.Update(originalAnswer, updatedAnswer);
        }
    }
}
