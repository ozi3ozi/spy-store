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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepo _repo;
        public CustomerController(ICustomerRepo repo)
        {
            _repo = repo;
        }

        [HttpGet(Name = "GetAllCustomers")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IList<Customer>> Get()
        {
            IEnumerable<Customer> customers = _repo.GetAll().ToList();
            return Ok(customers);
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        [Produces("Application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Customer> Get(int id)
        {
            Customer customer = _repo.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
    }
}
