using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EntityData;
using System.ComponentModel.DataAnnotations;

namespace HongfuDongWuan.Models
{
    public class CustomerModel
    {        
        public Nullable<System.DateTime> Month { get; set; }
        public string EndUser { get; set; }
        public Nullable<double> AverageStock { get; set; }
        public Nullable<double> TurnoverRate { get; set; }
        public Nullable<double> NumOfOverdue { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N}%", ApplyFormatInEditMode = true)]
        public Nullable<double> SlowSalesRate { get; set; }
        public Nullable<double> ReturnRate { get; set; }
        public Nullable<double> BreakageRate { get; set; }
        public Nullable<double> MonCumlShipQty { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<decimal> MonCumlShipAmt { get; set; }
        public string F12 { get; set; }
        public string CreditRating { get; set; }
        public Nullable<double> CreditScore { get; set; }
        public string CanRaiseCapital { get; set; }
        public Nullable<decimal> CapitalAmt { get; set; }
        public Nullable<double> CapExpRate { get; set; }
        public int ID { get; set; }

    }
}