using cafe_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace cafe_management_system.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        CafeEntities db = new CafeEntities();
        [HttpPost, Route("signup")]
        public HttpResponseMessage Signup([FromBody] User user)
        {
            try
            {
                User userObj = db.Users.FirstOrDefault(u => u.email == user.email);
                if (userObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "User already exists with this email id");
                }
                else
                {
                    user.status = "true";
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "User registered successfully");
                }
                
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("log")]
        public HttpResponseMessage Login([FromBody] User user)
        {
            try
            {
                User userObj = db.Users.FirstOrDefault(u => u.email == user.email);
                if (userObj != null)
                {
                    if(userObj.status == "false")
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, "Wait for Admin Approval");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token=TokenManager.GenerateToken(userObj.email,userObj.role)});
                    }
                  
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid email or password");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
