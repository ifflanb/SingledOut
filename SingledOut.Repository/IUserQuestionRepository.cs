using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public interface IUserQuestionRepository
    {
        IQueryable<UserQuestion> GetAllUserQuestions();

        UserQuestion GetUserQuestion(int userQuestionID);

        int Insert(UserQuestion userQuestion);

        int Update(UserQuestion originalUserQuestion, UserQuestion updatedUserQuestion);

        int DeleteUserQuestion(int id);
    }
}
