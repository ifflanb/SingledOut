using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public class AnswerRepository : BaseRepository, IAnswerRepository
    {
        private readonly SingledOutEntities _ctx;

        public AnswerRepository(SingledOutEntities ctx)
            : base(ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Answer> GetAllAnswers()
        {
            return _ctx.Answers.AsQueryable();
        }

        public Answer GetAnswerByID(int answerID)
        {
            return _ctx.Answers.SingleOrDefault(o => o.ID == answerID);
        }

        public int Insert(Answer answer)
        {
            _ctx.Answers.Add(answer);
            return SaveAll();
        }

        public int Update(Answer originalAnswer, Answer updatedAnswer)
        {
            _ctx.Answers.Attach(updatedAnswer);
            var entry = _ctx.Entry(updatedAnswer);

            if (originalAnswer.AnswerDescription != updatedAnswer.AnswerDescription)
            {
                entry.Property(e => e.AnswerDescription).IsModified = true;
            }

            if (originalAnswer.CreatedDate != updatedAnswer.CreatedDate)
            {
                entry.Property(e => e.CreatedDate).IsModified = true;
            }

            if (originalAnswer.UpdateDate != updatedAnswer.UpdateDate)
            {
                entry.Property(e => e.UpdateDate).IsModified = true;
            }

            return SaveAll();
        }

        public int DeleteAnswer(int id)
        {
            var answer = GetAnswerByID(id);
            _ctx.Answers.Remove(answer);
            return SaveAll();
        }
    }
}
