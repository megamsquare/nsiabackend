using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class LgaDTO
    {
        public int ID { get; set; }
        public string LgaName { get; set; }
    }
}