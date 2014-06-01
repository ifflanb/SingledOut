using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SingledOut.Data.Entities;

namespace SingledOut.Data.Mappers
{
    public class AnswerMapper : EntityTypeConfiguration<Answer>
    {
        public AnswerMapper()
        {
            this.ToTable("Answers");

            this.HasKey(c => c.ID);
            this.Property(c => c.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.CreatedDate).IsRequired();
            this.Property(c => c.UpdateDate).IsRequired();
            this.Property(c => c.AnswerDescription).IsRequired();
        }
    }
}
