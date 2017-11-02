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
      PasswordHasher hasher = new PasswordHasher();
      // Logon anlegen
      // User anlegen
      var u1 = new IdentityUser {
        Email = "user@users.de",
        UserName = "Ulrich User",
        PhoneNumber = "1234568",
        PasswordHash =  hasher.HashPassword("p@ssw0rd")
      };
      var r1 = new IdentityRole {
        Name = "Admin"
      };
      var r2 = new IdentityRole {
        Name = "User"
      };
      context.Users.Add(u1);
      context.Roles.Add(r1);
      context.Roles.Add(r2);
      context.SaveChanges();
      r1.Users.Add(new IdentityUserRole { RoleId = r1.Id, UserId = u1.Id });
      r2.Users.Add(new IdentityUserRole { RoleId = r2.Id, UserId = u1.Id });
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
