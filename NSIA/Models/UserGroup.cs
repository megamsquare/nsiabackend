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
    
    public partial class UserGroup
    {
        public int Id { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string UserEmail { get; set; }
        public Nullable<bool> IsDisabled { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Group Group { get; set; }
    }
}
