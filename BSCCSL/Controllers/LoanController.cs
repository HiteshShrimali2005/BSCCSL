using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BSCCSL.Models;
using BSCCSL.Services;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using BSCCSL.Extension;

namespace BSCCSL.Controllers
{
    [Authorize]
    public class LoanController : ApiController
    {
        LoanService loanservice;

        public LoanController()
        {
            loanservice = new LoanService();
        }

        [HttpGet]
        public HttpResponseMessage GetAllLookup()
        {
            try
            {
                var result = loanservice.GetAllLookup();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveLoan(Loan loan)
        {
            try
            {
                var result = loanservice.SaveLoan(loan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveLoanFromGroupLoan(LoanStatusData loan)
        {
            try
            {
                var result = loanservice.SaveLoan(loan.Loan, Request.GetCurrentUser());
                if (loan.LoanCharges.Count != 0)
                {
                    foreach (LoanCharges lc in loan.LoanCharges)
                    {
                        lc.LoanId = loan.Loan.LoanId;
                    }
                    var result2 = loanservice.SaveLoanCharges(loan.LoanCharges);
                }
                if (loan.LoanStatus != null)
                {
                    var result1 = loanservice.SaveUpdatedLoanStatus(loan.LoanStatus);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveBorrower(List<Borrower> borrowers)
        {
            try
            {
                var result = loanservice.SaveBorrower(borrowers, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveReferencer(Reference reference)
        {
            try
            {
                var result = loanservice.SaveReferencer(reference, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage LoanList(DataTableSearch search)
        {
            try
            {
                var result = loanservice.LoanList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GroupLoanList(DataTableSearch search)
        {
            try
            {
                var result = loanservice.GroupLoanList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage LoanApprovalList(DataTableSearch search)
        {
            try
            {
                var result = loanservice.LoanApprovalList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetReferencerDetailById(Guid id)
        {
            try
            {
                var result = loanservice.GetReferencerDetailById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerPersonalDetailById(Guid id1, Guid? id2)
        {
            try
            {
                var result = loanservice.GetCustomerPersonalDetailById(id1, id2);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetGroupLoandetail(Guid Id)
        {
            try
            {
                var result = loanservice.GetGroupLoandetail(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerDetailByPersonalId(Guid id1, Guid? id2)
        {
            try
            {
                var result = loanservice.GetCustomerDetailByPersonalId(id1, id2);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerPersonalDetailByAccountId(string id)
        {
            try
            {
                var result = loanservice.GetCustomerPersonalDetailByAccountId(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerPersonalDetailByAccountIdForGroupLoan(string id)
        {
            try
            {
                var result = loanservice.GetCustomerPersonalDetailByAccountIdForGroupLoan(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage CustomerProductId(Guid id)
        {
            try
            {
                var result = loanservice.CustomerProductId(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveBusinessLoanData(BusinessLoan Businessloan)
        {
            try
            {
                var result = loanservice.SaveBusinessLoanData(Businessloan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveEconomicDetails(List<BusinessEconomicDetails> businessEconomicDetails)
        {
            try
            {
                var result = loanservice.SaveEconomicDetails(businessEconomicDetails, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetBusinessLoanById(Guid Id)
        {
            try
            {
                var result = loanservice.GetBusinessLoanById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveVehicleLoanData(VehicleLoan VehicleLoanData)
        {
            try
            {
                var result = loanservice.SaveVehicleLoanData(VehicleLoanData, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetVehicleLoanById(Guid Id)
        {
            try
            {
                var result = loanservice.GetVehicleLoanById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteLoan(Guid Id)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteLoan(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteMorgageItem(Guid Id)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteMorgageItem(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteRefById(Guid Id)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteRefById(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteborrowById(Guid Id)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteborrowById(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveGoldLoan(GoldLoan goldloan)
        {
            try
            {
                var result = loanservice.SaveGoldLoan(goldloan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveJewelleryInformation(List<JewelleryInformation> jewelleryInformation)
        {
            try
            {
                var result = loanservice.SaveJewelleryInformation(jewelleryInformation);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveMortgageLoan(MortgageLoan mortgageLoan)
        {
            try
            {
                var result = loanservice.SaveMortgageLoan(mortgageLoan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveMortgageItemInfo(List<MortgageItemInformation> itemInformation)
        {
            try
            {
                var result = loanservice.SaveMortgageItemInfo(itemInformation, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveEducationLoan(EducationLoan educationLoan)
        {
            try
            {
                var result = loanservice.SaveEducationLoan(educationLoan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveEducationInfoDetails(SaveEducationInfo EduInfo)
        {
            try
            {
                var result = loanservice.SaveEducationInfoDetails(EduInfo, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveUpdatedLoanStatus(UpdateLoanStatus updateLoanStatus)
        {
            try
            {
                var result = loanservice.SaveUpdatedLoanStatus(updateLoanStatus);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLoanStatusList(Guid Id)
        {
            try
            {
                var result = loanservice.GetLoanStatusList(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteBorrowerLoan(Guid Id)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteBorrowerLoan(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveGroupLoan(GroupLoanData GrpLoan)
        {
            try
            {
                var result = loanservice.SaveGroupLoan(GrpLoan);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteGroupLoan(Guid Id)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteGroupLoan(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteMember(MemberData MemData)
        {
            bool flag;
            try
            {
                flag = loanservice.DeleteMember(MemData);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage UploadLoanDocuments()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["LoanDocumentPath"];
                var a = HttpContext.Current.Request.Params["data"];
                List<LoanDocuments> list = JsonConvert.DeserializeObject<List<LoanDocuments>>(HttpContext.Current.Request.Params["data"]);
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    int count = HttpContext.Current.Request.Files.AllKeys.Count();

                    for (int i = 0; i < HttpContext.Current.Request.Files.AllKeys.Length; i++)
                    {
                        var httpPostedFile = HttpContext.Current.Request.Files[i];

                        //DocumentType pet = (DocumentType)Enum.Parse(typeof(DocumentType), t.ToString());
                        //int caseOriginCode = (int)(DocumentType)Enum.Parse(typeof(DocumentType), "H/SAPC Registration Certificate");

                        bool folderExists = Directory.Exists(HttpContext.Current.Server.MapPath(path));
                        if (!folderExists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                        }

                        // string timeStamp = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss_tt");
                        Guid filename = Guid.NewGuid();
                        string fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);

                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);

                        if (!File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        else
                        {
                            filename = Guid.NewGuid();
                            fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                            httpPostedFile.SaveAs(fileSavePath);
                        }

                        foreach (var docs in list)
                        {
                            if (docs.LoanDocumentId == Guid.Empty && docs.DocumentName == httpPostedFile.FileName)
                            {
                                //Document  Type t = Enum.GetValues(typeof(DocumentType)).Cast<DocumentType>().FirstOrDefault(v => GetDescription(v) == document.DocumentTypeName);
                                //int number = (int)((DocumentType)Enum.Parse(typeof(DocumentType), t.ToString()));
                                docs.DocumentName = httpPostedFile.FileName;
                                docs.Path = fileName;
                            }
                        }
                    }

                }

                var result = loanservice.UploadLoanDocuments(list);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteDocuments(Guid Id)
        {
            try
            {
                var result = loanservice.DeleteDocuments(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLoanDocumentList(Guid Id)
        {
            try
            {
                var result = loanservice.GetLoanDocumentList(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetChargesList(Guid? Id)
        {
            try
            {
                var result = loanservice.GetChargesList(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllCharges()
        {
            try
            {
                var result = loanservice.GetAllCharges();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveLoanCharges(List<LoanCharges> loanCharges)
        {
            try
            {
                var result = loanservice.SaveLoanCharges(loanCharges);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage LoanAmountisation(LoanAmountisation loanAmountisation)
        {
            try
            {
                var result = loanservice.LoanAmountisation(loanAmountisation);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage LoanAmountisationforPrePayment(LoanAmountisation loanAmountisation)
        {
            try
            {
                var result = loanservice.LoanAmountisationforPrePayment(loanAmountisation);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage DisplayAmountisation(Guid Id)
        {
            try
            {
                var result = loanservice.DisplayAmountisation(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage DeleteCharges(Guid Id)
        {
            try
            {
                var result = loanservice.DeleteCharges(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage DisbursementLetter(Guid Id)
        {
            try
            {
                var result = loanservice.DisbursementLetter(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveAllLoanCharges(AllLoanCharges loanCharges)
        {
            try
            {
                foreach (Guid id in loanCharges.LoanIds)
                {
                    foreach (LoanCharges charge in loanCharges.Charges)
                    {
                        charge.LoanId = id;
                        charge.LoanChargesId = Guid.Empty;
                    }
                    var result = loanservice.SaveLoanCharges(loanCharges.Charges);
                }
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveDisbursementAmount(Loan loan)
        {
            try
            {
                var result = loanservice.SaveDisbursementAmount(loan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage ApproveGroupLoan(ApproveGroupLoan approveGroupLoan)
        {
            try
            {
                var result = loanservice.ApproveGroupLoan(approveGroupLoan, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLoanDetailByIdForPrePayment(Guid Id)
        {
            try
            {
                var result = loanservice.GetLoanDetailByIdForPrePayment(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
