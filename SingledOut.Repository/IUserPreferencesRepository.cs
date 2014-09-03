using System.Linq;
using SingledOut.Data;

namespace SingledOut.Repository
{
    public interface IUserPreferencesRepository
    {
        IQueryable<UserPreference> GetAllUserPreferences();

        UserPreference GetUserPreference(int userPreferenceID);

        int Insert(UserPreference userPreference);

        int Update(UserPreference originalUserPreference, UserPreference updatedUserPreference);
    }
}
