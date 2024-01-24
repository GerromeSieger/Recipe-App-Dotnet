using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace RecipeApp.Models
{
    public class userDataResponse
    {
        public string UId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Jwt { get; set; }

    }

    public class userDataResponse_No_JWT
    {
        public string UId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Jwt { get; set; }

    }
}
