using Microsoft.AspNetCore.Mvc;

namespace RecipeApp.Models
{
    public class Response
    {
        public Response()
        {
            message = "";
            result = false;
            data = new { };
        }
        public String message { get; set; }
        public Boolean result { get; set; }
        public Object data { get; set; }
    }
}
