using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FingerPrintDetectionModel
{
    public class RealUser
    {
        [Key]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] FirstFinger { get; set; }
        public byte[] SecondFinger { get; set; }
        public byte[] ThirdFinger { get; set; }
        public virtual LogicalUser LogicalUser { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Birthday { get; set; }
        public bool Deleted { get; set; }
    }
}
