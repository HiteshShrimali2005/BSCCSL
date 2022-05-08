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
    public class CashFlowController : ApiController
    {
        CashFlowService cashFlowService;
        public CashFlowController()
        {
            cashFlowService = new CashFlowService();
        }


        [HttpGet]
        public HttpResponseMessage GetCashFlowData(DateTime Id1, DateTime Id2, Guid Id3)
        {
            try
            {
                var result = cashFlowService.GetCashFlowData(Id1, Id2, Id3, Request.GetCurrentUser());
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
