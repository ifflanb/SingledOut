namespace SingledOut.Model
{
    public class UserLocationModel : BaseModel
    {
        public int UserID { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
