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
    public class JournalVoucherController : ApiController
    {
        JournalVoucherService journalVoucherService;

        public JournalVoucherController()
        {
            journalVoucherService = new JournalVoucherService();
        }

        [HttpGet]
        public HttpResponseMessage GetJournalVoucherById(Guid Id)
        {
            try
            {
                var result = journalVoucherService.GetJournalVoucherById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetJournalVoucherList(DataTableSearch search)
        {
            try
            {
                var result = journalVoucherService.GetJournalVoucherList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveJournalVoucher(JournalVoucher journalVoucher)
        {
            try
            {
                var result = journalVoucherService.SaveJournalVoucher(journalVoucher, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteJournalVoucherById(Guid Id)
        {
            try
            {
                var result = journalVoucherService.DeleteJournalVoucherById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerDataByProductId(string id)
        {
            try
            {
                var result = journalVoucherService.GetCustomerDataByProductId(id);
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

    }
}
