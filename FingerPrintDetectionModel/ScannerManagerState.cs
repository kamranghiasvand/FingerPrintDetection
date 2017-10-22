using System.ComponentModel.DataAnnotations;

namespace FingerPrintDetectionModel
{
   public class ScannerManagerState
    {
        [Key]
        public long Id { get; set; }
        public bool Started { get; set; }
    }
}
