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
    public class ProfitandLossStatementController : ApiController
    {
        ProfitandLossStatementService profitandLossStatementService;
        public ProfitandLossStatementController()
        {
            profitandLossStatementService = new ProfitandLossStatementService();
        }


        [HttpGet]
        public HttpResponseMessage GetProfitandLossStatement(DateTime Id1, DateTime Id2, Guid Id3)
        {
            try
            {
                var result = profitandLossStatementService.GetProfitandLossStatement(Id1, Id2, Id3, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage ClosePandLFinancialYear(PandLClosingYearModel data)
        {
            try
            {
                var savedata = profitandLossStatementService.ClosePandLFinancialYear(data, Request.GetCurrentUser());
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
