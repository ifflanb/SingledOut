﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SingledOut.Data.Entities
{
    public class User : BaseEntity
    {
        public string FirstName	{ get; set; }

        public string Surname { get; set; }

        public string  Sex { get; set; }

        public int? Age { get; set; }

        public string FacebookAccessToken { get; set; }

        public string FacebookUserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [ForeignKey("UserLocation")]
        public int? UserLocationId { get; set; }
        public UserLocation UserLocation { get; set; }

        public ICollection<UserQuestion> UserQuestions { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }

        public Guid AuthToken { get; set; }
    }
}
