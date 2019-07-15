using AutoMapper;
using NSIA.DTO;
using NSIA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NSIA.Controllers.Api
{
    public class NsiaQuoteController : ApiController
    {
        public readonly NSIAMobileEntities _context;

        public NsiaQuoteController()
        {
            _context = new NSIAMobileEntities();
        }

        [HttpPost]
        [Route("NSIAMobile/api/GetQuote")]
        public IHttpActionResult GetMake([FromBody] QuoteInputDTO qotInputDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var quote = Mapper.Map<QuoteInputDTO, Quote>(qotInputDto);
            //save quote
            _context.Quotes.Add(quote);
            _context.SaveChanges();
            return Ok();
        }
    }
}
