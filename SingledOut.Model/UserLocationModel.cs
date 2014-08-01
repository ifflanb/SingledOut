namespace SingledOut.Model
{
    public class UserLocationModel : BaseModel
    {
        public int UserID { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PlaceName { get; set; }
    }
}
