using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BSCCSL.Models;
using BSCCSL.Services;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using BSCCSL.Extension;

namespace BSCCSL.Controllers
{
    [Authorize]
    public class CustomerController : ApiController
    {
        CustomerService customerService;
        public CustomerController()
        {
            customerService = new CustomerService();
        }

        [HttpPost]
        public HttpResponseMessage CustomerRegister()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["DocumentPath"];
                var a = HttpContext.Current.Request.Params["data"];
                CustomerDetail customerDetail = JsonConvert.DeserializeObject<CustomerDetail>(HttpContext.Current.Request.Params["data"]);
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
                        Guid filename = Guid.NewGuid();
                        string fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);

                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);

                        if (!File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        else
                        {
                            filename = Guid.NewGuid();
                            fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                            httpPostedFile.SaveAs(fileSavePath);
                        }

                        foreach (var Personal in customerDetail.Personal)
                        {
                            foreach (var document in Personal.Documents)
                            {
                                if (document.DocumentId == Guid.Empty && document.DocumentName == httpPostedFile.FileName)
                                {
                                    //DocumentType t = Enum.GetValues(typeof(DocumentType)).Cast<DocumentType>().FirstOrDefault(v => GetDescription(v) == document.DocumentTypeName);
                                    //int number = (int)((DocumentType)Enum.Parse(typeof(DocumentType), t.ToString()));
                                    document.DocumentName = httpPostedFile.FileName;
                                    document.Path = fileName;
                                }
                            }
                        }
                    }


                    //string acno = custmer.AccountNo;
                    //CustomerDetail customerDetail = new CustomerDetail();
                    //customerDetail = custmer.Customer;
                }

                var result = customerService.CustomerRegister(customerDetail);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage CheckCustomerExist(string Id1, DateTime Id2)
        {
            try
            {
                var result = customerService.CheckCustomerExist(Id1,Id2);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UploadHolderPhotograph()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["DocumentPath"];
                var a = HttpContext.Current.Request.Params["data"];
                List<UploadPhotograph> customerDetail = JsonConvert.DeserializeObject<List<UploadPhotograph>>(a);

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
                        Guid filename = Guid.NewGuid();
                        string fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);

                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);

                        if (!File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        else
                        {
                            filename = Guid.NewGuid();
                            fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        foreach (var newdocument in customerDetail)
                        {
                            if (newdocument.NewDocument == httpPostedFile.FileName)
                            {
                                newdocument.Path = fileName;
                            }
                        }
                        //customerDetail[i].Document = fileName;

                    }
                }

                var result = customerService.SaveHolderPhoto(customerDetail);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UploadHolderSign()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["DocumentPath"];
                var a = HttpContext.Current.Request.Params["data"];
                List<UploadPhotograph> customerDetail = JsonConvert.DeserializeObject<List<UploadPhotograph>>(a);

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
                        Guid filename = Guid.NewGuid();
                        string fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);

                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);

                        if (!File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        else
                        {
                            filename = Guid.NewGuid();
                            fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                            httpPostedFile.SaveAs(fileSavePath);
                        }

                        foreach (var newdocument in customerDetail)
                        {
                            if (newdocument.NewDocument == httpPostedFile.FileName)
                            {
                                newdocument.Path = fileName;
                            }
                        }

                        // customerDetail[i].Document = fileName;

                    }
                }
                var result = customerService.SaveHolderSign(customerDetail);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetBranchCode(Guid Id)
        {
            try
            {
                var result = customerService.GetBranchCode(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetEmployeeDetail(DataTableSearch search)
        {
            try
            {
                var result = customerService.GetEmployeeDetail(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEmployeeDetailById(Guid id)
        {
            try
            {
                var result = customerService.GetEmployeeDetailById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetBalanceById(Guid id)
        {
            try
            {
                var result = customerService.GetBalanceById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetCustomerList(DataTableSearch search)
        {
            try
            {
                var result = customerService.GetCustomerList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteCustomer(Guid Id)
        {
            bool flag;
            try
            {
                flag = customerService.DeleteCustomer(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        //Get CustomerDetailsById
        [HttpGet]
        public HttpResponseMessage GetCustomerDetailsbyId(Guid Id)
        {
            try
            {
                var result = customerService.GetCustomerDetailsbyId(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerDetailsForAgentCreationbyId(Guid Id)
        {
            try
            {
                var result = customerService.GetCustomerDetailsForAgentCreationbyId(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetHolderData(string id)
        {
            try
            {
                var result = customerService.GetHolderData(id);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage gettransactionDetails(Guid id)
        {
            try
            {
                var result = customerService.gettransactionDetails(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveCustomerShare(CustomerShare customershare)
        {
            try
            {
                var result = customerService.SaveCustomerShare(customershare);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetShareList(DataTableSearch search)
        {
            try
            {
                var result = customerService.GetShareList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetShareDataById(Guid id)
        {
            try
            {
                var result = customerService.GetShareDataById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetShareDetailForPrint(Guid ID)
        {
            try
            {

                var result = customerService.GetShareDetailForPrint(ID);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        //[HttpPost]
        //public HttpResponseMessage UpdateBalance(CustomerUpdateData customer)
        //{
        //    try
        //    {
        //        var result = customerService.UpdateBalance(customer);
        //        return Request.CreateResponse(HttpStatusCode.OK, result);

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogService.InsertLog(ex);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        [HttpPost]
        public HttpResponseMessage SaveNominee(Nominee nominee)
        {
            try
            {
                var result = customerService.SaveNominee(nominee, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteShare(Guid Id)
        {
            bool flag;
            try
            {
                flag = customerService.DeleteShare(Id);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetHolders(Guid id)
        {
            try
            {
                var result = customerService.GetHolders(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerDocument(Guid Id)
        {
            try
            {
                var result = customerService.GetCustomerDocument(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UploadCustomerDocument()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["DocumentPath"];
                var a = HttpContext.Current.Request.Params["data"];
                var CustomerId = HttpContext.Current.Request.Params["CustomerId"];
                var DocumentName = HttpContext.Current.Request.Params["documentname"];
                List<CustomerDocuments> customerDetail = JsonConvert.DeserializeObject<List<CustomerDocuments>>(a);


                CustomerDocuments objCustomerDocuments = new CustomerDocuments();
                objCustomerDocuments.CustomerId = JsonConvert.DeserializeObject<Guid>(CustomerId);
                objCustomerDocuments.DocumentName = JsonConvert.DeserializeObject<string>(DocumentName);
                objCustomerDocuments.IsDeleted = false;
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
                        Guid filename = Guid.NewGuid();
                        string fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);

                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);

                        if (!File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        else
                        {
                            filename = Guid.NewGuid();
                            fileName = filename.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                        objCustomerDocuments.Path = fileName;
                        //foreach (var newdocument in customerDetail)
                        //{
                        //    if (newdocument.NewDocument == httpPostedFile.FileName)
                        //    {
                        //        newdocument.Path = fileName;
                        //    }
                        //}

                        // customerDetail[i].Document = fileName;

                    }
                }
                var result = customerService.SaveCustomerDocuments(objCustomerDocuments);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage DeleteCustomerDocument(Guid Id)
        {
            try
            {
                var result = customerService.DeleteCustomerDocument(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RefundShareAmount(Guid Id)
        {

            int Count = 0;
            try
            {
                Count = customerService.RefundShareAmount(Id, Request.GetCurrentUser());
                if (Count != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Count);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }



    }
}
