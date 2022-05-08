using BSCCSL.Extension;
using BSCCSL.Models;
using BSCCSL.Models.Accounting;
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
    public class ChartsofAccountController : ApiController
    {
        ChartsofAccountService chartsofAccountService;
        public ChartsofAccountController()
        {
            chartsofAccountService = new ChartsofAccountService();
        }


        [HttpPost]
        public HttpResponseMessage GetChartofAccountList(DataTableSearch search)
        {
            try
            {
                var result = chartsofAccountService.GetChartofAccountList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetParentAccounts()
        {
            try
            {
                var result = chartsofAccountService.GetParentAccouts();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetSubAccounts(Guid Id)
        {
            try
            {
                var result = chartsofAccountService.GetSubAccounts(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetAccountDetailsById(Guid Id)
        {
            try
            {
                var result = chartsofAccountService.GetAccountDetailsById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveAccount(Accounts objAccounts)
        {
            try
            {
                var result = chartsofAccountService.SaveAccount(objAccounts, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage DeleteAccount(Guid Id)
        {
            try
            {
                var result = chartsofAccountService.DeleteAccount(Id);
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
