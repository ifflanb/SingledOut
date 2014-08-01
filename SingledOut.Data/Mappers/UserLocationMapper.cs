using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SingledOut.Data.Entities;

namespace SingledOut.Data.Mappers
{
    public class UserLocationMapper : EntityTypeConfiguration<UserLocation>
    {
        public UserLocationMapper()
        {
            this.ToTable("UserLocations");

            this.HasKey(c => c.ID);
            this.Property(c => c.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.UserID).IsRequired();
            this.Property(c => c.Latitude).IsRequired();
            this.Property(c => c.Longitude).IsRequired();
            this.Property(c => c.PlaceName).IsRequired();
            this.Property(c => c.CreatedDate).IsRequired();
            this.Property(c => c.UpdateDate).IsRequired();
        }
    }
}
