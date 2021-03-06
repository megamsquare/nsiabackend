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
    
    public partial class InvestLinkQuote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InvestLinkQuote()
        {
            this.InvestPayments = new HashSet<InvestPayment>();
            this.InvRenewals = new HashSet<InvRenewal>();
        }
    
        public int Id { get; set; }
        public string UserID { get; set; }
        public System.DateTime Dob { get; set; }
        public int tenor { get; set; }
        public decimal contribution { get; set; }
        public System.DateTime PostedOn { get; set; }
        public string Reference { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvestPayment> InvestPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvRenewal> InvRenewals { get; set; }
    }
}
