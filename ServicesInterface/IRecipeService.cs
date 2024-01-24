using RecipeApp.Models;
using System.Security.Claims;

namespace RecipeApp.ServicesInterface
{
    public interface IRecipeService
    {
        Task<Response> Create(string token, Recipe recipe);
        Task<Response> Edit(string token, string recipeId, Recipe recipe);
        Task<Response> Delete(string id);
        Task<Response> GetById(string id);
        Task<Response> GetAll(string token);
    }
}
