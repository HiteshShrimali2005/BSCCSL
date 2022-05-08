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
    public class GeneralLedgerDetailsController : ApiController
    {
        GeneralLedgerDetailsService generalLedgerDetailsService;
        public GeneralLedgerDetailsController()
        {
            generalLedgerDetailsService = new GeneralLedgerDetailsService();
        }


        [HttpGet]
        public HttpResponseMessage GetAccountList()
        {
            try
            {
                var result = generalLedgerDetailsService.GetAccountList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetGeneralLedgerList(DateTime Id1, DateTime Id2, Guid Id3,Guid Id4)
        {
            try
            {
                var result = generalLedgerDetailsService.GetGeneralLedgerList(Id1, Id2, Id3, Id4, Request.GetCurrentUser());
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
