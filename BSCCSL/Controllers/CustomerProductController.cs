using BSCCSL.Extension;
using BSCCSL.Models;
using BSCCSL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSCCSL.Controllers
{
    [Authorize]
    public class CustomerProductController : ApiController
    {
        CustomerProductService customerProductService;
        public CustomerProductController()
        {
            customerProductService = new CustomerProductService();
        }

        [HttpPost]
        public HttpResponseMessage CheckCustomerAccountExist(AccountExist accountExist)
        {
            try
            {
                var result = customerProductService.CheckCustomerAccountExist(accountExist);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSavingAccountNo(Guid Id)
        {
            try
            {

                var result = customerProductService.GetSavingAccountNo(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetProductNameAsSelectedType(ProductType Id)
        {
            try
            {
                var result = customerProductService.GetProductNameAsSelectedType(Id);
                if (result != null)

                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetProductNameOfGroupLoan()
        {
            try
            {
                var result = customerProductService.GetProductNameOfGroupLoan();
                if (result != null)

                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetProductDetailsAsSelectedName(Guid Id)
        {
            try
            {
                var result = customerProductService.GetProductDetailsAsSelectedName(Id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveCustomerProductData(CustomerProduct CustomerProductdata)
        {
            try
            {
                var result = customerProductService.SaveCustomerProductData(CustomerProductdata, Request.GetCurrentUser());
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        //for display list of product of perticuler customer
        [HttpPost]
        public HttpResponseMessage GetAllProductDataByCustomerId(DataTableSearch search)
        {
            try
            {
                var result = customerProductService.GetAllProductDataByCustomerId(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // For Edit Product of customer
        [HttpGet]
        public HttpResponseMessage GetCustomerProductById(Guid Id)
        {
            try
            {
                var result = customerProductService.GetCustomerProductById(Id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPremetureRIPData(Guid Id)
        {
            try
            {
                var result = customerProductService.GetPremetureRIPData(Id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetProductPrintData(Guid Id)
        {
            try
            {
                var result = customerProductService.GetProductPrintData(Id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetRIPPrintData(Guid Id)
        {
            try
            {
                var result = customerProductService.GetRIPPrintData(Id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage RIPMaturityCalculation(CalculateRIP calculate)
        {
            try
            {
                var result = customerProductService.RIPMaturityCalculation(calculate);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage MISMaturityCalculation(CalculateRIP calculate)
        {
            try
            {
                var result = customerProductService.MISMaturityCalculation(calculate);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAkshyaTrityaPrintData(Guid Id)
        {
            try
            {
                var result = customerProductService.GetAkshyaTrityaPrintData(Id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SubmitCustomerRDFD(CustomerProduct CustomerProduct)
        {
            try
            {
                var result = customerProductService.SubmitCustomerRDFD(CustomerProduct);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerByIdForProduct(Guid id)
        {
            try
            {
                var result = customerProductService.GetCustomerByIdForProduct(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetInterestRate(int? Id1, TimePeriod? Id2)
        {
            try
            {
                var result = customerProductService.GetInterestRate(Id1, Id2);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage CalculateMaturityAmount(CalculateMaturityAmount calculateAmount)
        {
            try
            {
                var result = customerProductService.CalculateMaturityAmount(calculateAmount);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage PendingRDInstallments(Guid Id)
        {
            try
            {
                var result = customerProductService.PendingRDInstallments(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerRDAccount(Guid Id)
        {
            try
            {
                var result = customerProductService.GetCustomerRDAccount(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetCustomerMappedRDAccount(Guid Id)
        {
            try
            {
                var result = customerProductService.GetCustomerMappedRDAccount(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage CalculatePrematureWithdrawalRDFD(Guid Id)
        {
            try
            {
                var result = customerProductService.CalculatePrematureWithdrawalRDFD(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PrematureWithDrawalRDFD(PrematureRDFD PrematureRDFD)
        {
            try
            {
                var result = customerProductService.PrematureWithDrawalRDFD(PrematureRDFD, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PrematureWithDrawalRIP(PrematureRIPData PrematureRIP)
        {
            try
            {
                var result = customerProductService.PrematureWithDrawalRIP(PrematureRIP, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage UpdatePrintFlag(Guid Id)
        {
            try
            {
                var result = customerProductService.UpdatePrintFlag(Id);
                if (result)
                {
                    PrintCertificateHistory printCertificateHistory = new PrintCertificateHistory();
                    printCertificateHistory.CustomerProductId = Id;
                    customerProductService.SaveCertificatePrintHistory(printCertificateHistory, Request.GetCurrentUser());
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
        public HttpResponseMessage SaveCertificatePrintHistory(PrintCertificateHistory printCertificateHistory)
        {
            try
            {
                var result = customerProductService.SaveCertificatePrintHistory(printCertificateHistory, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetCertificatePrintHistory(DataTableSearch search)
        {
            try
            {
                var result = customerProductService.GetCertificatePrintHistory(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage CloseCustomerAccount(Guid Id)
        {
            try
            {
                var result = customerProductService.CloseCustomerAccount(Id, Request.GetCurrentUser());
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
