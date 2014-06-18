using System;
using System.Net.Http;
using System.Web.Http.Routing;
using SingledOut.Data.Entities;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.Services.Interfaces;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.ModelFactory
{
    public class UserModelFactory : IUserModelFactory
    {
        private UrlHelper _urlHelper;
        private readonly IUserQuestionModelFactory _userQuestionModelFactory;
        private readonly IUserLocationModelFactory _userLocationModelFactory;
        private readonly IUserAnswerModelFactory _userAnswerModelFactory;
        private readonly IUserQuestionRepository _userQuestionRepository;
        private readonly ISecurity _security;

        public UserModelFactory(
            IUserQuestionRepository userQuestionRepository,
            ISecurity security)
            //,
            //IUserQuestionModelFactory userQuestionModelFactory,
            //IUserLocationModelFactory userLocationModelFactory,
            //IUserAnswerModelFactory userAnswerModelFactory)
        {
            _userQuestionRepository = userQuestionRepository;
            _security = security;
            //_userQuestionModelFactory = userQuestionModelFactory;
            //_userLocationModelFactory = userLocationModelFactory;
            //_userAnswerModelFactory = userAnswerModelFactory;
        }

        public UserModel Create(User user, HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);

            return new UserModel
            {
                ID = user.ID,
                Url = _urlHelper.Link("Users", new { controller = "Users", id = user.ID }),
                FirstName = user.FirstName,
                Surname = user.Surname,
                FacebookAccessToken = user.FacebookAccessToken,
                FacebookUserName = user.FacebookUserName,
                Sex = user.Sex,
                CreatedDate = user.CreatedDate,
                UpdateDate = user.UpdateDate,
                Email = user.Email,
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
                    FacebookUserName = model.FacebookUserName,
                    FacebookAccessToken = model.FacebookAccessToken,
                    CreatedDate = model.CreatedDate,
                    UpdateDate = model.UpdateDate,
                    Email = model.Email,
                    Password = model.Password //!string.IsNullOrEmpty(model.Password) ? _security.CreateHash(model.Password) : string.Empty
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