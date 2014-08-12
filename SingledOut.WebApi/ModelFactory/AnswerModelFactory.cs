using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class AnswerModelFactory  : IAnswerModelFactory
    {
        private readonly UrlHelper _urlHelper;

        public AnswerModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public AnswerModel Create(Answer answer)
        {
            return new AnswerModel
            {
                ID = answer.ID,
                Url = _urlHelper.Link("Answers", new { id = answer.ID }),
                AnswerDescription = answer.AnswerDescription,
                CreatedDate = answer.CreatedDate,
                UpdateDate = answer.UpdateDate
            };
        }
    }
}