using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public class QuestionRepository : BaseRepository,  IQuestionRepository
    {
        private readonly SingledOutEntities _ctx;

        public QuestionRepository(SingledOutEntities ctx)
            : base(ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Question> GetAllQuestions()
        {
            return _ctx.Questions.AsQueryable();
        }

        public Question GetQuestionByID(int questionID)
        {
            return _ctx.Questions.SingleOrDefault(o => o.ID == questionID);
        }

        public int Insert(Question question)
        {
            _ctx.Questions.Add(question);
            return SaveAll();
        }

        public int Update(Question originalQuestion, Question updatedQuestion)
        {
            _ctx.Questions.Attach(updatedQuestion);
            var entry = _ctx.Entry(updatedQuestion);

            if (originalQuestion.QuestionDescription != updatedQuestion.QuestionDescription)
            {
                entry.Property(e => e.QuestionDescription).IsModified = true;
            }

            if (originalQuestion.CreatedDate != updatedQuestion.CreatedDate)
            {
                entry.Property(e => e.CreatedDate).IsModified = true;
            }

            if (originalQuestion.UpdateDate != updatedQuestion.UpdateDate)
            {
                entry.Property(e => e.UpdateDate).IsModified = true;
            }

            return SaveAll();
        }

        public int DeleteQuestion(int id)
        {
            var question = GetQuestionByID(id);
            _ctx.Questions.Remove(question);
            return SaveAll();
        }
    }
}
