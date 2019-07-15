using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class ProcessPaymentDTO
    {
        public string txnref { get; set; }
        public string payRef { get; set; }
        public string retRef { get; set; }
        
    }
}