using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.DTO
{
    public class DstvCustomerDetailsInputDTO
    {
        public string Title { get; set; }

        public string Surname { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string SubscriberAddress { get; set; }

        public string DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string IdentificationType { get; set; }

        public string IdentificationNumber { get; set; }

        public string Profession { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}