using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seminar.DataSource;
using System.Linq;
using System.Data.Entity;

namespace Seminar.Tests.DatabaseClient {
  [TestClass]
  public class UnitTest1 {

    [TestInitialize]
    public void SetupDatabase() {
      Database.SetInitializer(new InitDatabase());

      using (var context = new UserContext()) {
        var models = context.Customers.ToList();
      }

    }

    [TestMethod]
    public void TestDbForUsers() {

      using (var context = new UserContext()) {
        var users = context.Customers.ToList();
        Assert.IsNotNull(users);
        var count = users.Count();
        Assert.AreEqual(3, count);
      }

    }


    [TestMethod]
    public void IdentityTest() {

      using (var context = new UserContext()) {
        var count = context.Roles.Count();
        Assert.AreEqual(2, count);
      }

    }

  }
}
