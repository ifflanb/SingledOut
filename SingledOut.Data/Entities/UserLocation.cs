using System;

namespace SingledOut.Data.Entities
{
    public class UserLocation : BaseEntity
    {
        public int UserID { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
