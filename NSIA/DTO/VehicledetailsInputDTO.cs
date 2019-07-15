using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class VehicledetailsInputDTO
    {
        public string RegNo { get; set; }
        public string ChasisNo { get; set; }
        public string EngineNo { get; set; }
      //  public string VehicleStatus { get; set; }
      //  public string OtherDetails { get; set; }
    }
}