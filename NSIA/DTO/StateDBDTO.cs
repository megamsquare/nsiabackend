using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class StateDBDTO
    {
        public int ID { get; set; }
        public string StateName { get; set; }
    }
}