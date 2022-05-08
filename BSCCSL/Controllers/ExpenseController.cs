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
    public class ExpenseController : ApiController
    {
        ExpenseService expenseService;

        public ExpenseController()
        {
            expenseService = new ExpenseService();
        }

        [HttpGet]
        public HttpResponseMessage GetExpenseById(Guid Id)
        {
            try
            {
                var result = expenseService.GetExpenseById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetExpenseList(DataTableSearch search)
        {
            try
            {
                var result = expenseService.GetExpenseList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveExpense()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["ExpenseDocumentsPath"];
                var a = HttpContext.Current.Request.Params["data"];
                Expense expense = JsonConvert.DeserializeObject<Expense>(HttpContext.Current.Request.Params["data"]);
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    int count = HttpContext.Current.Request.Files.AllKeys.Count();

                    for (int i = 0; i < HttpContext.Current.Request.Files.AllKeys.Length; i++)
                    {
                        var httpPostedFile = HttpContext.Current.Request.Files[i];

                        //DocumentType pet = (DocumentType)Enum.Parse(typeof(DocumentType), t.ToString());
                        //int caseOriginCode = (int)(DocumentType)Enum.Parse(typeof(DocumentType), "H/SAPC Registration Certificate");

                        bool folderExists = Directory.Exists(HttpContext.Current.Server.MapPath(path));
                        if (!folderExists)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                        }

                        // string timeStamp = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss_tt");
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(httpPostedFile.FileName);
                        expense.FileName = fileName;
                        expense.OrgFileName = httpPostedFile.FileName;
                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);

                        if (!File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        else
                        {
                            fileName = Guid.NewGuid().ToString() + Path.GetExtension(httpPostedFile.FileName);
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                            expense.FileName = fileName;
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                    }

                }

                var result = expenseService.SaveExpense(expense, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteExpenseById(Guid Id)
        {
            try
            {
                var result = expenseService.DeleteExpenseById(Id);
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
                var result = expenseService.GetCustomerDataByProductId(id);
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

        [HttpPost]
        public HttpResponseMessage PaidExpense(Expense expense)
        {
            try
            {
                var result = expenseService.PaidExpense(expense,Request.GetCurrentUser());
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
