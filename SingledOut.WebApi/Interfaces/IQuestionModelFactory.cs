using SingledOut.Data;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IQuestionModelFactory
    {
        QuestionModel Create(Question question);
    }
}
