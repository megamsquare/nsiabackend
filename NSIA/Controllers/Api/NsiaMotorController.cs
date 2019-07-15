using AutoMapper;
using Excel;
using NSIA.DTO;
using NSIA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using Excel.Log;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;

namespace NSIA.Controllers.Api
{
	
	public class NsiaMotorController : ApiController
	{
		public readonly NSIAMobileEntities _context;

		public NsiaMotorController()
		{
			_context = new NSIAMobileEntities();
		}

		[HttpGet]
		[Route("NSIAMobile/api/GetMake")]
		public IHttpActionResult GetMake()
		{

			var vehicles = _context.Carmakes.ToList();
			var vehicledto = Mapper.Map<List<Carmake>, List<CarMakeOutputDTO>>(vehicles);

			return Ok(vehicledto);
		}

		[HttpPost]
		[Route("NSIAMobile/api/GetModel")]
		public IHttpActionResult GetModel([FromBody] CarMakeIDDTO makeDto)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid state id");

			var make = _context.Carmodels.Where(m => m.make_id == makeDto.id).ToList();

			if (make == null)
				return NotFound();

			var modeldto = Mapper.Map<List<Carmodel>, List<CarModelDTO>>(make);

			return Ok(modeldto);
		}

		[HttpGet]
		[Route("NSIAMobile/api/GetState")]
		public IHttpActionResult GetState()
		{
			var states = _context.StatesDBs.ToList();           
			var statedto = Mapper.Map<List<StatesDB>, List<StateDBDTO>>(states);

			return Ok(statedto);
		}

		[HttpPost]
		[Route("NSIAMobile/api/GetLga")]
		public IHttpActionResult GetLga([FromBody] StateIDDTO state)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid state id");

			var lga = _context.LGAs.Where(l => l.StateID == state.StateId).ToList();

			if (lga == null)
				return NotFound();

			var lgadto = Mapper.Map<List<LGA>, List<LgaDTO>>(lga);

			return Ok(lgadto);
		}

		[HttpPost]
		[Route("NSIAMobile/api/BuyMotorWeb")]
		public IHttpActionResult BuyMotorWeb([FromBody] MotorDetails motorDetails)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid Motor details");

			var userFound = _context.AspNetUsers.FirstOrDefault(u => u.Email == motorDetails.Email ||u.PhoneNumber == motorDetails.PhoneNo );

			var regTime = DateTime.UtcNow;
			var timeStamp = new DateTimeOffset(regTime).ToUnixTimeSeconds();


			if (userFound == null)
			{
				try
				{
					//var user = Mapper.Map<UserInputDTO, AspNetUser>(userInput);
					 userFound = new AspNetUser
					{
						Firstname = motorDetails.Firstname,
						Surname = motorDetails.Surname,
						Email = motorDetails.Email,
						PhoneNumber = motorDetails.PhoneNo,
						UserName = motorDetails.Email,
						IsCoperate = motorDetails.UsageType.ToString() == "2",
						RegCode = motorDetails.RegNo,
						CompanyName = motorDetails.companyName != "" ? motorDetails.companyName : null,
						RegisterationDate = regTime,
						Address = motorDetails.InsuredAddress,
						EmailConfirmed = false,
						PhoneNumberConfirmed = false,
						TwoFactorEnabled = false,
						LockoutEnabled = false,
						AccessFailedCount = 0,
						IsMobile = false,
						IsDeleted = false,
						Id = generateID() + "-" + RandomID()
					};
					_context.AspNetUsers.Add(userFound);
					_context.SaveChanges();

					userFound = _context.AspNetUsers.FirstOrDefault(u => u.Email == motorDetails.Email); // || u.PhoneNumber == motorDetails.PhoneNo
				}
				catch(DbEntityValidationException e)
				{
					return Ok(e);
				}

			}
		   

			//save the vehicle details
			var vehicledetailsInput = new VehicledetailsInputDTO
			{
				ChasisNo = motorDetails.ChasisNo,
				EngineNo = motorDetails.EngineNo,
				RegNo = motorDetails.RegNo
			};

			var vehicleDetail = Mapper.Map<VehicledetailsInputDTO, VehicleDetail>(vehicledetailsInput);
			_context.VehicleDetails.Add(vehicleDetail);
			_context.SaveChanges();

			var vehicleDetailFound = _context.VehicleDetails.FirstOrDefault(vd=>vd.RegNo==motorDetails.RegNo && vd.ChasisNo == motorDetails.ChasisNo && vd.EngineNo == motorDetails.EngineNo);
			//
			//save the vehicle 
			var vehicle = new Vehicle
			{
				UserID = userFound.Id,
				VehicleMake = motorDetails.VehicleMake,
				VehicleModel = motorDetails.VehicleModel,
				VehicleDetailID = vehicleDetailFound.ID,
				VehicleDate = motorDetails.VehicleDate,
				VehicleType = vehicleType(motorDetails.UsageType.ToString()),
				CoverType = userType(motorDetails.UserType.ToString()) + " |" +  motorClass(motorDetails.MotorClass.ToString()) + " |" + cardDuration(motorDetails.carduration.ToString()),
			   VehicleValue = motorDetails.VehicleValue.ToString()
				 };

		   // var vehicle = Mapper.Map<VehicleInputDTO, Vehicle>(vehiclerInput);
			_context.Vehicles.Add(vehicle);
			_context.SaveChanges();

			var vehicleFound = _context.Vehicles.FirstOrDefault(v => v.VehicleDetailID == vehicleDetailFound.ID);

		   // var intPremium = Convert.ToDouble(motorDetails.premium);

			byte[] filebaseDocBytes = Convert.FromBase64String(motorDetails.filebase64doc);
			byte[] filebaseIdBytes = Convert.FromBase64String(motorDetails.filebase64ID);

		   //var transRef = "NSI" + RandomID() + "-" + generateID();
			var transRef = "NSI" + generateID();
			transRef = transRef.Substring(0, 11);
			var custDocument = new CustDocument
			{
				IdType = "motor",
				FileType = motorDetails.filetypeID,
				FileTypeLicence = motorDetails.filetypedoc,
				IsAchived = false,
				UserId = userFound.Id,
				UploadedDate = regTime,
				IdMeans = filebaseIdBytes,
				IdLicense = filebaseDocBytes,
				TransRef = transRef 
			};

			_context.CustDocuments.Add(custDocument);
			_context.SaveChanges();

			// Check if there is a promo
			//if (modelDetails.IsDstv)
			//{

			//}
			//Insert  Quote table // approve should be updated after payment is done
			var quote = new Quote()
			{
				UserId = userFound.Id,
				VehicleID = vehicleFound.ID,
				InitialPremium = (decimal)motorDetails.premium,
				PurchaseDiscount = (decimal)motorDetails.purchaseDiscount,
				FinalPremium = ((decimal)motorDetails.premium - (decimal)motorDetails.purchaseDiscount),
				FinalPremiumString = ((decimal)motorDetails.premium - (decimal)motorDetails.purchaseDiscount).ToString(),
				Approved = false,
				IsComprehensive = motorClass(motorDetails.MotorClass.ToString()) == "Comprehensive",
				CanPay = true,
				Cover = cardDuration(motorDetails.carduration.ToString()),
				IsMobile = false,
				TransactionDate = regTime,
				IsRenewal = false,
				isDeleted = false,
			   // MyTimestamp = Convert.FromBase64String(timeStamp.ToString()),
				BizId = 1,
				HasExcess = false,
				UsageType = usageType(motorDetails.UsageType.ToString()),
				Referral = motorDetails.reference,
				Name = motorDetails.Surname + " " + motorDetails.Firstname,
				Coverperiod = motorDetails.carduration,
				InsuredName = motorDetails.Surname + " " + motorDetails.Firstname,
				InsuredAddress = motorDetails.InsuredAddress,
				Reference = (motorDetails.reference == null || motorDetails.reference == "") ? "N/A" : motorDetails.reference,
				IsLive = false,
				StateId = Convert.ToInt32(motorDetails.state),
				LgaId = Convert.ToInt32(motorDetails.lga)

			};

			_context.Quotes.Add(quote);
			_context.SaveChanges();

			var quoteFound = _context.Quotes.FirstOrDefault(q => q.UserId == userFound.Id && q.VehicleID == vehicleFound.ID);

			// Add to payment table
			var motorPayment = new PaymentDetail()
			{
				UserID = userFound.Id,
				txtRef = transRef,
				QuoteID = quoteFound.ID,
				amount = quoteFound.FinalPremium,
				AmountString = quoteFound.FinalPremium.ToString(),
				TransactionDate = regTime
			};

			_context.PaymentDetails.Add(motorPayment);
			_context.SaveChanges();

			//update promo table if promo
			//var promo = _context.Promos.FirstOrDefault(p=>p.Code == motorDetails.promoCode);
			//if (promo != null)
			//{
			//    promo.transactionRef = motorDetails.promoCode;
			//    _context.SaveChanges();
			//}
			

			var motorPaymentFound = _context.PaymentDetails.FirstOrDefault(p=> p.UserID == userFound.Id && p.txtRef == transRef);

			var intPremium = (int)quoteFound.FinalPremium ;
			var premiumKobo = intPremium * 100;


			var productID = "6205";

			var pay_item_id = "101";

			var payment_redirect_url = ConfigurationManager.AppSettings["return_url"];
			
			var  MAC_KEY = ConfigurationManager.AppSettings["MAC_KEY"];
			// txn_ref + product_id + pay_item_id + amount + site_redirect_url + MacKey
			// var Hash = (transRef + quoteFound.VehicleID.ToString() + motorPaymentFound.ID.ToString() + intPremium.ToString() + payment_redirect_url + MAC_KEY);
			var Hash = (transRef + productID + pay_item_id + premiumKobo.ToString() + payment_redirect_url + MAC_KEY);

			Hash = GenSHA512(Hash);

			var motorResponse = new MotorResponseDTO
			{
			   
				businessClass = "motor",
				customerName = userFound.Surname + " " + userFound.Firstname,
				hash = Hash,
				premium = premiumKobo,
				premiumString = intPremium.ToString(),
				quoteID = quoteFound.ID,
				reference = (motorDetails.reference == null || motorDetails.reference == "") ? "N/A" : motorDetails.reference,
				transactionRefNo = transRef,
				vehicledetail = vehicle.VehicleMake + " " + vehicle.VehicleModel + " " + vehicle.VehicleDate,
				payItemId = pay_item_id
				// payItemId = motorPaymentFound.ID.ToString()

			};

			//send email
			//var description = userType(motorDetails.UserType.ToString()) + " |" +
			//				  motorClass(motorDetails.MotorClass.ToString()) + " |" +
			//				  cardDuration(motorDetails.carduration.ToString());

			//SendEmail(userFound, motorDetails, description, intPremium.ToString("C"));


			return Ok(motorResponse);

		}

		[HttpPost]
		[Route("NSIAMobile/api/paymentresponse")]
		public HttpResponseMessage UpdatePayment([FromBody] ProcessPaymentDTO processPaymentDTO)
		{
			var updatedTime = DateTime.UtcNow;
			//update the paymentdetails table
			var paymentFound = _context.PaymentDetails.FirstOrDefault(p => p.txtRef == processPaymentDTO.txnref);

			//update paymentdetail table
			if (processPaymentDTO.txnref.Trim() == "" || processPaymentDTO.payRef.Trim() == "" || processPaymentDTO.retRef.Trim() == "")
			{
				paymentFound.ResponseCode = "";
				paymentFound.Description = "";
			}
			else
			{
				paymentFound.ResponseCode = "00";
				paymentFound.Description = "Approved Successful";
			}

			paymentFound.txtRef = processPaymentDTO.txnref;
			paymentFound.Payref = processPaymentDTO.payRef;
			paymentFound.RetRef = processPaymentDTO.retRef;
			paymentFound.TransactionDate = updatedTime;

			_context.SaveChanges();

			var redirect_url = ConfigurationManager.AppSettings["responsePage"];
			var response = Request.CreateResponse(HttpStatusCode.Moved);
			response.Headers.Location = new Uri(redirect_url +
												"?txnRef=" + processPaymentDTO.txnref 
												//"&payRef=" + processPaymentDTO.payRef +
												//"&retRef=" + processPaymentDTO.retRef
													);

			return response;
		}

		[HttpPost]
		[Route("NSIAMobile/api/retrievedetail")]
		public IHttpActionResult getPayment([FromBody] ProcessPaymentDTO processPaymentDTO)
		{
			if (!ModelState.IsValid)

				return BadRequest("Invalid payment details");

			//update the paymentdetails table
			var paymentFound = _context.PaymentDetails.FirstOrDefault(p => p.txtRef == processPaymentDTO.txnref);

			if (paymentFound == null) return BadRequest("Invalid payment detail transaction Ref!");

			//return payment details
			var paymentResponseDTO = new PaymentResponseDTO
			{
				responseCode = paymentFound.ResponseCode,
				responsedescription = paymentFound.Description,
				txtref = paymentFound.txtRef,
				payref = paymentFound.Payref,
				retref = paymentFound.RetRef
			};
			
			return Ok(paymentResponseDTO);
		}



		private void SendEmail(AspNetUser subscriber, MotorDetails motorDetails, string assetDescription, string PremiumLess)
		{
			// Applicant applicant = _recruitmentContext.Applicants.Where(a => a.ApplicantId == id).SingleOrDefault();
			
			var receivers = _context.NsiaAdmins.ToList();

			string admins = string.Join(";", receivers);

			string SenderEmail = "nsiaonline@outlook.com";
			string RecieverEmail = admins;
			var password = ConfigurationManager.AppSettings["emailPassword"];
			string morning = "Good Morning";
			string afternoon = "Good Afternoon";
			string evening = "Good Evening";
			string goodday = "Good Day";
			string subject = "Dstv Thanks Transaction for : " + motorClass(motorDetails.MotorClass.ToString());
			var getPresentTime = (DateTime.Now.Hour <= 12) && !(DateTime.Now.Hour > 12)
				? morning : (DateTime.Now.Hour > 12) && (DateTime.Now.Hour <= 16)
				? afternoon : (DateTime.Now.Hour > 16) ? evening : goodday;
			string body1 = @"<head>  
		 < style >
			   thead { background - color:black; color: white; }
			tbody { background - color:white; }

			table, tr, td, th {
				border: 1px solid black;
				margin: 2px;
				font - family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans - serif;
				font - size: 14px;
				text - align: center;
			}
		 </ style >
</ head >

< body >
	< h1 style = 'font-size: 17.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Good day,</ h1 >
	 
		 < p style = 'font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Please find attached details of an
				  < span style = 'font-weight:bold;' > NSIA DSTV THANKS</ span > transctions for:</ p >
				 

					 < p >< span
				 
							 style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Name:</ span >
						 &nbsp;< span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;' >" +
						  subscriber.Firstname + " " + subscriber.Surname +


			@"</ span >< br />
				 

					 < span
				 
							 style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Address:</ span >
						 &nbsp;< span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;' >"
								 + subscriber.Address +
			@"</ span >< br />
					 

						< span
			style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > E - mail:</ span >
			  &nbsp;< span style = 'color:rgb(50, 63, 245);font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;' >
	   
				   < u >" + subscriber.Email +
			@"</ u ></ span >< br />
		  


			  < span style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Phone
			no:</ span > &nbsp;< span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;' >"
			   + subscriber.PhoneNumber + @"</ span ></ p >< br />
   

	   < p style = 'font-size: 16.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Please find the details of the
			   policy below for the further processing.
	   
			   Relevant documents have also benn attached:</ p >
	   

		   < br />
	   
		   < table >
	   
			   < thead >
	   
				   < tr >
	   
					   < th > Insured Name </ th >
	   
					   < th > Policy category </ th >
	   
					   < th > Description of Asset </ th >
	   
					   < th > Reg Number </ th >
	   
					 < th > Chasis Number </ th >
	   
					   < th > Engine Number </ th >
	   
					   < th > Insured Value /< br > Annual emoulment </ th >
	   
					   < th > Premium </ th >
	   
					  
	   
				   </ tr >
	   
			   </ thead >
	   
			   < tbody >
	   
				   < tr >
	   
					   < td > " + subscriber.Firstname + " " + subscriber.Surname + @"</ td >
	   
					   < td > " + motorClass(motorDetails.MotorClass.ToString()) + @" </ td >
	   
					   < td >" + assetDescription + @" </ td >
	   
					   < td >" + motorDetails.RegNo + @" </ td >
	   
					   < td >" + motorDetails.ChasisNo + @" </ td >
	   
					   < td >" + motorDetails.EngineNo + @"</ td >
	   
					   < td >" + motorDetails.purchaseDiscount + @" </ td >
	   
					   < td > " + PremiumLess + @" </ td >
	   
					 
	   
					   </ tr >
	   
			   </ tbody >
	   
		   </ table >
	   
		   < p style = 'font-size: 14px; font-family: Verdana, Geneva, Tahoma, sans-serif;' > Best Regards,< br />< br />
			NSIA DSTVTHANKS Administrator < br />
			NSIA Insurance < br />
		   No 3 Femi Peace Street,< br />
		   Victoria Island,< br />
		   Lagos state.
	   
		   </ p >
	   

	   </ body > ";




			var sender = new MailAddress(SenderEmail);
			var reciever = new MailAddress(RecieverEmail);
			var body = body1;

			var smtp = new SmtpClient
			{
				Host = "smtp-mail.outlook.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(sender.Address, password)
			};


			var message = new MailMessage(sender, reciever)
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			{
				try
				{
				
						smtp.Send(message);
						message.Dispose();

				}
				catch (SmtpFailedRecipientsException ex)
				{
					for (int i = 0; i < ex.InnerExceptions.Length; i++)
					{
						SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
						if (status == SmtpStatusCode.MailboxBusy ||
							status == SmtpStatusCode.MailboxUnavailable)
						{

							System.Threading.Thread.Sleep(5000);

							smtp.Send(message);
							message.Dispose();

						}

					}
				}
			}
		}

		//



		private string vehicleType(string i){
			string valueType;
			switch (i)
			{
				
				case "1":
					valueType = "Saloon";
					break;
				
				case "2":
					valueType = "SUV";
					break;
				case "3":
					valueType = "Bus";
					break;
				case "4":
					valueType = "Truck";
					break;
				default:
					valueType = "Saloon";
					break;
			}

			return valueType;
		}

		private string motorClass(string m)
		{
			string _class;
			switch (m)
			{
			   case "1":
					_class = "Comprehensive";
					break;

				case "2":
					_class = "Third Party";
					break;
				case "3":
					_class = "Super Motor DSTV";
					break;
				case "4":
					_class = "Third Party Fire & Theft DSTV";
					break;

				default:
					_class = "Comprehensive";
					break;
			}

			return _class;
		}

		private string cardDuration(string d)
		{
			string _duration;
			switch (d)
			{
				case "1":
					_duration = "One year";
					break;

				case "2":
					_duration = "Half yearly";
					break;

				default:
					_duration = "One year";
					break;
			}

			return _duration;
		}

		private string userType(string u)
		{
			string _type;
			switch (u)
			{
				case "1":
					_type = "Individual";
					break;

				case "2":
					_type = "Corporate";
					break;

				default:
					_type = "Individual";
					break;
			}

			return _type;
		}

		private string usageType(string u)
		{
			string _type;
			switch (u)
			{
				case "1":
					_type = "Private";
					break;

				case "2":
					_type = "Commercial";
					break;

				default:
					_type = "Private";
					break;
			}

			return _type;
		}

		private string RandomID()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(RandomString(5, true));
			builder.Append(RandomNumber(100000000, 999999999));
			builder.Append(RandomString(2, false));
			return builder.ToString();
		}

		private int RandomNumber(int min, int max)
		{
			Random random = new Random();
			return random.Next(min, max);
		}

		private string RandomString(int size, bool lowerCase)
		{
			StringBuilder builder = new StringBuilder();
			Random random = new Random();
			char ch;
			for (int i = 0; i < size; i++)
			{
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
				builder.Append(ch);
			}
			if (lowerCase)
				return builder.ToString().ToLower();
			return builder.ToString();
		}

		private string generateID()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static string GenSHA512(string s, bool l = false)
		{
			string r = "";
			try
			{
				byte[] d = Encoding.UTF8.GetBytes(s);
				using (SHA512 a = new SHA512Managed())
				{
					byte[] h = a.ComputeHash(d);
					r = BitConverter.ToString(h).Replace("-", "");
				}
				if (l)
					r = r.ToLower();
			}
			catch
			{

			}
			return r;
		}
	}
}
