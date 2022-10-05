using System.Collections.Generic;
using HotelBooking.Core;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly IRepository<Customer> repository;

        public CustomersController(IRepository<Customer> repos)
        {
            repository = repos;
        }

        // GET: customers
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return repository.GetAll();
        }
        //GETById
        [HttpGet("{id}")]
        public Customer GetById(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                throw new Exception("Customer not Found");
            }
            return item;
        }

    }
}
