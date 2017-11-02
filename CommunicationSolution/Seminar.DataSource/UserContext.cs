using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Seminar.DataSource {
  public class UserContext : IdentityDbContext {

    public UserContext() : base(nameof(UserContext)) {

    }

    public DbSet<Customer> Customers { get; set; }

    

  }
}
