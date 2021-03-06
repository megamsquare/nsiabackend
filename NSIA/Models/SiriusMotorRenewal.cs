//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NSIA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SiriusMotorRenewal
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PolicyNo { get; set; }
        public string RegNo { get; set; }
        public string MotorDetail { get; set; }
        public string ChasisNo { get; set; }
        public string PolicyClass { get; set; }
        public string BusinessClass { get; set; }
        public decimal Amount { get; set; }
        public string AmountString { get; set; }
        public string txtRef { get; set; }
        public string Payref { get; set; }
        public string ResponseCode { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Status { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string ResponseText { get; set; }
        public Nullable<bool> IsRenewal { get; set; }
        public Nullable<bool> Isdeleted { get; set; }
        public Nullable<System.DateTime> CoverStart { get; set; }
        public Nullable<System.DateTime> CoverEnd { get; set; }
        public string Cert { get; set; }
        public Nullable<int> covertype { get; set; }
        public string InsuredName { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
