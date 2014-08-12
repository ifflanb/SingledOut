using SingledOut.Data;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IAnswerModelFactory
    {
        AnswerModel Create(Answer answer);
    }
}
