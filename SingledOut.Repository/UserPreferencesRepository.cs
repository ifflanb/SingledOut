using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public class UserPreferencesRepository : BaseRepository, IUserPreferencesRepository
    {
        private readonly SingledOutEntities _ctx;

        public UserPreferencesRepository(SingledOutEntities ctx)
            : base(ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<UserPreference> GetAllUserPreferences()
        {
            return _ctx.UserPreferences.AsQueryable();
        }

        public UserPreference GetUserPreference(int userPreferenceID)
        {
            return _ctx.UserPreferences.SingleOrDefault(o => o.UserPreferencesID == userPreferenceID);
        }

        public int Insert(UserPreference userPreference)
        {
            // Check if there is already a user preference for this user.
            var existingUserPreferences = _ctx.UserPreferences.Where(o => o.UserID == userPreference.UserID).Select(o => o);
            if (existingUserPreferences.Any())
            {
                foreach (var existingUserPreference in existingUserPreferences)
                {
                    _ctx.UserPreferences.Remove(existingUserPreference);
                }
            }

            _ctx.UserPreferences.Add(userPreference);
            var result = SaveAll();
            var userPreferenceId = userPreference.UserPreferencesID;

            return userPreferenceId;
        }

        public int Update(UserPreference originalUserPreference, UserPreference updatedUserPreference)
        {
            _ctx.UserPreferences.Attach(updatedUserPreference);
            var entry = _ctx.Entry(updatedUserPreference);

            if (originalUserPreference.Age != updatedUserPreference.Age)
            {
                entry.Property(e => e.Age).IsModified = true;
            }

            if (originalUserPreference.Sex != updatedUserPreference.Sex)
            {
                entry.Property(e => e.Sex).IsModified = true;
            }

            if (originalUserPreference.Distance != updatedUserPreference.Distance)
            {
                entry.Property(e => e.Distance).IsModified = true;
            }

            if (originalUserPreference.DisplayProfilePicture != updatedUserPreference.DisplayProfilePicture)
            {
                entry.Property(e => e.DisplayProfilePicture).IsModified = true;
            }

            return SaveAll();
        }
    }
}
