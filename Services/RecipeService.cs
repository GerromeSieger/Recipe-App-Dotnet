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
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static RecipeApp.ServicesInterface.IRecipeService;

namespace RecipeApp.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly DataContext dataContext;
        public RecipeService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<Response> Create(string token, Recipe recipe)

        {
            try
            { 
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;
                recipe.UserId = userId;

                // Get new recipe id
                var recipeId = recipe.Id;

                // Update ingredient records with recipe id
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.RecipeId = recipeId;
                }

                await dataContext.Recipes.AddAsync(recipe);
                await dataContext.SaveChangesAsync();


                return new Response
                {
                    result = true,
                    message = "Recipe created successfully",
                    data =  recipe
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    result = false,
                    message = "Error creating recipe: " + ex.Message
                };
            }
        }

        public async Task<Response> Delete(string id)
        {
            try
            {
                var recipe = await dataContext.Recipes.FindAsync(id);
                if (recipe == null)
                    return new Response { message = "Recipe not found" };

                dataContext.Recipes.Remove(recipe);
                await dataContext.SaveChangesAsync();

                return new Response
                {
                    result = true,
                    message = "Recipe deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    result = false,
                    message = "Error deleting user: " + ex.Message
                };
            }
        }

        public async Task<Response> Edit(string token, string recipeId, Recipe recipe)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;

                var existingRecipe = await dataContext.Recipes.FindAsync(recipeId);

                if (existingRecipe == null)
                    return new Response
                    {
                        result = false,
                        message = "Recipe not found"
                    };

                if (existingRecipe.UserId != userId)
                    return new Response
                    {
                        result = false,
                        message = "Not authorized to edit this recipe"
                    };
                if (!string.IsNullOrEmpty(recipe.Name))
                    existingRecipe.Name = recipe.Name;

                if (!string.IsNullOrEmpty(recipe.Description))
                    existingRecipe.Description = recipe.Description;


                if (recipe.Ingredients != null)
                {

                    // Remove old ingredients
                    dataContext.Ingredients.RemoveRange(
                      existingRecipe.Ingredients);

                    // Add new ingredients 
                    existingRecipe.Ingredients = recipe.Ingredients;
                }

                    await dataContext.SaveChangesAsync();

                return new Response
                {
                    result = true,
                    message = "Recipe updated successfully",
                    data = existingRecipe
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    result = false,
                    message = "Error updating recipe: " + ex.Message
                };
            }
        }
        public async Task<Response> GetAll(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken.Claims.First(claim => claim.Type == "Id").Value;

            var userRecipes = dataContext.Recipes.Where(r => r.UserId == userId).ToList();

            return new Response
            {
                result = true,
                data = userRecipes
            };
        }

        //    public async Task<Response> GetAll()
        //    {
        //        var recipes = await dataContext.Recipes.ToListAsync();
        //       return new Response
        //      {
        //            result = true,
        //            data = recipes
        //        };
        //    }

        public async Task<Response> GetById(string id)
        {
            var recipe = await dataContext.Recipes.FindAsync(id);
            if (recipe == null)
                return new Response { message = "Recipe not found" };

            return new Response
            {
                result = true,
                data = recipe
            };
        }
    }
}
