using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace NSIA.DTO
{
    public class LoginResponse : HttpRequestBase
    {
        public string responseMsg { get; set; }
    }
}