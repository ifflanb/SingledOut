using System.Linq;
using SingledOut.Data.Entities;

namespace SingledOut.Repository
{
    public interface IAnswerRepository
    {
        IQueryable<Answer> GetAllAnswers();

        Answer GetAnswerByID(int answerID);

        int Insert(Answer answer);

        int Update(Answer originalAnswer, Answer updatedAnswer);

        int DeleteAnswer(int id);
    }
}
