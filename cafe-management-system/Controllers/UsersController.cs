using cafe_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
                    if (userObj.status == "false")
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, "Wait for Admin Approval");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManager.GenerateToken(userObj.email, userObj.role) });
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
        [HttpGet, Route("checkToken")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage CheckToken()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var emailClaim = identity.FindFirst(ClaimTypes.Email);
                var roleClaim = identity.FindFirst(ClaimTypes.Role);
                if (emailClaim != null && roleClaim != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { email = emailClaim.Value, role = roleClaim.Value });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid token");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpGet, Route("getAllUsers")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllUsers()
        {
            try
            {
                var token=Request.Headers.GetValues("Authorization").FirstOrDefault();
                var tokenClaim = TokenManager.ValidateToken(token);
                if(tokenClaim.Role!= "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Access denied");
                }
                var result = db.Users.ToList();
                if (result.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No users found");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
