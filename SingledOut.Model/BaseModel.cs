using System;

namespace SingledOut.Model
{
    public abstract class BaseModel
    {
        public int ID { get; set; }

        public string Url { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
