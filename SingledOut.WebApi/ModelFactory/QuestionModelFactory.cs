using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class QuestionModelFactory : IQuestionModelFactory
    {
        private readonly UrlHelper _urlHelper;

        public QuestionModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public QuestionModel Create(Question question)
        {
            return new QuestionModel
            {
                ID = question.ID,
                Url = _urlHelper.Link("Questions", new { id = question.ID }),
                QuestionDescription = question.QuestionDescription,
                CreatedDate = question.CreatedDate,
                UpdateDate = question.UpdateDate
            };
        }
    }
}