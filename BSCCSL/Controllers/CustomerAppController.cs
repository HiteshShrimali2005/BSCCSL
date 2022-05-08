using BSCCSL.Models;
using BSCCSL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BSCCSL.Extension;

namespace BSCCSL.Controllers
{

    public class CustomerAppController : ApiController
    {
        private readonly CustomerAppService customerAppService;

        public CustomerAppController()
        {
            customerAppService = new CustomerAppService();
        }

        [HttpPost]
        public HttpResponseMessage Login(CustomerLogin login)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.Login(login));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage Register(CustomerRegister customerRegister)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.Register(customerRegister));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage ForgotPassword(CustomerForgotPassword customerForgotPassword)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.ForgotPassword(customerForgotPassword));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage VerifyToken(VerifyOTP verifyOTP)
        {
            try
            {
                customerAppService.VerifyToken(verifyOTP);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage SetPassword(SetPasswordModel setPasswordModel)
        {
            try
            {
                customerAppService.SetPassword(setPasswordModel);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, CustomerAuthorization]
        public HttpResponseMessage GetAccountList(Guid Id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.GetAccountList(Id));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, CustomerAuthorization]
        public HttpResponseMessage GetAccountDetail(Guid Id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.GetAccountDetail(Id));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, CustomerAuthorization]
        public HttpResponseMessage GetCustomerDetail(Guid Id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.GetCustomerDetail(Id));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, CustomerAuthorization]
        public HttpResponseMessage SaveProductEnquiry(ProductEnquiry productEnquiry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customerAppService.SaveProductEnquiry(productEnquiry);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, CustomerAuthorization]
        public HttpResponseMessage GetNotification(Guid Id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.GetNotification(Id));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage CheckUpdate(string CurrentVersion, string Platform)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, customerAppService.CheckUpdate(CurrentVersion, Platform));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}