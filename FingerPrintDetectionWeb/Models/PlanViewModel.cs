using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionWeb.Models
{
    public class PlanViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RepeatNumber { get; set; }
        public int UserCount { get; set; }
    }
}
