using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seminar.DataSource;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers {
  [Route("api/[controller]")]
  public class CustomerController : Controller {

    List<Customer> customers;

    public CustomerController() {
      customers = new List<Customer>();
      var user1 = new Customer {
        Id = 1,
        UserName = "Anton Arbeiter",
        Password = "p@ssw0rd",
        Room = "A.100"
      };
      var user2 = new Customer {
        Id = 2,
        UserName = "Berta Büro",
        Password = "p@ssw0rd",
        Room = "A.101"
      };
      var user3 = new Customer {
        Id = 3,
        UserName = "Cäsar Chef",
        Password = "p@ssw0rd",
        Room = "B.50"
      };
      // Speichern
      customers.Add(user1);
      customers.Add(user2);
      customers.Add(user3);
    }

    // GET: api/values
    [HttpGet]
    public IEnumerable<Customer> Get() {
      return customers;
    }

    [HttpGet]
    [Route("special")] //api/customer/special
    public IActionResult MeineTolleFunktion() {
      return NoContent(); // 204
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public IActionResult Get(int id) {
      var customer =customers.SingleOrDefault(c => c.Id == id);
      if (customer == null) {
        return NotFound();
      } else {
        return Ok(customer);
      }
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody]Customer value) {
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]Customer value) {

    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id) {
      var hasCustomer = customers.Any(c => c.Id == id);
      if (hasCustomer) {
        // OK
        return Ok();
      } else {
        // Bad Request
        return BadRequest();
      }
    }
  }
}
