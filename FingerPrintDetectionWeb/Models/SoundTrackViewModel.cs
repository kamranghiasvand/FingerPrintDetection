using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FingerPrintDetectionModel;

namespace FingerPrintDetectionWeb.Models
{
    public class SoundTrackViewModel
    {
        public long Id { get; set; }
        public string Uri { get; set; }
        public long Duration { get; set; }
        public SoundTrackType Type { get; set; }
        public string Name { get; set; }

    }
}
