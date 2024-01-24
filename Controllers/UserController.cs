using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;
using RecipeApp.Models;
using RecipeApp.ServicesInterface;
using RecipeApp.Data;
using System.IdentityModel.Tokens.Jwt;

namespace RecipeApp.Controllers
{
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService Service;
        private readonly DataContext dataContext;
        protected String Userid;
        public UserController(DataContext dataContext, IUserService service)
        {
            this.dataContext = dataContext;
            this.Service = service;
        }

        /**
         * register new user
         */
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Response>> RegisterUser([FromBody]UserRegisterData user)
        {
            if (ModelState.IsValid) { 
                var res = await Service.Register(user.UserName, user.Email, user.Password, user.PhoneNumber);
                
              if (res != null)
                {
                    return res;
                }
            }
            else
            {
                Console.Write("Invalid Model State");
            }
            return BadRequest(new Response { });
        }

        /**
         * Login
         */
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Response>> Login([FromBody]UserLoginData user)
        {
            var data = await Service.Login(user.UserName, user.Password);
            return data;
        }

        /**
            Gets User Public Data
        */
        [HttpGet]
        [Route("/")]
        [Route("get/{id}")]
        public async Task<ActionResult<Response>> GetUserById(string id)
        {
            if (id != null)
            {
                var response1 = await Service.GetUserByID(id);
                if (response1 != null)
                {
                    return response1;
                }

                return NotFound(new Response { message = "user not found!" });
            }
            var claims = User.Claims;

            foreach (Claim c in claims)
            {
                Console.Out.WriteLine(c.Type + ": " + c.Value);

            }
            id = claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier).Value;
            var response = await Service.GetUserByID(id, true);
            if (response != null)
            {
                return response;
            }
            System.Diagnostics.Debug.WriteLine("Not Found  2");

            return NotFound(new Response { message = "user not found!" });
        }

        [HttpGet]
        [Route("/user")]
        public async Task<ActionResult<Response>> GetUser()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var tok = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.ReadToken(tok));

                var response = await Service.GetUser(token, true);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();
        }


        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<Response>> DeleteUser()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var tok = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.ReadToken(tok));

                var response = await Service.DeleteUser(token);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();

        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<Response>> UpdateUser([FromBody] User user)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var tok = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.ReadToken(tok));

                var response = await Service.UpdateUser(token, user);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();

        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await Service.GetAllUsers();
            return Ok(users);
        }


        [HttpPut]
        [Route("changepassword")]
        public async Task<ActionResult<Response>> ChangePassword([FromBody] ChangePasswordModel model)
        {

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var tok = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.ReadToken(tok));

                var response = await Service.ChangePassword(token, model.OldPassword, model.NewPassword);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();
        }
        public class ChangePasswordModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
