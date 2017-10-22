using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FingerPrintDetectionModel
{
    public class LogicalUser
    {
        [Key]
        public long Id { get; set; }
        public bool Deleted { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual SoundTrack Sound { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual ICollection<RealUser> RealUsers { get; set; } = new List<RealUser>();

    }
}
