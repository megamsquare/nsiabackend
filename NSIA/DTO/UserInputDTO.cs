using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    [JsonObject(IsReference = false)]
    public class UserInputDTO
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public bool IsCoperate { get; set; }
        public string RegCode { get; set; }
        //public Nullable<System.DateTime> IssuedDate { get; set; }
        //public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string CompanyName { get; set; }
        public System.DateTime RegisterationDate { get; set; }
        //public string IdMeans { get; set; }
        //public string IdNumber { get; set; }
        public string Address { get; set; }
        //public string City { get; set; }
        //public string Country { get; set; }
        //public string State { get; set; }
        //public string PostCode { get; set; }
    }
}