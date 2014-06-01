using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserQuestionModelFactory: IUserQuestionModelFactory
    {
        private readonly UrlHelper _urlHelper;

        public UserQuestionModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public IEnumerable<UserQuestionModel> Create(IEnumerable<UserQuestion> userQuestions)
        {
            var userQuestionsModel = new Collection<UserQuestionModel>();

            foreach (var userQuestion in userQuestions)
            {
                userQuestionsModel.Add(Create(userQuestion));
            }
            return userQuestionsModel;
        }

        public UserQuestionModel Create(UserQuestion userQuestion)
        {
            return new UserQuestionModel
            {
                ID = userQuestion.ID,
                Url = _urlHelper.Link("UserQuestion", new { id = userQuestion.ID }),
                UserID = userQuestion.UserID,
                QuestionDescription = userQuestion.QuestionDescription,
                CreatedDate = userQuestion.CreatedDate,
                UpdateDate = userQuestion.UpdateDate
            };
        }
    }
}