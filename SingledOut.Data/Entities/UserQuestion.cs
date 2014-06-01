namespace SingledOut.Data.Entities
{
    public class UserQuestion : BaseEntity
    {
        public int UserID { get; set; }

        public string QuestionDescription { get; set; }
    }
}
