using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DataSource {
  public class UserContext : IdentityDbContext {

    public UserContext() : base("UserContext") {

    }

    public DbSet<Customer> Customers { get; set; }

    

  }
}
