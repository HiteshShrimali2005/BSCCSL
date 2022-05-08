using BSCCSL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace BSCCSL.Extension
{
    public class CustomerAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            IEnumerable<string> CustomerIds;
            if (actionContext.Request.Headers.TryGetValues("X-CustomerId", out CustomerIds) && CustomerIds.Count() > 0)
            {
                var StrCustomerId = CustomerIds.First();
                Guid CustomerId;
                if(Guid.TryParse(StrCustomerId, out CustomerId))
                {
                    if (IsCustomerActive(CustomerId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsCustomerActive(Guid CustomerId)
        {
            if(CustomerId != Guid.Empty)
            {
                using (var db = new BSCCSLEntity())
                {
                    var customer = db.Customer.Where(x => !x.IsDelete && x.CustomerId == CustomerId).FirstOrDefault();
                    if(customer != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}