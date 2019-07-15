using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class PaymentResponseDTO
    {
        public string responseCode { get; set; }
        public string responsedescription { get; set; }
        public string txtref { get; set; }
        public string payref { get; set; }
        public string retref { get; set; }
    }
}