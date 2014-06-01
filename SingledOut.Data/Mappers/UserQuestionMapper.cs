using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SingledOut.Data.Entities;

namespace SingledOut.Data.Mappers
{
    public class UserQuestionMapper : EntityTypeConfiguration<UserQuestion>
    {
        public UserQuestionMapper()
        {
            this.ToTable("UserQuestions");

            this.HasKey(c => c.ID);
            this.Property(c => c.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.QuestionDescription).IsRequired();
            this.Property(c => c.CreatedDate).IsRequired();
            this.Property(c => c.UpdateDate).IsRequired();
        }
    }
}
