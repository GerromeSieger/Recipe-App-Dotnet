using Microsoft.AspNetCore.Identity;

namespace RecipeApp.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }
        public int OTP { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
