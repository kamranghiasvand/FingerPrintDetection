using System;
using System.Collections.Generic;

namespace FingerPrintDetectionWeb.Models
{
    public class RealUserViewModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool FirstFinger { get; set; }
        public bool SecondFinger { get; set; }
        public bool ThirdFinger { get; set; }
        public virtual LogicalUserViewModel LogicalUser { get; set; }
        public long LogicalUserId { get; set; } = -1;
        public string Birthday { get; set; }
        public bool Deleted { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(FirstName))
                Errors.Add("نام  وارد کنید");
            if (string.IsNullOrEmpty(LastName))
                Errors.Add("نام خانوادگی را وارد کنید");
            if (LogicalUserId < 0)
                Errors.Add("کابر منطقی را انتخاب کنید");
            if (string.IsNullOrEmpty(Birthday))
                Errors.Add("تاریخ تولد را وارد کنید");
            return Errors.Count == 0;
        }
    }
}
