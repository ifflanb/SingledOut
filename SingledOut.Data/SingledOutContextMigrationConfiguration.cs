using System.Data.Entity.Migrations;

namespace SingledOut.Data
{
    public class SingledOutContextMigrationConfiguration : DbMigrationsConfiguration<SingledOutContext>
    {
        public SingledOutContextMigrationConfiguration()
    {
        this.AutomaticMigrationsEnabled = true;
        this.AutomaticMigrationDataLossAllowed = true;
    }

#if DEBUG
        protected override void Seed(SingledOutContext context)
    {
       // new SingledOutDataSeeder(context).Seed();
    }
#endif
    }
}
