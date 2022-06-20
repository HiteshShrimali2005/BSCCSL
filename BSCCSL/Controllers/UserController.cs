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
    public class UserController : ApiController
    {
        UserService userService;
        public UserController()
        {
            userService = new UserService();
        }

        [HttpGet]
        public HttpResponseMessage Login(string username, string password)
        {
            try
            {
                var result = userService.Login(username, password);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UserRegister(User user)
        {
            try
            {
                var result = userService.UserRegister(user,Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetUserList(DataTableSearch search)
        {
            try
            {
                var result = userService.GetUserList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAllUser(DataTableSearch search)
        {
            try
            {
                var result = userService.GetAllUser(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetUserDataById(Guid Id)
        {
            try
            {
                var result = userService.GetUserDataById(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteUser(Guid Id)
        {
            bool flag;
            try
            {
                flag = userService.DeleteUser(Id);
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

        [HttpPost]
        public HttpResponseMessage GetUsersEmailId(User userdata)
        {
            try
            {
                var flag = false;
                flag = userService.GetUsersEmailId(userdata);
                if (flag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, flag);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UserChangePassword(UserPassworData Userdata)
        {
            try
            {
                var result = userService.UserChangePassword(Userdata);
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
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllEmployee()
        {
            try
            {
                var result = userService.GetAllEmployee();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAgentList(DataTableSearch search)
        {
            try
            {
                var result = userService.GetAgentList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAgentListByBranchId(Guid? Id)
        {
            try
            {
                var result = userService.GetAgentListByBranchId(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetScreeSalesList(DataTableSearch search)
        {
            try
            {
                var result = userService.GetScreeSalesList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAgentRankList(Guid? Id)
        {
            try
            {
                var result = userService.GetAgentRankList(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveAgentHierarchy(List<AgentHierarchy> agentHierarchy)
        {
            try
            {
                var result = userService.SaveAgentHierarchy(agentHierarchy, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, AllowAnonymous]
        public IHttpActionResult GetPassword(string input)
        {
            return Ok(UserService.Decrypt(input));
        }
    }
}
