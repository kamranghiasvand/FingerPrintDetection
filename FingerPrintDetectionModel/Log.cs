using System;
using System.ComponentModel.DataAnnotations;

namespace FingerPrintDetectionModel
{
    public class Log
    {
        [Key]
        public long Id { get; set; }
        public bool Income { get; set; }
        public DateTime Time { get; set; }
        public long RealUserId { get; set; }
        public long LogicalUserId { get; set; }
        public long PlanId { get; set; }
        public bool Deleted { get; set; }
    }

}
