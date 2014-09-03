using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserPreferenceModelFactory : IUserPreferenceModelFactory
    {
        public IEnumerable<UserPreferenceModel> Create(IEnumerable<UserPreferenceModel> userPreferences)
        {
            var userPreferenceModel = new Collection<UserPreferenceModel>();

            return userPreferenceModel;
        }

        public UserPreferenceModel Create(UserPreference userPreference)
        {
            var userPreferenceModel = new UserPreferenceModel
            {
                ID = userPreference.UserPreferencesID,
                Age = userPreference.Age,
                Sex = userPreference.Sex,
                Distance = userPreference.Distance,
                DisplayProfilePicture = userPreference.DisplayProfilePicture
            };

            return userPreferenceModel;
        }

        public UserPreference Parse(UserPreferenceModel model)
        {
            try
            {
                var userPreferenceModel = new UserPreference
                {
                    UserID = model.UserID,
                    UserPreferencesID = model.ID,
                    Age = model.Age,
                    Sex = model.Sex,
                    Distance = model.Distance,
                    DisplayProfilePicture = model.DisplayProfilePicture
                };

                return userPreferenceModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}