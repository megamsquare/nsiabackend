using NSIA.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NSIA.Models;

namespace NSIA.Controllers.Api
{
    public class NsiaClaimController : ApiController
    {
        public readonly NSIAMobileEntities _context;
        public NsiaClaimController()
        {
            _context = new NSIAMobileEntities();
        }
        [HttpPost]
        [Route("NSIAMobile/api/postclaim")]
        public IHttpActionResult PostClaim([FromBody] ClaimDTO claimDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid claim");

            var response = "";    
            // ClaimsMotor
            if (claimDto.Businessclass == "1")
            {
                var claim = new WebClaim
                {
                    bizclassId = claimDto.Businessclass,
                    bizclass = claimDto.Businessclassname,
                    policyno = claimDto.RegistrationNo,
                    description = claimDto.Description,
                    location = claimDto.Location,
                    lossDate = DateTime.Parse(claimDto.Dateofloss),
                    Email = claimDto.Email,
                    Phoneno = claimDto.Phoneno      
                };

                _context.WebClaims.Add(claim);
                _context.SaveChanges();
            }

            // ClaimsMarine ="2"
            //ClaimsFire & Burglary = "3"


            return Ok();
        }

    }
}
