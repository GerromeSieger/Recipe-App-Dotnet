using System.Security.Claims;
using RecipeApp.Models;

namespace RecipeApp.ServicesInterface
{
    public interface IUserService
    {
        Task<Response> Register(String Username, String Email, string Password, string Number);
        Task<Response> Login(string Username, string Password);
        Task<Response> GetUserByID(string ID, bool loggedin = false);
        Task<Response> GetUser(string token, bool loggedin = false);
        Task<bool> EmailExists(string Email);
        Task<bool> UsernameExist(string Username);
        Task<Response> DeleteUser(string token);
        Task<Response> UpdateUser(string token, User user);
        Task<Response> ChangePassword(string token, string oldPassword, string newPassword);
        Task<Response> GetAllUsers();
    }
}
