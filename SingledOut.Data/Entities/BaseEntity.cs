using System;

namespace SingledOut.Data.Entities
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
    
        public DateTime CreatedDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
