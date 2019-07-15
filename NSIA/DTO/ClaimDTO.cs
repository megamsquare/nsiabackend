using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class ClaimDTO
    {
        public string Email { get; set; }
        public string Phoneno { get; set; }
        public string Businessclass { get; set; }
        public string Businessclassname { get; set; }
        public string Policyno { get; set; }
        public string RegistrationNo { get; set; }
        public string Location { get; set; }
        public string Dateofloss { get; set; }
        public string Description { get; set; }
        public string FilenameID { get; set; }
        public string FiletypeID { get; set; }
        public string FilestringId { get; set; }
        public string Filenamedoc { get; set; }
        public string Filetypedoc { get; set; }
        public string Filebase64doc { get; set; }
    }
}