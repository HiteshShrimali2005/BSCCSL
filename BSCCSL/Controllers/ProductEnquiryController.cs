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
    public class ProductEnquiryController : ApiController
    {

        ProductEnquiryService productEnquiryService;

        public ProductEnquiryController()
        {
            productEnquiryService = new ProductEnquiryService();
        }

        [HttpPost]
        public HttpResponseMessage GetProductEnquiryList(DataTableSearch search)
        {
            try
            {
                var result = productEnquiryService.GetProductEnquiryList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveProductEnquiry(ProductEnquiry productEnquiry)
        {
            try
            {
                var result = productEnquiryService.SaveProductEnquiry(productEnquiry, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetProductEnquiryById(Guid Id)
        {
            try
            {
                var result = productEnquiryService.GetProductEnquiryById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }


    }
}
