using System.Collections.Generic;
using Newtonsoft.Json;

namespace SingledOut.Model
{
    public class UserModel : BaseModel
    {
        public string FirstName	{ get; set; }

        public string Surname { get; set; }

        public string  Sex { get; set; }

        public string FacebookAccessToken { get; set; }

        public string FacebookUserName { get; set; }

        public IEnumerable<UserLocationModel> UserLocations { get; set; }

        public IEnumerable<UserQuestionModel> UserQuestions { get; set; }

        public IEnumerable<UserAnswerModel> UserAnswers { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
