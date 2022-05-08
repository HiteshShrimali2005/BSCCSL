using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using BSCCSL.Models;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using BSCCSL.Services;
using System.Web.Script.Serialization;

namespace BSCCSL.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (var obj = new BSCCSLEntity())
            {
                string decpwd = UserService.Decrypt("HI+zKCbwgXYuTNbM4/eRCA==");
                //comment by vishal brlow 1 line code and pass hardcore pwd
                //string pwd = UserService.Encrypt(context.Password);
                string pwd = UserService.Encrypt(decpwd);

                //User user = obj.User.Where<User>(record => record.UserCode == context.UserName && record.Password == pwd && record.IsActive == true && record.IsDelete == false && (record.Role != Role.Agent && record.Role != Role.Scree_Sales)).FirstOrDefault();
                User user = obj.User.Where<User>(record => record.UserName == context.UserName && record.Password == pwd && record.IsActive == true && record.IsDelete == false && (record.Role != Role.Agent)).FirstOrDefault();

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                //context.Response.Cookies.Append("User", new JavaScriptSerializer().Serialize(user));


                ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                ClaimsIdentity cookiesIdentity = new ClaimsIdentity(context.Options.AuthenticationType);

                AuthenticationProperties properties = CreateProperties(user);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            //validate your client  
            //var currentClient = context.ClientId;  

            if (context.ClientId == null)
            {// Change authentication ticket for refresh token requests 
                var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
                newIdentity.AddClaim(new Claim("newClaim", "newValue"));

                var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
                context.Validated(newTicket);

                return Task.FromResult<object>(null);
            }
            else
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string,
            string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication
        (OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }
            else
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                context.Rejected();
            }


            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(User user)
        {

            var userdata = new
            {
                UserId = user.UserId,
                BranchId = user.BranchId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = Convert.ToInt32(user.Role),
                RoleName = Enum.GetName(typeof(Role), user.Role),
                UserCode = user.UserCode
            };

            IDictionary<string, string>
            data = new Dictionary<string, string>
            {
                { "UserId", userdata.UserId.ToString() },
                { "BranchId" , userdata.BranchId.ToString()},
                { "FirstName" , userdata.FirstName.ToString()},
                { "LastName" , userdata.LastName.ToString()},
                { "Role" , userdata.Role.ToString()},
                { "RoleName" , user.RoleName.ToString()},
                { "UserCode" , user.UserCode.ToString()},
            };

            return new AuthenticationProperties(data);
        }
    }
}