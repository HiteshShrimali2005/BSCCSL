using BSCCSL.Extension;
using BSCCSL.Models;
using BSCCSL.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSCCSL.Controllers
{
    [Authorize]
    public class DDSPaymentController : ApiController
    {
        DDSPaymentService ddsPaymentService;

        public DDSPaymentController()
        {
            ddsPaymentService = new DDSPaymentService();
        }


        [HttpPost]
        public HttpResponseMessage GetDDSPaymentList(DataTableSearch search)
        {
            try
            {
                var result = ddsPaymentService.GetDDSPaymentList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddPayment(DDSPaymentListModel ddspaymentmodel)
        {
            try
            {
                var result = ddsPaymentService.AddPayment(ddspaymentmodel, Request.GetCurrentUser());
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
