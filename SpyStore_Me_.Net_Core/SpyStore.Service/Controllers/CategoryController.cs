using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Models.Entities;

namespace SpyStore.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _repo;
        public CategoryController(ICategoryRepo repo)
        {
            _repo = repo;
        }

        [HttpGet(Name = "GetAllCategories")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Category>> Get()
        {
            IEnumerable<Category> categories = _repo.GetAll().ToList();
            return Ok(categories);
        }

        [HttpGet("{id}", Name = "GetCategory")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Category> Get(int id)
        {
            Category item = _repo.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        //The ProductRepo is injected in with method injection, which requires the [FromServices] attribute. 
        [HttpGet("{categoryId}/products", Name = "GetCategoryProducts")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Product>> GetProductsForCategory([FromServices] IProductRepo productRepo, int categoryId)
            => productRepo.GetProductsForCategory(categoryId).ToList();
    }
}
