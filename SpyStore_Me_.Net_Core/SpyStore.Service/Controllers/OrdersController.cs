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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepo _repo;
        public OrdersController(IOrderRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("{customerId}", Name = ("GetOrderHistory")]
        [Produces("Appication/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetOrderHistory(int customerId)
        {
            _repo.Context.CustomerId = customerId;
            IEnumerable<Order> orders = _repo.GetOrderHistoy().ToList();
            return orders == null
                ? (IActionResult)NotFound()
                : new ObjectResult(orders);
        }
        //Can also be done this way
        //public ActionResult<IList<Order>> GetOrderHistory(int customerId)
        //{
        //    _repo.Context.CustomerId = customerId;
        //    IEnumerable<Order> orders = _repo.GetOrderHistoy().ToList();
        //    if (orders == null)
        //        return NotFound();
        //    return Ok(orders);
        //}
    }
}
