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
    public class IncomeController : ApiController
    {
        IncomeService incomeService;

        public IncomeController()
        {
            incomeService = new IncomeService();
        }

        [HttpGet]
        public HttpResponseMessage GetIncomeById(Guid Id)
        {
            try
            {
                var result = incomeService.GetIncomeById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetIncomeList(DataTableSearch search)
        {
            try
            {
                var result = incomeService.GetIncomeList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveIncome(Income income)
        {
            try
            {
                var result = incomeService.SaveIncome(income, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteIncomeById(Guid Id)
        {
            try
            {
                var result = incomeService.DeleteincomeById(Id);
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
