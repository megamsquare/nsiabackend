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
    
    public partial class ProfileImage
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public byte[] ProfileFile { get; set; }
        public System.DateTime PostedDate { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
