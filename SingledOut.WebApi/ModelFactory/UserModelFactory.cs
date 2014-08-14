using System;
using System.Collections.Generic;
using System.Web;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.Repository;
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
        public IEnumerable<UserModel> Create(IEnumerable<User> users)
        {
            var userModels = new List<UserModel>();

            foreach (var user in users)
            {
                userModels.Add(new UserModel
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
                        UserLocation = user.UserLocation != null ? _userLocationModelFactory.Create(user.UserLocation) : null,
                        Interests = user.Interests
                    });
            }

            return userModels;
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
                Interests = user.Interests
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
                    Interests = model.Interests
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
    }
}