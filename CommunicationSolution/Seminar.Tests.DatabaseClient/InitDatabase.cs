using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Seminar.DataSource;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.Tests.DatabaseClient {
  internal class InitDatabase : DropCreateDatabaseAlways<UserContext> {

    protected override void Seed(UserContext context) {
      base.Seed(context);
      var hasher = new PasswordHasher();
      // Login
      var u1 = new IdentityUser {
        Email = "ulrich@user.de",
        UserName = "UlrichUser",
        PasswordHash = hasher.HashPassword("p@ssw0rd")
      };
      context.Users.Add(u1);
      context.SaveChanges();

      // User anlegen
      var user1 = new Customer {
        UserName = "Anton Arbeiter",
        Password = "p@ssw0rd",
        Room = "A.100"
      };
      var user2 = new Customer {
        UserName = "Berta Büro",
        Password = "p@ssw0rd",
        Room = "A.101"
      };
      var user3 = new Customer {
        UserName = "Cäsar Chef",
        Password = "p@ssw0rd",
        Room = "B.50"
      };
      // Speichern
      context.Customers.Add(user1);
      context.Customers.Add(user2);
      context.Customers.Add(user3);
      context.SaveChanges();
    }

  }
}
