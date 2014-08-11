using System;
using System.Collections.Generic;

namespace SingledOut.Model
{
    public class UserModel : BaseModel
    {
        public string FirstName	{ get; set; }

        public string Surname { get; set; }

        public string  Sex { get; set; }

        public int? Age { get; set; }

        public string FacebookAccessToken { get; set; }

        public string FacebookUserName { get; set; }

        public string FacebookPhotoUrl { get; set; }

        public UserLocationModel UserLocation { get; set; }

        public IEnumerable<UserQuestionModel> UserQuestions { get; set; }

        public IEnumerable<UserAnswerModel> UserAnswers { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public Guid AuthToken { get; set; }
    }
}
