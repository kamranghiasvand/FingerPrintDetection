using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionWeb.Models
{
    public class LogicalUserViewModel
    {
       public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long PlanId { get; set; }
        public PlanViewModel Plan { get; set; }
        public long SoundTrackId { get; set; } = -1;
        public SoundTrackViewModel SoundTrack { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public bool IsValid()
        {
            if (PlanId < 0)
                Errors.Add("Plan is not selected");
            return Errors.Count <= 0;

        }
    }
}

