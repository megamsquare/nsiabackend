using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace NSIA.DTO
{
   // [JsonObject(IsReference = false)]
    public class MotorDetails
    {
        public string ChasisNo { get; set; }
        public string Email { get; set; }
        public string EngineNo { get; set; }
        public string Firstname { get; set; }
        public bool HasTracker { get; set; }
        public string InsuredAddress { get; set; }
        public string InsuredName { get; set; }
        public int MotorClass { get; set; }
        public string PhoneNo { get; set; }
        public string RegNo { get; set; }
        public string Surname { get; set; }
        public int UsageType { get; set; }
        public int UserType { get; set; }
        public string VehicleDate { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleValue { get; set; }
        public int carduration { get; set; }
        public string companyName { get; set; }
        public string filebase64ID { get; set; }
        public string filebase64doc { get; set; }
        public string filenameID { get; set; }
        public string filenamedoc { get; set; }
        public string filetypeID { get; set; }
        public string filetypedoc { get; set; }
        public string lga { get; set; }
        public decimal premium { get; set; }
        public string state { get; set; }
        public int vehicletype { get; set; }

        public double purchaseDiscount { get; set; }
        public string reference { get; set; }
       // public string promoCode { get; set; }


    }
}