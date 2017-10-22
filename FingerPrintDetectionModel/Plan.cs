using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "datetime2")]
        public DateTime StartTime { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime EndTime { get; set; }

    }
}
