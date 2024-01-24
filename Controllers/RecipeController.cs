using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;
using RecipeApp.Models;
using RecipeApp.ServicesInterface;
using RecipeApp.Data;
using RecipeApp.Services;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using static RecipeApp.ServicesInterface.IRecipeService;
using System.Collections.Generic;

namespace RecipeApp.Controllers
{
    [Route("recipe")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService Service;
        private readonly DataContext dataContext;

        public RecipeController(DataContext dataContext, IRecipeService service)
        {
            this.dataContext = dataContext;
            this.Service = service;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody]Recipe recipe)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var tok = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.ReadToken(tok));

                var response = await Service.Create(token, recipe);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();

        }

        [HttpPut]
        [Route("update/{recipeId}")]
        public async Task<IActionResult> Update(string recipeId, [FromBody] Recipe recipe)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var tok = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.ReadToken(tok));
                var response = await Service.Edit(token, recipeId, recipe);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();

        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await Service.Delete(id);
            return Ok(response);
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var tokenString = handler.WriteToken(handler.ReadToken(token));

                var response = await Service.GetAll(tokenString);

                return response.result ? Ok(response) : BadRequest(response);
            }

            return Unauthorized();

        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await Service.GetById(id);
            return Ok(response);
        }
    }
}
