//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SingledOut.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserLocation
    {
        public UserLocation()
        {
            this.Users = new HashSet<User>();
        }
    
        public int ID { get; set; }
        public int UserID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string PlaceName { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
    }
}