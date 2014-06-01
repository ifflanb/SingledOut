using System.Linq;
using SingledOut.Data.Entities;

namespace SingledOut.Repository
{
    public interface IQuestionRepository
    {
        IQueryable<Question> GetAllQuestions();

        Question GetQuestionByID(int questionID);

        int Insert(Question question);

        int Update(Question originalQuestion, Question updatedQuestion);

        int DeleteQuestion(int id);
    }
}
