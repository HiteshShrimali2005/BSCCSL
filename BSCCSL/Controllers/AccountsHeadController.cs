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
    public class AccountsHeadController : ApiController
    {
        AccountsHeadService accountsHeadService;

        public AccountsHeadController()
        {
            accountsHeadService = new AccountsHeadService();
        }

        [HttpGet]
        public HttpResponseMessage GetaccountsheadDataById(Guid Id)
        {
            try
            {
                var result = accountsHeadService.GetaccountsheadDataById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAccountListforParent()
        {
            try
            {
                var result = accountsHeadService.GetAccountListforParent();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAccountsHead(DataTableSearch search)
        {
            try
            {
                var result = accountsHeadService.GetAccountsHead(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveAccountsHead(AccountsHead accountsHead)
        {
            try
            {
                var result = accountsHeadService.SaveAccountsHead(accountsHead, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteAccountsHead(Guid Id)
        {
            try
            {
                var result = accountsHeadService.DeleteAccountsHead(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetHeadWiseAccountsHead(HeadType Id)
        {
            try
            {
                var result = accountsHeadService.GetHeadWiseAccountsHead(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSubHead(HeadType Id)
        {
            try
            {
                var result = accountsHeadService.GetSubHead(Id);
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
