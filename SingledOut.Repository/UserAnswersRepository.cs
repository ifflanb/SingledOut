using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public class UserAnswersRepository : BaseRepository, IUserAnswersRepository
    {
        private readonly SingledOutEntities _ctx;

        public UserAnswersRepository(SingledOutEntities ctx)
            : base(ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<UserAnswer> GetAllUserAnswers()
        {
            return _ctx.UserAnswers.AsQueryable();
        }

        public UserAnswer GetUserAnswer(int userAnswerID)
        {
            return _ctx.UserAnswers.SingleOrDefault(o => o.ID == userAnswerID);
        }

        public int Insert(UserAnswer userAnswer)
        {
            _ctx.UserAnswers.Add(userAnswer);
            return SaveAll();
        }

        public int Update(UserAnswer originalUserAnswer, UserAnswer updatedUserAnswer)
        {
            _ctx.UserAnswers.Attach(updatedUserAnswer);
            var entry = _ctx.Entry(updatedUserAnswer);

            if (originalUserAnswer.AnswerDescription != updatedUserAnswer.AnswerDescription)
            {
                entry.Property(e => e.AnswerDescription).IsModified = true;
            }

            if (originalUserAnswer.CreatedDate != updatedUserAnswer.CreatedDate)
            {
                entry.Property(e => e.CreatedDate).IsModified = true;
            }

            if (originalUserAnswer.UpdateDate != updatedUserAnswer.UpdateDate)
            {
                entry.Property(e => e.UpdateDate).IsModified = true;
            }

            return SaveAll();
        }

        public int DeleteUserAnswer(int id)
        {
            var userAnswer = GetUserAnswer(id);
            _ctx.UserAnswers.Remove(userAnswer);
            return SaveAll();
        }
    }
}
