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
    public class AgentCommissionController : ApiController
    {
        AgentCommissionService _commissionService;

        public AgentCommissionController()
        {
            _commissionService = new AgentCommissionService();
        }

        [HttpPost]
        public HttpResponseMessage GetAgentCommissionList(ReportSearch Search)
        {
            try
            {
                var agentCustomerlist = _commissionService.GetAgentCommissionList(Search,Request.GetCurrentUser());
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
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
        public HttpResponseMessage GetCommissionSubData(CommissionSubdata data)
        {
            try
            {
                var Accountlistdata = _commissionService.GetCommissionSubData(data,Request.GetCurrentUser());
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
        public HttpResponseMessage AgentHierarchyCommissionByMonthAndYear(CommissionSubdata data)
        {
            try
            {
                var Accountlistdata = _commissionService.AgentHierarchyCommissionByMonthAndYear(data, Request.GetCurrentUser());
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
        public HttpResponseMessage SaveAgentCommissionPayment(CommissionData data)
        {
            try
            {
                var result = _commissionService.SaveAgentCommissionPayment(data);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RptAgentHierarchy(DataTableSearch search)
        {
            try
            {
                var result = _commissionService.RptAgentHierarchy(search, Request.GetCurrentUser());
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
