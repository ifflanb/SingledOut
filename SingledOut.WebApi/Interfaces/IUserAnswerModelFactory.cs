using System.Collections.Generic;
using SingledOut.Data.Entities;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserAnswerModelFactory
    {
        IEnumerable<UserAnswerModel> Create(IEnumerable<UserAnswer> userAnswers);

        UserAnswerModel Create(UserAnswer userAnswer);
    }
}
