using System.Data.Entity;
using SingledOut.Data.Entities;
using SingledOut.Data.Mappers;

namespace SingledOut.Data
{
    public class SingledOutContext : DbContext
    {
        public SingledOutContext() : base("SingledOut")
    {
        Configuration.ProxyCreationEnabled = false;
        Configuration.LazyLoadingEnabled = false;

        Database.SetInitializer(new MigrateDatabaseToLatestVersion<SingledOutContext,
               SingledOutContextMigrationConfiguration>());
    }

    public DbSet<User> Users {get;set;}
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<UserQuestion> UserQuestions { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<UserLocation> UserLocations { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Configurations.Add(new UserMapper());
        modelBuilder.Configurations.Add(new QuestionMapper());
        modelBuilder.Configurations.Add(new AnswerMapper());
        modelBuilder.Configurations.Add(new UserQuestionMapper());
        modelBuilder.Configurations.Add(new UserAnswerMapper());
        modelBuilder.Configurations.Add(new UserLocationMapper());

        base.OnModelCreating(modelBuilder);
    }
    }
}
