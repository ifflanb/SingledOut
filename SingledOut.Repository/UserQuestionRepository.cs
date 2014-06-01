using System.Linq;
using SingledOut.Data;
using SingledOut.Data.Entities;

namespace SingledOut.Repository
{
    public class UserQuestionRepository : BaseRepository, IUserQuestionRepository
    {
        private readonly SingledOutContext _ctx;

        public UserQuestionRepository(SingledOutContext ctx)
            : base(ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<UserQuestion> GetAllUserQuestions()
        {
            return _ctx.UserQuestions.AsQueryable();
        }

        public UserQuestion GetUserQuestion(int userQuestionID)
        {
            return _ctx.UserQuestions.SingleOrDefault(o => o.ID == userQuestionID);
        }

        public int Insert(UserQuestion userQuestion)
        {
            _ctx.UserQuestions.Add(userQuestion);
            return SaveAll();
        }

        public int Update(UserQuestion originalUserQuestion, UserQuestion updatedUserQuestion)
        {
            _ctx.UserQuestions.Attach(updatedUserQuestion);
            var entry = _ctx.Entry(updatedUserQuestion);

            if (originalUserQuestion.QuestionDescription != updatedUserQuestion.QuestionDescription)
            {
                entry.Property(e => e.QuestionDescription).IsModified = true;
            }

            if (originalUserQuestion.CreatedDate != updatedUserQuestion.CreatedDate)
            {
                entry.Property(e => e.CreatedDate).IsModified = true;
            }

            if (originalUserQuestion.UpdateDate != updatedUserQuestion.UpdateDate)
            {
                entry.Property(e => e.UpdateDate).IsModified = true;
            }

            return SaveAll();
        }

        public int DeleteUserQuestion(int id)
        {
            var userQuestion = GetUserQuestion(id);
            _ctx.UserQuestions.Remove(userQuestion);
            return SaveAll();
        }
    }
}
