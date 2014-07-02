namespace SingledOut.Data.Entities
{
    public class UserLocation : BaseEntity
    {
        public int UserID { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
