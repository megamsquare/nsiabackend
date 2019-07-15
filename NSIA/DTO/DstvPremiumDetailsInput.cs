using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class DstvPremiumDetailsInput
    {
        public string DstvSubscriberId { get; set; }

        public string DstvPolicy { get; set; }

        public string HomeType { get; set; }

        public string PersonalAccident { get; set; }

        public string SumInsured { get; set; }

        public string AnnualEmolument { get; set; }

        public string AssetDescription { get; set; }

        public string VehicleDate { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string Location { get; set; }
        public string PromoCode { get; set; }

        public string VehicleReg { get; set; }

        public string Chasis { get; set; }

        public string EngineNumbers { get; set; }

       
        public DateTime? creeated_at { get; set; }

        public DateTime? updated_at { get; set; }
        // public string Images { get; set; }

    }
}