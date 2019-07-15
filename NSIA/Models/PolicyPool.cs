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
    
    public partial class PolicyPool
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public int BusinessID { get; set; }
        public string Description { get; set; }
        public string PolicyNo { get; set; }
        public System.DateTime CoverStart { get; set; }
        public System.DateTime CoverEnd { get; set; }
        public bool IsMobile { get; set; }
        public System.DateTime PostedDate { get; set; }
        public Nullable<bool> IsRenewal { get; set; }
        public string Option1 { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual BusinessClass BusinessClass { get; set; }
    }
}