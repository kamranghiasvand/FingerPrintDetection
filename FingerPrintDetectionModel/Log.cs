﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FingerPrintDetectionModel
{
    public class Log
    {
        [Key]
        public long Id { get; set; }
        public bool Income { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Time { get; set; }
        public long RealUserId { get; set; }
        public long LogicalUserId { get; set; }
        public long PlanId { get; set; }
        public bool Deleted { get; set; }
        public override string ToString()
        {
            return
                $"{{ Id:{Id}, Income:{Income}, Time:{Time}, RealUserId:{RealUserId}, LogicalUserId:{LogicalUserId}, PlanId:{PlanId} , Deleted: {Deleted} }}";
        }
    }

}
