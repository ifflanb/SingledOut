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
    
    public partial class UserPreference
    {
        public int UserPreferencesID { get; set; }
        public int UserID { get; set; }
        public string Sex { get; set; }
        public Nullable<int> Age { get; set; }
        public Nullable<int> Distance { get; set; }
        public Nullable<bool> DisplayProfilePicture { get; set; }
    
        public virtual User User { get; set; }
    }
}