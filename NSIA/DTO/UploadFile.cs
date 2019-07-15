using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class UploadFile
    {
        [Required]
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}