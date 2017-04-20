using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionWeb.Models
{
    public class RealUserViewModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool FirstFinger { get; set; }
        public bool SecondFinger { get; set; }
        public bool ThirdFinger { get; set; }
        public virtual LogicalUserViewModel LogicalUser { get; set; }
        public long LogicalUserId { get; set; }

        public bool Deleted { get; set; }
    }
}
