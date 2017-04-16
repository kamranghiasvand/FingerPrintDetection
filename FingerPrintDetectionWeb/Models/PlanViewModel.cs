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
        public int MaxUserCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Errors.Add("نام پلن را مشخص کنید");
                return false;
            }
            if (RepeatNumber <= 0)
            {
                Errors.Add("تعداد تکرار حداقل یک باید باشد");
                return false;
            }
            if (MaxUserCount <= 0)
            {
                Errors.Add("حداکثر کاربران مجاز حداقل یک باید باشد");
                return false;
            }
            if (EndTime.CompareTo(StartTime)<=0)
            {
                Errors.Add("باید زمان شروع از زمان پایان کوچکتر باشد");
                return false;
            }

            return true;
        }
    }
}
