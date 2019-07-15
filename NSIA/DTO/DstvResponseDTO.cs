using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class DstvResponseDTO
    {
        public bool exists { get; set; }

        public int? SubscriberId { get; set; }
    }
}