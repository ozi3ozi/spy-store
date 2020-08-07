using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Models.ViewModels;

namespace SpyStore.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderRepo _repo;
        public OrderDetailsController(IOrderRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("{orderId}", Name = "GetOrdertDetails")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetOrderWithDetailsForCustomer(int orderId)
        {
            OrderWithDetailsAndProductInfo orderWithDetails = _repo.GetOneWithDetails(orderId);
            return orderWithDetails == null
                ? (IActionResult) NotFound()
                : new ObjectResult(orderWithDetails);
        }
        //can also be done this way:
        //public ActionResult<OrderWithDetailsAndProductInfo> GetOrderWithDetailsForCustomer(int orderId)
        //{
        //    OrderWithDetailsAndProductInfo orderWithDetails = _repo.GetOneWithDetails(orderId);
        //    if (orderWithDetails == null)
        //        return NotFound();
        //    return Ok(orderWithDetails);
        //}
    }
}
