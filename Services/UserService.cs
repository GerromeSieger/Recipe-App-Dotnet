using Microsoft.AspNetCore.Identity;
using RecipeApp.ServicesInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeApp.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Reflection;
using RecipeApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;


namespace RecipeApp.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext dataContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signinManager;
        private readonly jwtSettings JWTSettings;

        public UserService(DataContext dataContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinManager, IOptions<jwtSettings> jwtSettings)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
            this.signinManager = signinManager;
            this.JWTSettings = jwtSettings.Value;
        }

        public async Task<Response> Register(string Username, string Email, string Password, string Number)
        {
            var NewUser = new User
            {
                UserName = Username,
                Email = Email,
                PasswordHash = Password,
                PhoneNumber = Number
            };

            var result = await userManager.CreateAsync(NewUser, Password);

            if (result.Succeeded)
            {
                // Generate a random 6 digit OTP code
                var otp = RandomNumberGenerator.GetInt32(100000, 999999);

                // Save OTP to user table with expiration time
                NewUser.OTP = otp;

                // Send confirmation email with OTP code
                return new Response()
                {
                    result = true,
                    message = "User Created Successfully",
                    data = NewUser
                };
            }

            // Return error response
            return new Response
            {
                message = result.Errors.FirstOrDefault()?.Description
            };
        }

        public async Task<Response> Login(string Username, string Password)
        {
            var user = (User) dataContext.Users.FirstOrDefault(a => a.UserName == Username || a.Email == Username);
            if (user == null)
            {
                return new Response
                {
                    message = "userNotFound"
                };
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(this.JWTSettings.jwtKey);
            var res = await signinManager.PasswordSignInAsync(user.UserName, Password, false, true);

            if (!res.Succeeded)
            {
                return new Response
                {
                    result = false,
                    message = "Incorrect Password"
                };
            }

            Claim[] c = new Claim[]{
                new Claim("UserName", Username),
                // new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id),
                //new Claim("jwt", )
            };
            var tokenDesc = new SecurityTokenDescriptor
            {
                Issuer = this.JWTSettings.iss,
                Subject = new ClaimsIdentity(c),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)

            };
            var token = tokenHandler.CreateToken(tokenDesc);
            string finalTokenHandler = tokenHandler.WriteToken(token);
            return new Response
            {
                result = true,
                message = "Login Successful",
                data = new userDataResponse
                {
                    UId = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    Jwt = finalTokenHandler
                }
            };

        }
        public async Task<bool> EmailExists(string Email)
        {
            var test = await userManager.FindByEmailAsync(Email);
            return test != null;
        }

        public async Task<bool> UsernameExist(string Username)
        {
            var test = await userManager.FindByNameAsync(Username);
            return test != null;
        }

        public async Task<Response> GetUserByID(string ID, bool loggedin = false)
        {
            var user = (User)await userManager.FindByIdAsync(ID);
            if (user != null)
            {
                if (loggedin)
                {
                    return new Response()
                    {
                        result = true,
                        message = "" ,
                        data = user
                    };
                } else
                {
                    return new Response() 
                    { 
                        result = true,
                        message = "",
                        data = user
                    };

                }

            }
            return null;
        }
        public async Task<Response> GetUser(string token, bool loggedin = false)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;

            var user = (User)await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (loggedin)
                {
                    return new Response()
                    {
                        result = true,
                        message = "This is " + user.UserName,
                        data = user
                    };
                }
                else
                {
                    return new Response()
                    {
                        result = true,
                        message = "This is " + user.UserName,
                        data = user
                    };

                }

            }
            return null;
        }
        public async Task<Response> DeleteUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;

            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new Response
                    {
                        result = true,
                        message = "User deleted successfully"
                    };
                }
                else
                return new Response
                {
                    message = "Error deleting user"
                };
            }
            else
            return new Response
            {
                message = "User not found"
            };
        }

        public async Task<Response> UpdateUser(string token, User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;

            var existuser = await dataContext.User.FindAsync(userId);
            var existingUser = await userManager.FindByIdAsync(userId);
            if (existingUser != null)
            {
                if (!string.IsNullOrEmpty(user.UserName))
                    existingUser.UserName = user.UserName;

                if (!string.IsNullOrEmpty(user.Email))
                    existingUser.Email = user.Email;

                if (!string.IsNullOrEmpty(user.PhoneNumber))
                    existingUser.PhoneNumber = user.PhoneNumber;

                if (!string.IsNullOrEmpty(user.FullName))
                    existuser.FullName = user.FullName;

                if (!string.IsNullOrEmpty(user.Bio))
                    existuser.Bio = user.Bio;

                if (!string.IsNullOrEmpty(user.Country))
                    existuser.Country = user.Country;

                if (!string.IsNullOrEmpty(user.PasswordHash))
                    existuser.PasswordHash = user.PasswordHash;
            

                var result = await userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return new Response
                    { 
                        result = true,
                        message = "User Updated Successfully",
                        data = existingUser
                    };
                }
                else
                return new Response
                {

                    message = "Error updating user"
                };
            }
            else
            return new Response
            {
                message = "User not found"
            };
        }

        public async Task<Response> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (users.Count == 0)
            {
                return new Response { message = "No users found" };
            }

            var userDataList = new List<userDataResponse>();
            foreach (var user in users)
            {
                userDataResponse userData = new userDataResponse
                {
                    UId = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                };
                userDataList.Add(userData);
            }

            return new Response { result = true, data = userDataList };
        }

        public async Task<Response> ChangePassword(string token, string oldPassword, string newPassword)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Response { message = "User not found" };
            }

            var res = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!res.Succeeded)
            {
                return new Response
                {
                    message = string.Join(",", res.Errors.Select(e => e.Description))
                };
            }

            if (newPassword == oldPassword)
            {
                return new Response
                {
                    message = "New password must not match old password"
                };
            }

            return new Response
            {
                result = true,
                message = "Password changed successfully"
            };
        }
    }
}
