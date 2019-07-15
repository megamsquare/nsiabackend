using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class DstvTransactionDTO
    {
        public int Subscriber_id { get; set; }
       // public int DstvPremiumDetailsId { get; set; }
        public string TransactionRef { get; set; }
        public double Amount { get; set; }
        public string Hash { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime? deleted_at { get; set; } = null;
    }
}