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
    
    public partial class User
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Sex { get; set; }
        public string FacebookAccessToken { get; set; }
        public string FacebookUserName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> UserAnswerID { get; set; }
        public Nullable<int> UserLocationID { get; set; }
        public Nullable<int> UserQuestionID { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Nullable<System.Guid> AuthToken { get; set; }
        public Nullable<int> Age { get; set; }
        public string FacebookPhotoUrl { get; set; }
        public string Interests { get; set; }
    
        public virtual UserAnswer UserAnswer { get; set; }
        public virtual UserLocation UserLocation { get; set; }
        public virtual UserQuestion UserQuestion { get; set; }
    }
}
