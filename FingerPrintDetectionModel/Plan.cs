using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionModel
{
    public class Plan
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Description { get; set; }
        public int RepeatNumber { get; set; }
        public int MaxNumberOfUse { get; set; }
        public virtual ICollection<LogicalUser> Users { get; set; } = new List<LogicalUser>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
