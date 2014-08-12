using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.Controllers
{
    public class QuestionsController : ApiController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionModelFactory _questionModelFactory;

        public QuestionsController(
            IQuestionRepository questionRepository,
            IQuestionModelFactory questionModelFactory)
        {
            _questionRepository = questionRepository;
            _questionModelFactory = questionModelFactory;
        }
       
        public IEnumerable<QuestionModel> Get()
        {
            var query = _questionRepository.GetAllQuestions();

            var results = query.ToList().Select(s => _questionModelFactory.Create(s));

            return results;
        }

        public HttpResponseMessage GetQuestionByID(int id)
        {
            try
            {
                var question = _questionRepository.GetQuestionByID(id);
                if (question != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _questionModelFactory.Create(question));
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public int AddQuestion(Question user)
        {
            return _questionRepository.Insert(user);
        }

        public int UpdateQuestion(Question originalQuestion, Question updatedQuestion)
        {
            return _questionRepository.Update(originalQuestion, updatedQuestion);
        }
    }
}
