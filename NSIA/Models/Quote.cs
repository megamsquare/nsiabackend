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
    
    public partial class Quote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quote()
        {
            this.CustDocumentMotors = new HashSet<CustDocumentMotor>();
            this.NIIDMotors = new HashSet<NIIDMotor>();
            this.PaymentDetails = new HashSet<PaymentDetail>();
            this.Renewals = new HashSet<Renewal>();
            this.UserPolicies = new HashSet<UserPolicy>();
        }
    
        public int ID { get; set; }
        public string UserId { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public Nullable<decimal> InitialPremium { get; set; }
        public Nullable<decimal> PurchaseDiscount { get; set; }
        public Nullable<decimal> FinalPremium { get; set; }
        public string FinalPremiumString { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<bool> IsComprehensive { get; set; }
        public Nullable<bool> CanPay { get; set; }
        public string Cover { get; set; }
        public Nullable<bool> IsMobile { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public Nullable<bool> IsRenewal { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public byte[] MyTimestamp { get; set; }
        public Nullable<int> BizId { get; set; }
        public Nullable<bool> HasExcess { get; set; }
        public Nullable<bool> HasTracker { get; set; }
        public string UsageType { get; set; }
        public string Referral { get; set; }
        public string Name { get; set; }
        public Nullable<int> Coverperiod { get; set; }
        public string InsuredName { get; set; }
        public string InsuredAddress { get; set; }
        public string Reference { get; set; }
        public Nullable<bool> IsLive { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> LgaId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual BusinessClass BusinessClass { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustDocumentMotor> CustDocumentMotors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NIIDMotor> NIIDMotors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Renewal> Renewals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserPolicy> UserPolicies { get; set; }
    }
}
