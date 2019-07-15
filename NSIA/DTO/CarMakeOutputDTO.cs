using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class CarMakeOutputDTO
    {
        public int id { get; set; }
       
        public string title { get; set; }
    }
}