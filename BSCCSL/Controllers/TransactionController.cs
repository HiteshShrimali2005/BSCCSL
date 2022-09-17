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
    public class TransactionController : ApiController
    {
        TransactionService transactionService;
        public TransactionController()
        {
            transactionService = new TransactionService();
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerDataByProductId(string id)
        {
            try
            {
                var result = transactionService.GetCustomerDataByProductId(id);
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
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetTransactionData(DataTableSearch search)
            {
            try
            {
                var result = transactionService.GetTransactionData(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)

            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveTransaction(Transactions transaction)
        {
            try
            {
                var result = transactionService.SaveTransaction(transaction);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateTransactionData(Transactions Transactiondata)
        {
            try
            {
                var result = transactionService.UpdateTransactionData(Transactiondata, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateTransactionDataForBounce(Transactions BounceDetails)
        {
            try
            {
                var result = transactionService.UpdateTransactionDataForBounce(BounceDetails);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetUnclearChequeList(DataTableSearch search)
        {
            try
            {
                var result = transactionService.GetUnclearChequeList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetClearCheckList(DataTableSearch search)
        {
            try
            {
                var result = transactionService.GetClearCheckList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetTransactionRptData(DataTableSearch Search)
        {
            try
            {
                var Transactiondata = transactionService.GetTransactionRptData(Search);
                if (Transactiondata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Transactiondata);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PrintPassbookData(PassbookPrintSearch search)
        {
            try
            {
                var Transactiondata = transactionService.PrintPassbookData(search);
                return Request.CreateResponse(HttpStatusCode.OK, Transactiondata);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SavePrintPassbook(PassbookPrint printPassbook)
        {
            try
            {
                var result = transactionService.SavePrintPassbook(printPassbook);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage PrintAccountDetailOnPassbook(string id)
        {
            try
            {
                var result = transactionService.PrintAccountDetailOnPassbook(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateRDPaymentStatus(RDPendingPayment rdPendingPayment)
        {
            try
            {
                var result = transactionService.UpdateRDPaymentStatus(rdPendingPayment, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage TransactionStatement(PrintStatement printStatement)
        {
            try
            {
                var result = transactionService.TransactionStatement(printStatement);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddPrePayment(LoanPrePayment loanPrePayment)
        {
            try
            {
                var result = transactionService.AddPrePayment(loanPrePayment, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage DeleteLastTransaction(Guid Id)
        {
            CustomerProduct objCustomerProduct = new CustomerProduct();
            try
            {
                objCustomerProduct = transactionService.DeleteLastTransaction(Id, Request.GetCurrentUser());
                if (objCustomerProduct != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objCustomerProduct);
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

    }
}
