using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public interface IUserAnswersRepository
    {
        IQueryable<UserAnswer> GetAllUserAnswers();

        UserAnswer GetUserAnswer(int userAnswerID);

        int Insert(UserAnswer userAnswer);

        int Update(UserAnswer originalUserAnswer, UserAnswer updatedUserAnswer);

        int DeleteUserAnswer(int id);
    }
}
