using System.Collections.Generic;
using SingledOut.Data.Entities;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserQuestionModelFactory
    {
        IEnumerable<UserQuestionModel> Create(IEnumerable<UserQuestion> userQuestions);

        UserQuestionModel Create(UserQuestion userQuestion);
    }
}
