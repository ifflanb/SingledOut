using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.SearchParameters;
using SingledOut.Services.Interfaces;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserModelFactory : IUserModelFactory
    {
        private readonly IUserQuestionModelFactory _userQuestionModelFactory;
        private readonly IUserLocationModelFactory _userLocationModelFactory;
        private readonly IUserAnswerModelFactory _userAnswerModelFactory;
        private readonly IUserQuestionRepository _userQuestionRepository;
        private readonly ISecurity _security;

        public UserModelFactory(
            IUserLocationModelFactory userLocationModelFactory,
            IUserQuestionRepository userQuestionRepository,
            ISecurity security
            )
            //,
            //IUserQuestionModelFactory userQuestionModelFactory,
            //IUserAnswerModelFactory userAnswerModelFactory)
        {
            _userLocationModelFactory = userLocationModelFactory;
            _userQuestionRepository = userQuestionRepository;
            _security = security;
            //_userQuestionModelFactory = userQuestionModelFactory;
            //_userAnswerModelFactory = userAnswerModelFactory;
        }

        /// <summary>
        /// Creates a list of user models from a list of users.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public IEnumerable<UserModel> Create(IEnumerable<User> users, UsersSearchParameters sp)
        {
            var userModels = new List<UserModel>();

            foreach (var user in users)
            {
                userModels.Add(new UserModel
                    {
                        ID = user.ID,
                        Age = user.Age,
                        FirstName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(user.FirstName),
                        Surname = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(user.Surname),
                        FacebookAccessToken = user.FacebookAccessToken,
                        FacebookUserName = user.FacebookUserName,
                        FacebookPhotoUrl = HttpUtility.UrlDecode(user.FacebookPhotoUrl),
                        Sex = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(user.Sex),
                        CreatedDate = user.CreatedDate,
                        UpdateDate = user.UpdateDate,
                        Email = user.Email.ToLower(),
                        AuthToken = (Guid)user.AuthToken,
                        UserLocation = user.UserLocation != null ? _userLocationModelFactory.Create(user.UserLocation) : null,
                        Interests = user.Interests,
                        ProfilePicture = user.ProfilePicture,
                        DistanceFromUser = user.UserLocation != null && sp.UserLatitude.HasValue && sp.UserLongitude.HasValue ? GetDistanceFromLatLonInKm((double)sp.UserLatitude, (double)sp.UserLongitude, user.UserLocation.Latitude, user.UserLocation.Longitude) : (double?)null
                    });
            }

            userModels = userModels.OrderBy(o => o.DistanceFromUser).ThenBy(x => x.FirstName).ToList();

            return userModels;
        }

        private double GetDistanceFromLatLonInKm(double lat1,
                                 double lon1,
                                 double lat2,
                                 double lon2)
        {
            var R = 6371d; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = R * c; // Distance in km
            d = Math.Round(d, 2);
            return d;
        }

        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }

        public UserModel Create(User user)
        {
            return new UserModel
            {
                ID = user.ID,
                Age = user.Age,
                FirstName = user.FirstName,
                Surname = user.Surname,
                FacebookAccessToken = user.FacebookAccessToken,
                FacebookUserName = user.FacebookUserName,
                FacebookPhotoUrl = HttpUtility.UrlDecode(user.FacebookPhotoUrl),
                Sex = user.Sex,
                CreatedDate = user.CreatedDate,
                UpdateDate = user.UpdateDate,
                Email = user.Email,
                AuthToken = (Guid)user.AuthToken,
                Interests = user.Interests,
                ProfilePicture = user.ProfilePicture
                //Password = user.Password
                //UserQuestions = _userQuestionModelFactory.Create(user.UserQuestions),
                //UserLocations = _userLocationModelFactory.Create(user.UserLocations),
                //UserAnswers = _userAnswerModelFactory.Create(user.UserAnswers)
            };
        }

        public User Parse(UserModel model)
        {
            try
            {
                var user = new User()
                {
                    ID = model.ID,
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Sex = model.Sex,
                    Age = model.Age,
                    FacebookUserName = model.FacebookUserName,
                    FacebookAccessToken = model.FacebookAccessToken,
                    FacebookPhotoUrl = model.FacebookPhotoUrl,
                    CreatedDate = model.CreatedDate,
                    UpdateDate = model.UpdateDate,
                    Email = model.Email,
                    Password = model.Password ,
                    AuthToken = model.AuthToken,
                    Interests = model.Interests,
                    ProfilePicture = model.ProfilePicture
                    //!string.IsNullOrEmpty(model.Password) ? _security.CreateHash(model.Password) : string.Empty
                    //UserQuestions = _userQuestionRepository.GetUserQuestion(model.UserQuestions.)
                    //CourseTutor = _repo.GetTutor(model.Tutor.Id)

                };

                return user;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public User ParseUpdate(User originalUser, UserModel userModel)
        {
            try
            {
                var somethingChanged = false;

                if (originalUser.FirstName != userModel.FirstName)
                {
                    originalUser.FirstName = userModel.FirstName;
                    somethingChanged = true;
                }
                if (originalUser.Surname != userModel.Surname)
                {
                    originalUser.Surname = userModel.Surname;
                    somethingChanged = true;
                }
                if (originalUser.Sex != userModel.Sex)
                {
                    originalUser.Sex = userModel.Sex;
                    somethingChanged = true;
                }
                if (originalUser.Age != userModel.Age)
                {
                    originalUser.Age = userModel.Age;
                    somethingChanged = true;
                }
                if (originalUser.FacebookUserName != userModel.FacebookUserName)
                {
                    originalUser.FacebookUserName = userModel.FacebookUserName;
                    somethingChanged = true;
                }
                if (originalUser.FacebookAccessToken != userModel.FacebookAccessToken)
                {
                    originalUser.FacebookAccessToken = userModel.FacebookAccessToken;
                    somethingChanged = true;
                }
                if (originalUser.FacebookPhotoUrl != userModel.FacebookPhotoUrl)
                {
                    originalUser.FacebookPhotoUrl = userModel.FacebookPhotoUrl;
                    somethingChanged = true;
                }
                if (originalUser.Email != userModel.Email)
                {
                    originalUser.Email = userModel.Email;
                    somethingChanged = true;
                }
                if (originalUser.Interests != userModel.Interests)
                {
                    originalUser.Interests = userModel.Interests;
                    somethingChanged = true;
                }
                if (originalUser.ProfilePicture != userModel.ProfilePicture)
                {
                    originalUser.ProfilePicture = userModel.ProfilePicture;
                    somethingChanged = true;
                }
                if (somethingChanged)
                {
                    originalUser.UpdateDate = DateTime.UtcNow;
                }
                
                //!string.IsNullOrEmpty(model.Password) ? _security.CreateHash(model.Password) : string.Empty
                //UserQuestions = _userQuestionRepository.GetUserQuestion(model.UserQuestions.)
                //CourseTutor = _repo.GetTutor(model.Tutor.Id)

                return originalUser;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}