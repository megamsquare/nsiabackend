using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class MotorResponseDTO
    {
        public string businessClass { get; set; }
        public string customerName { get; set; }
        public string hash { get; set; }
        public double premium { get; set; }
        public string premiumString { get; set; }
        public int quoteID { get; set; }
        public string reference { get; set; }
        public string transactionRefNo { get; set; }
        public string vehicledetail { get; set; }
        public string payItemId { get; set; }
    }
}