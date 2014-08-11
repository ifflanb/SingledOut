using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SingledOut.Data.Entities;

namespace SingledOut.Data.Mappers
{
    public class UserMapper : EntityTypeConfiguration<User>
    {
        public UserMapper()
        {
            this.ToTable("Users");

            this.HasKey(c => c.ID);
            this.Property(c => c.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.FirstName).IsRequired();
            this.Property(c => c.FirstName).HasMaxLength(255);
            this.Property(c => c.Surname).IsRequired();
            this.Property(c => c.Surname).HasMaxLength(255);
            this.Property(c => c.Sex).IsRequired();
            this.Property(c => c.Age).IsOptional();
            this.Property(c => c.Email).HasMaxLength(255);
            this.Property(c => c.Password).HasMaxLength(255);
            this.Property(c => c.FacebookAccessToken).IsMaxLength();
            this.Property(c => c.FacebookUserName).HasMaxLength(255);
            this.Property(c => c.FacebookPhotoUrl).HasMaxLength(1000);
            this.Property(c => c.CreatedDate).IsRequired();
            this.Property(c => c.UpdateDate).IsRequired();
            this.Property(c => c.AuthToken).IsOptional();
            this.HasOptional(c => c.UserQuestions).WithMany().Map(s => s.MapKey("UserQuestionID"));
            this.HasOptional(c => c.UserAnswers).WithMany().Map(t => t.MapKey("UserAnswerID"));
            this.HasOptional(c => c.UserLocation).WithOptionalDependent().Map(t => t.MapKey("UserLocationID"));
        }
    }
}
