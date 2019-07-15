using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class VehicleInputDTO
    {
        public string UserID { get; set; }
        public Nullable<int> VehicleDetailID { get; set; }
        public string VehicleValue { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleDate { get; set; }
        public string CoverType { get; set; }
        public string VehicleType { get; set; }
    }
}