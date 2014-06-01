using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserAnswerModelFactory : IUserAnswerModelFactory
    {
        private readonly UrlHelper _urlHelper;

        public UserAnswerModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public IEnumerable<UserAnswerModel> Create(IEnumerable<UserAnswer> userAnswers)
        {
            var userAnswersModel = new Collection<UserAnswerModel>();

            foreach (var userAnswer in userAnswers)
            {
                userAnswersModel.Add(Create(userAnswer));
            }
            return userAnswersModel;
        }

        public UserAnswerModel Create(UserAnswer userAnswer)
        {
            return new UserAnswerModel
            {
                ID = userAnswer.ID,
                Url = _urlHelper.Link("UserAnswer", new { id = userAnswer.ID }),
                UserID = userAnswer.UserID,
                AnswerDescription = userAnswer.AnswerDescription,
                CreatedDate = userAnswer.CreatedDate,
                UpdateDate = userAnswer.UpdateDate
            };
        }
    }
}