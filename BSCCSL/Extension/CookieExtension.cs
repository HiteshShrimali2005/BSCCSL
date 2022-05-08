using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Script.Serialization;

namespace BSCCSL.Extension
{
    public static class CookieExtension
    {
        public static User GetCurrentUser(this HttpRequestMessage Request)
        {
            try
            {
                var UserData = Request.Headers.GetCookies("User").FirstOrDefault();
                if (UserData != null)
                {
                    return new JavaScriptSerializer().Deserialize<User>(UserData["User"].Value);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}