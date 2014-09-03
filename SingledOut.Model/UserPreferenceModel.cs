namespace SingledOut.Model
{
    public class UserPreferenceModel : BaseModel
    {
        public int UserID { get; set; }

        public string Sex { get; set; }

        public int? Age { get; set; }

        public int? Distance { get; set; }

        public bool? DisplayProfilePicture { get; set; }
    }
}
