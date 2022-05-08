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
    public class TrailBalanceController : ApiController
    {
        TrailBalanceService trailBalanceService;
        public TrailBalanceController()
        {
            trailBalanceService = new TrailBalanceService();
        }


        [HttpPost]
        public HttpResponseMessage GetTrialBalanceListforParentAccount(ReportSearch data)
        {
            try
            {
                var Accountlistdata = trailBalanceService.GetTrialBalanceListforParentAccount(data, Request.GetCurrentUser());
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
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
        public HttpResponseMessage GetTrialBalanceListforSubAccount(AccountDetails data)
        {
            try
            {
                var Accountlistdata = trailBalanceService.GetTrialBalanceListforSubAccount(data, Request.GetCurrentUser());
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
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
        public HttpResponseMessage GetTrialBalanceListforChildAccount(AccountDetails data)
        {
            try
            {
                var Accountlistdata = trailBalanceService.GetTrialBalanceListforChildAccount(data, Request.GetCurrentUser());
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
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
        public HttpResponseMessage SaveFinancialYearClosingBalance(FinancialYearViewModel data)
        {
            try
            {
                var savedata = trailBalanceService.SaveFinancialYearClosingBalance(data, Request.GetCurrentUser());
                if (savedata != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, savedata);
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


    }
}
