using AutoMapper;
using Excel;
using Newtonsoft.Json;
using NSIA.DTO;
using NSIA.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace NSIA.Controllers.Api
{
	public class DstvThanksController : ApiController
	{
		public readonly NSIAMobileEntities _context;

		public DstvThanksController()
		{
			_context = new NSIAMobileEntities();
		}

		[HttpPost]
		[Route("NSIAMobile/api/confirmdstvcode")]
		public IHttpActionResult ConfirmDstvCode([FromBody] DstvCodeInputDTO dstvDto)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid code");

			// remove outdated files
			var month = DateTime.Now.Month.ToString();

			var dtsvpromo = _context.Promos.FirstOrDefault(p =>
				p.PromoName == month && p.Code == dstvDto.code && p.deletedAt == null);


			if (dtsvpromo == null) return NotFound();

			//update time on promo table
			dtsvpromo.updatedAt = DateTime.UtcNow;
			//dtsvpromo.deletedAt = DateTime.UtcNow;
			_context.SaveChanges();

			var dstvResponseDTO = new DstvResponseDTO
			{
				exists = true
			};

			return Ok(dstvResponseDTO);
		}

		[HttpPost]
		[Route("NSIAMobile/api/checkDstvSubscriberExists")]
		public IHttpActionResult checkDstvSubscriberExists([FromBody] DstvSubscriberEmailInputDTO dstvEmailDto)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid code");

			var dtsvSubscriber = _context.DstvSubscribers.FirstOrDefault(s=>s.EmailAddress == dstvEmailDto.Email && s.deleted_at == null);

			var dstvResponseDTO = new DstvResponseDTO();

			if (dtsvSubscriber == null)
			{
				dstvResponseDTO = new DstvResponseDTO
				{
					exists = false
				};

				return Ok(dstvResponseDTO);
			}

			dtsvSubscriber.updated_at= DateTime.UtcNow;
			dtsvSubscriber.DstvCode = dstvEmailDto.Code;
			_context.SaveChanges();

			dstvResponseDTO = new DstvResponseDTO
			{
				exists = true,
				SubscriberId = dtsvSubscriber.Id
			};

			return Ok(dstvResponseDTO);
		}

		[HttpPost]

		[Route("NSIAMobile/dstvcodeupload")]
		public IHttpActionResult DstvCodeUpload()
		{
			var uploadfile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
			var date_time = DateTime.UtcNow;

			if (uploadfile == null || uploadfile.ContentLength < 0) return BadRequest("Please upload your file");

			//ExcelDataReader works on binary excel file
			Stream stream = uploadfile.InputStream;
			//We need to written the Interface.
			IExcelDataReader reader = null;
			if (uploadfile.FileName.EndsWith(".xls"))
			{
				//reads the excel file with .xls extension
				reader = ExcelReaderFactory.CreateBinaryReader(stream);
			}
			else if (uploadfile.FileName.EndsWith(".xlsx"))
			{
				//reads excel file with .xlsx extension
				reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
			}
			else
			{
				//Shows error if uploaded file is not Excel file
				return BadRequest("This file format is not supported");
			}

			//treats the first row of excel file as Column Names
			reader.IsFirstRowAsColumnNames = true;
			//Adding reader data to DataSet()
			DataSet result = reader.AsDataSet();
			reader.Close();

			//Sending result data to View
			var response = result.Tables[0];
			var latestupload = new List<Promo>();
			var rowCount = response.Rows.Count;
			for (int i = 0; i < rowCount; i++)
			{
				var rowFirstColumn = response.Rows[i][0];
				var rowSecondColumn = response.Rows[i][1];
				//var rowThirdColumn = response.Rows[i][1];

				var promo = new Promo
				{
					PromoName = rowFirstColumn.ToString(), //month
					Code = rowSecondColumn.ToString(),
					createdAt = date_time,
					updatedAt = date_time
				};


				_context.Promos.Add(promo);
				_context.SaveChanges();

				latestupload.Add(promo);
			}

			return Ok(latestupload);
		}

		[HttpPost]
		[Route("NSIAMobile/api/dstvuserdetails")]
		public IHttpActionResult DstvUserDetails([FromBody] DstvCustomerDetailsInputDTO dstvInputDto)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid customer details");

			//check if user already exists
			var dstUser = _context.DstvSubscribers.FirstOrDefault(u => u.EmailAddress == dstvInputDto.EmailAddress && u.deleted_at == null);

			if (dstUser == null)
			{
				dstvInputDto.created_at = DateTime.UtcNow;
				dstvInputDto.updated_at = DateTime.UtcNow;

				var dstvSubscriber = Mapper.Map<DstvCustomerDetailsInputDTO, DstvSubscriber>(dstvInputDto);

				_context.DstvSubscribers.Add(dstvSubscriber);
				_context.SaveChanges();

			}

			var dstvCustomerOutput =
				_context.DstvSubscribers.FirstOrDefault(u => u.EmailAddress == dstvInputDto.EmailAddress && u.deleted_at == null);

			return Ok(dstvCustomerOutput);

		}

		[HttpGet]
		[Route("NSIAMobile/api/GetDstvSubscriberTitles")]
		public IHttpActionResult GetDstvTitles()
		{
			var dstvTitles = _context.DstvSubscriberTitles.ToList();
			var dstvtitlesdto = Mapper.Map<List<DstvSubscriberTitle>, List<DstvSubscriberTitleDTO>>(dstvTitles);

			return Ok(dstvtitlesdto);
		}

		[HttpGet]
		[Route("NSIAMobile/api/GetDstvSubscriberIdentity")]
		public IHttpActionResult GetDstvSubscriberIdentityMeans()
		{
			var dstvIdMeans = _context.DstvSubscriberIdentificationTypes.ToList();
			var dstvIdMeansdto = Mapper.Map<List<DstvSubscriberIdentificationType>, List<DstvSubscriberIdMeansDTO>>(dstvIdMeans);

			return Ok(dstvIdMeansdto);
		}

		[HttpGet]
		[Route("NSIAMobile/api/GetDstvPolicies")]
		public IHttpActionResult GetDstvPolicies()
		{
			var dstvPolicies = _context.DstvPolicies.ToList();
			var dstvdto = Mapper.Map<List<DstvPolicy>, List<DstvPoliciesDTO>>(dstvPolicies);

			return Ok(dstvdto);
		}

		//getHouseHoldOptions
		[HttpGet]
		[Route("NSIAMobile/api/GetDstvetHouseHoldOptions")]
		public IHttpActionResult GetDstvetHouseHoldOptions()
		{
			var dstvHouseHoldOptions = _context.DstvHomeTypes.ToList();
			var dstvdto = Mapper.Map<List<DstvHomeType>, List<DstvHomeTypesDTO>>(dstvHouseHoldOptions);

			return Ok(dstvdto);
		}
		//getPersonalAccidentTiers
		[HttpGet]
		[Route("NSIAMobile/api/GetPersonalAccidentTiers")]
		public IHttpActionResult GetPersonalAccidentTiers()
		{
			var dstvPersonalAccidentOptions = _context.DstvPersonalOptions.ToList();
			var dstvdto = Mapper.Map<List<DstvPersonalOption>, List<DstvPersonalOptionsDTO>>(dstvPersonalAccidentOptions);

			return Ok(dstvdto);
		}

		[HttpPost]
		[Route("NSIAMobile/api/CalculateDstvPremium")]
		public IHttpActionResult CalculatePremium()
		{
			var httpContext = HttpContext.Current;
			DstvPremiumDetailsInput premiumDetails = JsonConvert.DeserializeObject<DstvPremiumDetailsInput>(httpContext.Request.Form["serviceBody"]);
			//        if (!ModelState.IsValid)
			//return BadRequest("Invalid premium details");
			  
			var premiumDetailsDstvSubscriberId = Convert.ToInt32(premiumDetails.DstvSubscriberId);

			var user = _context.DstvSubscribers.FirstOrDefault(u => u.Id == premiumDetailsDstvSubscriberId);

			if (user == null)
				return BadRequest("User details does not exist");

			//check the images

			// Check for any uploaded file  
			decimal premiumLess = 0.0M;
			decimal premium = 0.0M;

			string fileSavePathName;

			List<string> filePathName = new List<string>();
			//filePathName = null;

			var premiumDetailsDstvPolicy = Convert.ToInt32(premiumDetails.DstvPolicy);

			// var premiumDetailsSumInsured = Convert.ToDecimal(premiumDetails.SumInsured);
			var premiumDetailsSumInsured = Convert.ToInt64(Math.Round(Convert.ToDouble(premiumDetails.SumInsured)));

			if (premiumDetailsDstvPolicy == 1 || premiumDetailsDstvPolicy == 2)
			{
				if (httpContext.Request.Files.Count == 7)
				{
					//Loop through uploaded files  
					for (int i = 0; i < httpContext.Request.Files.Count; i++)
					{
						HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
						if (httpPostedFile != null)
						{
							var userFolder = user.FirstName + user.MiddleName + user.Surname + user.Id;

							Directory.CreateDirectory(ConfigurationManager.AppSettings["fileUploadFolder"] +
													   "/Dstv/" + userFolder + "/Vehicle"
													  );
							// Construct file save path  
							fileSavePathName =
							   Path.Combine(
								   (ConfigurationManager.AppSettings["fileUploadFolder"] +
									"/Dstv/" + userFolder + "/Vehicle"
								   ), httpPostedFile.FileName);

							// Save the uploaded file  
							httpPostedFile.SaveAs(fileSavePathName);
							//save file string to db
							filePathName.Add(fileSavePathName);



						}
					}


					// Do the calculations
					//For A.NSIA Comprehensive Motor Insurance Product

					if (premiumDetailsDstvPolicy == 1)
					{
						decimal premiumRate = 0.0M;
						 premium = 0.0M;
						//  var premiumDetailsSumInsured = Convert.ToInt32(premiumDetails.SumInsured);

						if (premiumDetailsSumInsured >= 1000000.0M && premiumDetailsSumInsured <= 4999999.0M)
						{
							premiumRate = 3.0M / 100.0M;
						}

						if (premiumDetailsSumInsured >= 5000000.0M && premiumDetailsSumInsured <= 9999999.0M)
						{
							premiumRate = 2.75M / 100.0M;
						}

						if (premiumDetailsSumInsured >= 10000000.0M && premiumDetailsSumInsured <= 14999999.0M)
						{
							premiumRate = 2.50M / 100.0M;
						}

						if (premiumDetailsSumInsured >= 15000000.0M && premiumDetailsSumInsured <= 19999999.0M)
						{
							premiumRate = 2.25M / 100.0M;
						}

						if (premiumDetailsSumInsured >= 20000000.0M)
						{
							premiumRate = 2.0M / 100.0M;
						}

						premium = premiumRate * premiumDetailsSumInsured;
						premiumLess = premium - (premium * (10.0M / 100.0M));
					}

					//B.NSIA  Motor Insurance- Third Party Fire & Theft 
					if (premiumDetailsDstvPolicy == 2)
					{
						decimal premiumRate = 0.0M;
						premium = 0.0M;
						//var premiumDetailsSumInsured = Convert.ToInt32(premiumDetails.SumInsured);

						if (premiumDetailsSumInsured >= 1000000.0M && premiumDetailsSumInsured <= 19999999.0M)
						{
							premiumRate = 1.65M / 100.0M;
						}

						if (premiumDetailsSumInsured >= 20000000.0M)
						{
							premiumRate = 1.40M / 100.0M;
						}

						premium = premiumRate * premiumDetailsSumInsured;

						premiumLess = premium - (premium * 10.0M / 100.0M);
					}
				}
				else { return BadRequest("File amount not complete"); }
			}

			//C.NSIA Householder Insurance

			if (premiumDetailsDstvPolicy == 3)
			{
				decimal premiumRate = 0.0M;
				premium = 0.0M;

				var premiumDetailsHomeType = Convert.ToInt32(premiumDetails.HomeType);
				if (premiumDetailsHomeType == 1)
				{
					if (httpContext.Request.Files.Count == 3)
					{
						//Loop through uploaded files  
						for (int i = 0; i < httpContext.Request.Files.Count; i++)
						{
							HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
							if (httpPostedFile != null)
							{
								var userFolder = user.FirstName + user.MiddleName + user.Surname + user.Id;

								Directory.CreateDirectory(ConfigurationManager.AppSettings["fileUploadFolder"] +
														  "/Dstv/" + userFolder + "/Building"
								);
								// Construct file save path  
								fileSavePathName =
									Path.Combine(
										(ConfigurationManager.AppSettings["fileUploadFolder"] +
										 "/Dstv/" + userFolder + "/Building"
										), httpPostedFile.FileName);

								// Save the uploaded file  
								httpPostedFile.SaveAs(fileSavePathName);
								//save file string to db
								filePathName.Add(fileSavePathName);



							}
						}
						premiumRate = 0.15M / 100.0M;

					}
					else { return BadRequest("File amount not complete"); }
				}
				//var premiumDetailsHomeType = Convert.ToInt32(premiumDetails.HomeType);
				// var premiumDetailsSumInsured = Convert.ToInt32(premiumDetails.SumInsured);

				//if (premiumDetailsHomeType == 1)
				//{
				//	premiumRate = 0.15 / 100.00;
				//}

				if (premiumDetailsHomeType == 2)
					{
						premiumRate = 0.275M / 100.0M;
					}

					premium = premiumRate * premiumDetailsSumInsured;
					premiumLess = premium - (premium * 10.0M / 100.0M);			   
				
			}

			//D.NSIA Personal Accident Cover
			if (premiumDetailsDstvPolicy == 4)
			{
				decimal premiumRate = 0.0M;
				premium = 0.0M;
				var premiumDetailsPersonalAccident = Convert.ToInt32(premiumDetails.PersonalAccident);

				if (premiumDetailsPersonalAccident == 1)
				{
					premium = 1000.0M;
				}
				if (premiumDetailsPersonalAccident == 2)
				{
					premium = 2000.0M;
				}
				if (premiumDetailsPersonalAccident == 3)
				{
					premium = 4000.0M;
				}

				premiumLess = premium - (premium * (10.0M / 100.0M));
			}

			premiumDetails.creeated_at = DateTime.UtcNow;
			premiumDetails.updated_at = DateTime.UtcNow;
			//save to db
			var dstvSubscriberPremium = Mapper.Map<DstvPremiumDetailsInput, DstvPremiumDetail>(premiumDetails);


			_context.DstvPremiumDetails.Add(dstvSubscriberPremium);
			_context.SaveChanges();

			//Add to Dstvtransaction table
			var date_time = DateTime.UtcNow;
			// var transRef = RandomID() + "-" + generateID();
			var transRef = "NSI" + generateID();
			transRef = transRef.Substring(0, 11);
			var productID = "6205";
			var pay_item_id = "101";
			var premiumKobo = ((int)premiumLess) * 100;

			var payment_redirect_url = ConfigurationManager.AppSettings["dstv_return_url"];
			var MAC_KEY = ConfigurationManager.AppSettings["MAC_KEY"];
			var hash = (transRef + productID + pay_item_id + premiumKobo.ToString() + payment_redirect_url + MAC_KEY);
			hash = GenSHA512(hash);
			//var dstvTransaction = new DstvTransactionDTO
			//{
			//	Subscriber_id = premiumDetails.DstvSubscriberId,
			//	TransactionRef = transRef,
			//	Amount = (double)premiumDetails.SumInsured,
			//	Hash = hash,
			//	PaymentStatus = "",
			//	created_at = date_time,
			//	updated_at = date_time,
			//	deleted_at = null
			//};

			//var dstvTrans = Mapper.Map<DstvTransactionDTO, DstvTransaction>(dstvTransaction);

			//_context.DstvTransactions.Add(dstvTrans);
			// Add to payment table
			try
			{
				var dstvPayment = new DstvTransactionPaymentsDetail()
				{
					Subscriber_id = premiumDetails.DstvSubscriberId,
					txtRef = transRef,
					Hash = hash,
					DstvPremiumDetailsId = premiumDetails.DstvPolicy,
					Amount = Convert.ToDecimal(premiumDetailsSumInsured),
					AmountString = premiumDetails.SumInsured,
					created_at = date_time,
					updated_at = date_time
				};

				_context.DstvTransactionPaymentsDetails.Add(dstvPayment);
				_context.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				var errorMessages = ex.EntityValidationErrors
					.SelectMany(x => x.ValidationErrors)
					.Select(x => x.ErrorMessage);
				var fullErrorMessage = string.Join(",", errorMessages);
				var exceptionMessage = string.Concat(ex.Message, fullErrorMessage);
				throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
				//ViewBag.RegisterError = "An Error Occurred :" + ex.Message + ex.TargetSite + ex.HResult;
			}


			//get user details
			// var userFound = _context.DstvSubscribers.FirstOrDefault(s=>s.Id == premiumDetails.DstvSubscriberId);
			var policy = _context.DstvPolicies.FirstOrDefault(p => p.Id == premiumDetailsDstvPolicy);

			var dstvPayResponse = new dstvPaymentResponseDTO
			{
				product_id = productID,
				cust_id = premiumDetails.DstvSubscriberId,
				currency = "566",
				businessClass = policy.Name,
				customerName = user.Surname + " " + user.FirstName,
				hash = hash,
				premium = (int)premiumLess,
				originalPremium = (int)premium,
				premiumString = premiumKobo.ToString(),
				transactionRefNo = transRef,
				payItemId = pay_item_id,
				redirect_url = ConfigurationManager.AppSettings["dstv_return_url"]


			};

			// Send email
			var subscriber = _context.DstvSubscribers.Where(u => u.Id == premiumDetailsDstvSubscriberId).OrderByDescending(u => u.Id).ToList();

			SendEmail(subscriber[0], premiumDetails, premiumLess.ToString(), filePathName);



			return Ok(dstvPayResponse);

		}

		[HttpPost]
		[Route("NSIAMobile/api/dstvpaymentresponse")]
		public HttpResponseMessage UpdatePayment([FromBody] ProcessPaymentDTO processPaymentDTO)
		{
			var updatedTime = DateTime.UtcNow;
			//update the paymentdetails table
			var paymentFound =
				_context.DstvTransactionPaymentsDetails.FirstOrDefault(p => p.txtRef == processPaymentDTO.txnref);

			//update paymentdetail table
			try
			{

				if (String.IsNullOrEmpty(processPaymentDTO.txnref.Trim()) ||
					String.IsNullOrEmpty(processPaymentDTO.payRef.Trim()) ||
					String.IsNullOrEmpty(processPaymentDTO.retRef.Trim()))
				{
					paymentFound.ResponseCode = "";
					paymentFound.Description = "";
				}
				else
				{
					// insert in dstvTransactionCode table

					paymentFound.ResponseCode = "00";
					paymentFound.Description = "Approved Successful";
				}

				paymentFound.txtRef = processPaymentDTO.txnref;
				paymentFound.Payref = processPaymentDTO.payRef;
				paymentFound.RetRef = processPaymentDTO.retRef;
				paymentFound.updated_at = updatedTime;

				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				paymentFound.ResponseCode = "";
				paymentFound.Description = "";
				paymentFound.txtRef = processPaymentDTO.txnref;
				paymentFound.Payref = processPaymentDTO.payRef;
				paymentFound.RetRef = processPaymentDTO.retRef;
				paymentFound.updated_at = updatedTime;

				_context.SaveChanges();
			}

		var redirect_url = ConfigurationManager.AppSettings["dstvResponsePage"];
			var response = Request.CreateResponse(HttpStatusCode.Moved);
			response.Headers.Location = new Uri(redirect_url +
												"?txnRef=" + processPaymentDTO.txnref
			//"&payRef=" + processPaymentDTO.payRef +
			//"&retRef=" + processPaymentDTO.retRef
			);

			return response;
		}

		[HttpPost]
		[Route("NSIAMobile/api/dstvretrievedetail")]
		public IHttpActionResult getPayment([FromBody] ProcessPaymentDTO processPaymentDTO)
		{
			if (!ModelState.IsValid)

				return BadRequest("Invalid payment details");
			
			var paymentFound = _context.DstvTransactionPaymentsDetails.FirstOrDefault(p => p.txtRef == processPaymentDTO.txnref);

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




		private void SendEmail(DstvSubscriber subscriber, DstvPremiumDetailsInput dstvPremiumDetails, string PremiumLess, List<string> fileSavePath)
		{
			// Applicant applicant = _recruitmentContext.Applicants.Where(a => a.ApplicantId == id).SingleOrDefault();
			var dstvPremiumDetailsDstvPolicy = Convert.ToInt32(dstvPremiumDetails.DstvPolicy);

			var policy = _context.DstvPolicies.FirstOrDefault(p => p.Id == dstvPremiumDetailsDstvPolicy);
			var houseHoldTypeName = "";
			var accidentTierName = "";
			var houseDescription = "";



			if (!String.IsNullOrEmpty(dstvPremiumDetails.HomeType))
			{
				var dstvPremiumDetailsHomeType = Convert.ToInt32(dstvPremiumDetails.HomeType);


				var houseHoldType = _context.DstvHomeTypes.FirstOrDefault(h => h.Id == dstvPremiumDetailsHomeType);

				houseHoldTypeName = houseHoldType.Name;

				houseDescription = String.IsNullOrEmpty(dstvPremiumDetails.AssetDescription) ? "Building" : dstvPremiumDetails.AssetDescription;


			}

			if (!String.IsNullOrEmpty(dstvPremiumDetails.PersonalAccident))
			{
				var dstvPremiumDetailsPersonalAccident = Convert.ToInt32(dstvPremiumDetails.PersonalAccident);
				var accidentTier = _context.DstvPersonalOptions.FirstOrDefault(h => h.Id == dstvPremiumDetailsPersonalAccident);
				accidentTierName = accidentTier.Name;

			}

			//var receivers = _context.NsiaDstvAdmins.ToList();

			//string admins = string.Join(";", receivers);

			string SenderEmail = ConfigurationManager.AppSettings["emailSender"];

			string RecieverEmail = ConfigurationManager.AppSettings["emailReceiver"];

			var password = ConfigurationManager.AppSettings["emailPassword"];
			string morning = "Good Morning";
			string afternoon = "Good Afternoon";
			string evening = "Good Evening";
			string goodday = "Good Day";
			string subject = "Dstv Thanks Transaction for : " + policy.Name;
			var getPresentTime = (DateTime.Now.Hour <= 12) && !(DateTime.Now.Hour > 12)
				? morning : (DateTime.Now.Hour > 12) && (DateTime.Now.Hour <= 16)
				? afternoon : (DateTime.Now.Hour > 16) ? evening : goodday;
			string body1 = @"<head><style>
			   thead { background-color:black; color: white; }
				tbody { background-color:white; }
				table, tr, td, th {
				border: 1px solid black;
				margin: 2px;
				font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans - serif;
				font-size: 14px;
				text-align: center;
				}</style></head><body><h1 style = 'font-size: 17.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Good day,</h1>
	 
		 <p style = 'font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Please find attached details of an
				  <span style = 'font-weight:bold;'> NSIA DSTV THANKS</span> transactions for:</p>
				 

					 <p><span style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Name:</span>
						 &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>" +
						  subscriber.FirstName + " " + subscriber.MiddleName + " " + subscriber.Surname +


			@"</span><br />
				 

					 <span style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Address:</span>
						 &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>"
								 + subscriber.SubscriberAddress +
			@"</span><br />
					 

						<span
			style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> E - mail:</span>
			  &nbsp;<span style = 'color:rgb(50, 63, 245);font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>
	   
				   <u>" + subscriber.EmailAddress +
			@"</u></span><br />
		  


			  <span style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Phone
			no:</span> &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>"
			   + subscriber.PhoneNumber + @"</span></p><br />
   

	   <p style = 'font-size: 16.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Please find the details of the
			   policy below for the further processing.
	   
			   Relevant documents have also been attached:</p>
	   

		   <br />
	   
		   <table>
	   
			   <thead>
	   
				   <tr>
	   
					   <th> Insured Name </th>
	   
					   <th> Policy category </th>
	   
					   <th> Description of Asset </th>

					   <th> Asset Location </th>
	   
					   <th> Reg Number </th>
	   
					   <th> Chasis Number </th>
	   
					   <th> Engine Number </th>
	   
					   <th> Insured Value </th>
					
	   
					   <th> Premium </th>
	   
					   <th> Date of Birth </th>
	   
					   <th> Identification Number </th>
	   
					   <th> profession </th>
	   
				   </tr>
	   
			   </thead>
	   
			   <tbody>
	   
				   <tr>
	   
					   <td> " + subscriber.FirstName + " " + subscriber.MiddleName + " " + subscriber.Surname + @"</td>
	   
					   <td> " + policy.Name + @" </td>
	   
					   <td>" + dstvPremiumDetails.AssetDescription + @" </td>

						<td>" + dstvPremiumDetails.Location + @" </td>
	   
					   <td>" + dstvPremiumDetails.VehicleReg + @" </td>
	   
					   <td>" + dstvPremiumDetails.Chasis + @"</td>
	   
					   <td>" + dstvPremiumDetails.EngineNumbers + @"</td>
	   
					   <td>" + dstvPremiumDetails.SumInsured + @" </td>
	   
					   <td> " + PremiumLess + @" </td>
	   
					   <td>" + subscriber.DateOfBirth + @" </td>
	   
					   <td> " + subscriber.IdentificationNumber + @"</td>
	   
					   <td>  " + subscriber.Profession + @" </td>
	   
					   </tr>
	   
			   </tbody>
	   
		   </table>
	   
		   <p style = 'font-size: 14px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Best Regards,<br /><br />
			NSIA DSTVTHANKS Administrator <br />
			NSIA Insurance <br />
		   No 3 Femi Peace Street,<br />
		   Victoria Island,<br />
		   Lagos state.
	   
		   </p>
	   

	   </body> ";




			var body2 = @"<head>
	
   
	   <style>
			   thead { background-color:black; color: white; }
			tbody { background-color:white; }

			table, tr, td, th {
				border: 1px solid black;
				margin: 2px;
				font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans - serif;
				font-size: 14px;
				text-align: center;
			}
			</style>
</head>

<body>
	<h1 style = 'font-size: 17.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Good day,</h1>
	 
		 <p style = 'font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Please find attached details of an
				  <span style = 'font-weight:bold;'> NSIA DSTV THANKS</span> transactions for:</p>
				 

					 <p><span
				 
							 style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Name:</span>
						 &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>" +
							  subscriber.FirstName + " " + subscriber.MiddleName + " " + subscriber.Surname +


				@"</span><br />
				 

					 <span
				 
							 style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Address:</span>
						 &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>"
									 + subscriber.SubscriberAddress +
				@"</span><br />
					 

						<span
			style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> E - mail:</span>
			  &nbsp;<span style = 'color:rgb(50, 63, 245);font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>
	   
				   <u>" + subscriber.EmailAddress +
				@"</u></span><br />
		  


			  <span style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Phone
			no:</span> &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>"
				   + subscriber.PhoneNumber + @"</span></p><br />
   

	   <p style = 'font-size: 16.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Please find the details of the
			   policy below for the further processing.
	   
			   Relevant documents have also been attached:</p>
	   

		   <br />
	   
		   <table>
	   
			   <thead>
	   
				   <tr>
	   
					   <th> Insured Name </th>
	   
					   <th> Policy category </th>
	   
					   <th> Description of Asset </th>

						<th> Location </th>
						
					   <th> Category </th>
					   
					   <th> Insured Value </th>
	   
					   <th> Premium </th>
	   
					   <th> Date of Birth </th>
	   
					   <th> Identification Number </th>
	   
					   <th> Profession </th>
	   
				   </tr>
	   
			   </thead>
	   
			   <tbody>
	   
				   <tr>
	   
					   <td> " + subscriber.FirstName + " " + subscriber.MiddleName + " " + subscriber.Surname + @"</td>
	   
					   <td> " + policy.Name + @" </td>
	   
					   <td>" + houseDescription + @" </td>

						<td>" + dstvPremiumDetails.Location + @" </td>
		
						<td>" + houseHoldTypeName + @"</td>
	   
					   <td>" + dstvPremiumDetails.SumInsured + @" </td>
	   
					   <td> " + PremiumLess + @" </td>
	   
					   <td>" + subscriber.DateOfBirth + @" </td>
	   
					   <td> " + subscriber.IdentificationNumber + @"</td>
	   
					   <td>  " + subscriber.Profession + @" </td>
	   
					   </tr>
	   
			   </tbody>
	   
		   </table>
	   
		   <p style = 'font-size: 14px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Best Regards,<br /><br />
			NSIA DSTVTHANKS Administrator <br />
			NSIA Insurance <br />
		   No 3 Femi Peace Street,<br />
		   Victoria Island,<br />
		   Lagos state.
	   
		   </p>
	   

	   </body> ";

			var body3 = @"<head>
	
   
	   <style>
			   thead { background-color:black; color: white; }
			tbody { background-color:white; }

			table, tr, td, th {
				border: 1px solid black;
				margin: 2px;
				font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans - serif;
				font-size: 14px;
				text-align: center;
			}
			</style>
</head>

<body>
	<h1 style = 'font-size: 17.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Good day,</h1>
	 
		 <p style = 'font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Please find attached details of an
				  <span style = 'font-weight:bold;'> NSIA DSTV THANKS</span> transactions for:</p>
				 

					 <p><span
				 
							 style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Name:</span>
						 &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>" +
						  subscriber.FirstName + " " + subscriber.MiddleName + " " + subscriber.Surname +


			@"</span><br />
				 

					 <span
				 
							 style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Address:</span>
						 &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>"
								 + subscriber.SubscriberAddress +
			@"</span><br />
					 

						<span
			style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> E - mail:</span>
			  &nbsp;<span style = 'color:rgb(50, 63, 245);font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>
	   
				   <u>" + subscriber.EmailAddress +
			@"</u></span><br />
		  


			  <span style = 'color:black;font-weight:bold;font-size: 15px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Phone
			no:</span> &nbsp;<span style = 'font-family: Verdana, Geneva, Tahoma, sans-serif; font-size:16px;'>"
			   + subscriber.PhoneNumber + @"</span></p><br />
   

	   <p style = 'font-size: 16.5px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Please find the details of the
			   policy below for the further processing.
	   
			   Relevant documents have also been attached:</p>
	   

		   <br />
	   
		   <table>
	   
			   <thead>
	   
				   <tr>
	   
					   <th> Insured Name </th>
	   
					   <th> Policy category </th>

						<th> Location </th>
						
					   <th> Category </th>
					   
						<th> Annual emoulment </th>
	   
					   <th> Premium </th>
	   
					   <th> Date of Birth </th>
	   
					   <th> Identification Number </th>
	   
					   <th> Type of Profession </th>
	   
				   </tr>
	   
			   </thead>
	   
			   <tbody>
	   
				   <tr>
	   
					   <td> " + subscriber.FirstName + " " + subscriber.MiddleName + " " + subscriber.Surname + @"</td>
	   
					   <td> " + policy.Name + @" </td>

						<td> " + dstvPremiumDetails.Location + @" </td>
						  
						<td>" + accidentTierName + @"</td>
	   
					   <td>" + dstvPremiumDetails.SumInsured + @" </td>
	   
					   <td> " + PremiumLess + @" </td>
	   
					   <td>" + subscriber.DateOfBirth + @" </td>
	   
					   <td> " + subscriber.IdentificationNumber + @"</td>
	   
					   <td>  " + subscriber.Profession + @" </td>
	   
					   </tr>
	   
			   </tbody>
	   
		   </table>
	   
		   <p style = 'font-size: 14px; font-family: Verdana, Geneva, Tahoma, sans-serif;'> Best Regards,<br /><br />
			NSIA DSTVTHANKS Administrator <br />
			NSIA Insurance <br />
		   No 3 Femi Peace Street,<br />
		   Victoria Island,<br />
		   Lagos state.
	   
		   </p>
	   

	   </body> ";


			var sender = new MailAddress(SenderEmail);
			var reciever = new MailAddress(RecieverEmail);
			var body = body1;
			if (!String.IsNullOrEmpty(dstvPremiumDetails.HomeType))
			{
				body = body2;
			}

			if (!String.IsNullOrEmpty(dstvPremiumDetails.PersonalAccident))
			{
				body = body3;
			}

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
					//attachment exists
					if (fileSavePath.Count() > 0)
					{
						// string fileAttachPath = Server.MapPath(attachment);

						Attachment attach = null;

						foreach (string attachment in fileSavePath)
						{
							attach = new Attachment(attachment);
							message.Attachments.Add(attach);
						}

						smtp.Send(message);
						attach.Dispose();
						message.Dispose();
					}
					else
					{
						smtp.Send(message);
						message.Dispose();
					}




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

							if (fileSavePath.Count() > 0)
							{
								// string fileAttachPath = Server.MapPath(attachment);

								Attachment attach = null;

								foreach (string attachment in fileSavePath)
								{
									attach = new Attachment(attachment);
									message.Attachments.Add(attach);
								}

								smtp.Send(message);
								attach.Dispose();
								message.Dispose();
							}
							else
							{
								smtp.Send(message);
								message.Dispose();
							}

						}

					}
				}
			}
		}

		//

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

		private static string GenSHA512(string s, bool l = false)
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
