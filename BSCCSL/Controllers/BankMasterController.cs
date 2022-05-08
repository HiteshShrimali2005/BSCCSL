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
    public class BankMasterController : ApiController
    {
        BankMasterService _bankService;
        
        public BankMasterController()
        {
            _bankService = new BankMasterService();
        }


        [HttpPost]
        public HttpResponseMessage GetAllBankList(DataTableSearch search)
        {
            try
            {
                var result = _bankService.GetAllBankList(search,Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveBankDetails(BankDetails bank)
        {
            try
            {
                if (bank.Bank.BankId != Guid.Empty) {
                    var result2 = _bankService.RemoveAllPresentMapping(bank.Bank.BankId);
                        }

                var result = _bankService.SaveBankDetails(bank);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteBank(Guid Id)
        {
            bool flag;
            try
            {
                flag = _bankService.DeleteBank(Id);
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
        
        [HttpGet]
        public HttpResponseMessage GetBankData(Guid Id)
        {
            try
            {
                var result = _bankService.GetBankData(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        
        [HttpGet]
        public HttpResponseMessage GetBankDetailById(Guid Id)
        {
            try
            {
                var result = _bankService.GetBankDetailById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetBankTransactionData(DataTableSearch search)
        {
            try
            {
                var result = _bankService.GetBankTransactionData(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveBankTransaction(BankTransaction transaction)
        {
            try
            {
                var result = _bankService.SaveBankTransaction(transaction);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage gettransactionDetails(Guid id)
        {
            try
            {
                var result = _bankService.gettransactionDetails(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateTransactionData(BankTransaction Transactiondata)
        {
            try
            {
                var result = _bankService.UpdateTransactionData(Transactiondata, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateTransactionDataForBounce(BankTransaction BounceDetails)
        {
            try
            {
                var result = _bankService.UpdateTransactionDataForBounce(BounceDetails);
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
            BankMaster objBankMaster = new BankMaster();
            try
            {
                objBankMaster = _bankService.DeleteLastTransaction(Id, Request.GetCurrentUser());
                if (objBankMaster != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objBankMaster);
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
