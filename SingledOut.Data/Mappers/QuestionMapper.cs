using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SingledOut.Data.Entities;

namespace SingledOut.Data.Mappers
{
    public class QuestionMapper : EntityTypeConfiguration<Question>
    {
        public QuestionMapper()
        {
            this.ToTable("Questions");

            this.HasKey(c => c.ID);
            this.Property(c => c.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.CreatedDate).IsRequired();
            this.Property(c => c.UpdateDate).IsRequired();
            this.Property(c => c.QuestionDescription).IsRequired();
        }
    }
}
