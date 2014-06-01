using SingledOut.Data.Entities;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IQuestionModelFactory
    {
        QuestionModel Create(Question question);
    }
}
