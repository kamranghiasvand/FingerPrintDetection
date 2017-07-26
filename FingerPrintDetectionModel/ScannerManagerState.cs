using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintDetectionModel
{
   public class ScannerManagerState
    {
        [Key]
        public long Id { get; set; }
        public bool Started { get; set; }
    }
}
