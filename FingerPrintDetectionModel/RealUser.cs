using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionModel
{
    public class RealUser
    {
        [Key]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] FirstFinger{ get; set; }
        public byte[] SecondFinger { get; set; }
        public byte[] ThirdFinger { get; set; }
        public virtual LogicalUser LogicalUser { get; set; }
    }
}
