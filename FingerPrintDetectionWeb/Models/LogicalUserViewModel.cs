using System.Collections.Generic;

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
                Errors.Add("پلنی انتخاب نشده است");
            return Errors.Count <= 0;

        }
    }
}

