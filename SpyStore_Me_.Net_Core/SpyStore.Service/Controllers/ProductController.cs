using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Models.Entities;

namespace SpyStore.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _repo;
        public ProductController(IProductRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("featured", Name = "GetFeaturedProducts")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Product>> GetFeatured()
            => Ok(_repo.GetFeaturedWithCategoryName().ToList());

        [HttpGet("{id}", Name = "GetProduct")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Product> Get(int id)
        {
            Product product = _repo.GetOneWithCategoryName(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
