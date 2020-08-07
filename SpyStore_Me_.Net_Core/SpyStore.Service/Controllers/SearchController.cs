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
    public class SearchController : ControllerBase
    {
        private readonly IProductRepo _repo;

        public SearchController(IProductRepo productRepo)
        {
            _repo = productRepo;
        }

        //The Search uses one of the view models created in the data access method, and it is invoked from the route 
        ///api/search/{searchString}, like this example search:
        //http://localhost:8477/api/search/persuade%20anyone.
        [HttpGet("{searchString}", Name = "searchProducts")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public ActionResult<List<Product>> Search(string searchString)
        {
            var searchResult = _repo.Search(searchString).ToList();
            if (searchResult.Count == 0)
                return NoContent();
            return Ok(searchResult);
        }
    }
}
