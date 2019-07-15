using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class DstvPersonalOptionsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}