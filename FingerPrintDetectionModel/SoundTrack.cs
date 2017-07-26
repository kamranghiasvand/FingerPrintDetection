using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionModel
{
    public class SoundTrack
    {
        [Key]
        public long Id { get; set; }
        public string Uri { get; set; }
        public long Duration { get; set; }
        public SoundTrackType Type { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{{ Id:{Id}, Uri:{Uri}, Duration:{Duration}, Type:{Type}, Deleted:{Deleted}, Name:{Name} }}";
        }

    }

    public enum SoundTrackType
    {
        Mp3
    }
}
