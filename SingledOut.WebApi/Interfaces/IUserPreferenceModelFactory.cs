using System.Collections.Generic;
using SingledOut.Data;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserPreferenceModelFactory
    {
        IEnumerable<UserPreferenceModel> Create(IEnumerable<UserPreferenceModel> userPreferences);

        UserPreferenceModel Create(UserPreference userPreference);

        UserPreference Parse(UserPreferenceModel model);
    }
}
