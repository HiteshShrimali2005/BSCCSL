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
    public class ActivateRDController : ApiController
    {
        ActivateRDService activateRDService;

        public ActivateRDController()
        {
            activateRDService = new ActivateRDService();
        }


        [HttpPost]
        public HttpResponseMessage GetList(DataTableSearch search)
        {
            try
            {
                var result = activateRDService.GetList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage UnFreezedRDAccount(string Id)
        {
            try
            {
                var result = activateRDService.UnFreezedRDAccount(Id);
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
