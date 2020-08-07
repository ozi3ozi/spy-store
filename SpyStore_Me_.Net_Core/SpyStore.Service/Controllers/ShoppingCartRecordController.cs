using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;

namespace SpyStore.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartRecordController : ControllerBase
    {
        private readonly IShoppingCartRepo _repo;

        public ShoppingCartRecordController(IShoppingCartRepo shoppingCartRepo)
        {
            _repo = shoppingCartRepo;
        }

        [HttpGet("{recordId}", Name = "GetShoppingCartRecord")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<CartRecordWithProductInfo> GetShoppingCartRecord(int recordId)
        {
            CartRecordWithProductInfo cartRecord = _repo.GetShoppingCartRecord(recordId);
            return cartRecord ?? (ActionResult<CartRecordWithProductInfo>)NotFound();
        }
        //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator (??)

        [HttpPost("{customerId}", Name = "AddCartRecord")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult AddShoppingCartRecord(int customerId, ShoppingCartRecord record)
        {
            if (record == null || record.CustomerId != customerId || !ModelState.IsValid)
            {
                return BadRequest();
            }
            record.DateCreated = DateTime.Now;
            record.CustomerId = customerId;
            _repo.Context.CustomerId = customerId;
            _repo.Add(record);
            //Location: http://localhost:8477/api/ShoppingCartRecord/1 (201)
            CreatedAtRouteResult createdAtRouteResult = CreatedAtRoute("GetShoppingCart",
                new { controller = "ShoppingCart", customerId = customerId }, null);
            return createdAtRouteResult;
        }

        [HttpPut("{recordId}", Name = "UpdateCartRecord")]
        public ActionResult UpdateShoppingCartRecord(int recordId, ShoppingCartRecord record)
        {
            if (record == null || record.Id != recordId || !ModelState.IsValid)
            {
                return BadRequest();
            }
            record.DateCreated = DateTime.Now;
            _repo.Context.CustomerId = record.CustomerId;
            _repo.Update(record);
            //Location: http://localhost:8477/api/ShoppingCart/0 (201)
            return CreatedAtRoute("GetShoppingCartRecord", new { controller = "ShoppingCart", recordId = record.Id }, null);
        }

        [HttpDelete("{recordId}", Name = "DeleteCartRecord")]
        //HTTP 1.1 spec allows for body in delete statement
        public IActionResult DeleteCartRecord(int recordId, ShoppingCartRecord item)
        {
            if (recordId != item.Id)
            {
                return NotFound();
            }
            _repo.Context.CustomerId = item.CustomerId;
            _repo.Delete(item);
            return NoContent();
        }
    }
}
