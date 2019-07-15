using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class dstvPaymentResponseDTO
    {
        public string product_id { get; set; }
        public string cust_id { get; set; }
        public string currency { get; set; }

        public string businessClass { get; set; }
        public string customerName { get; set; }
        public string hash { get; set; }
        public decimal premium { get; set; }

        public decimal originalPremium { get; set; }
        public string premiumString { get; set; }
        public string transactionRefNo { get; set; }
        public string payItemId { get; set; }
        public string redirect_url { get; set; }


    }
}