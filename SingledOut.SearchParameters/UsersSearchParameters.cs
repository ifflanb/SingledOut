using SingledOut.Model;

namespace SingledOut.SearchParameters
{
    public class UsersSearchParameters
    {
        public string FirstName { get; set; }

        public string Surname { get; set; }

        public GenderEnum? Sex { get; set; }

        public string FacebookUserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int? AgeFrom { get; set; }

        public int? AgeTo { get; set; }

        public int? Distance { get; set; }

        public double? UserLatitude { get; set; }

        public double? UserLongitude { get; set; }
    }
}
