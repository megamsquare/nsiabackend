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
    
    public partial class BurglaryQuote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BurglaryQuote()
        {
            this.BurglaryPayments = new HashSet<BurglaryPayment>();
            this.BurglaryRenewals = new HashSet<BurglaryRenewal>();
            this.BurglaryUserPolicies = new HashSet<BurglaryUserPolicy>();
        }
    
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Class { get; set; }
        public string AssetDescription { get; set; }
        public string AssetLocation { get; set; }
        public decimal StockValue { get; set; }
        public Nullable<decimal> Premium { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<bool> IsMobile { get; set; }
        public Nullable<bool> IsRenewal { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BurglaryPayment> BurglaryPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BurglaryRenewal> BurglaryRenewals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BurglaryUserPolicy> BurglaryUserPolicies { get; set; }
    }
}
