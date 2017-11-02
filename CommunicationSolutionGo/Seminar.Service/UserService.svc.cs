using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Seminar.DataSource;
using System.Security.Permissions;

namespace Seminar.Service {

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  //[PrincipalPermission(SecurityAction.Demand, Role ="User")]
  public class UserService : IUserService {

    private int countUserCache = 0;

    public UserContext Context {
      get {
        return ContextFactory.GetContext();
      }
    }

    public Customer[] GetAllUsers() {
      var cnt = Context.Customers.Count();
      if (cnt >= 20) {
        var fault = new UserFault { Count = cnt };
        throw new FaultException<UserFault>(fault, new FaultReason("Zu viel!"));
      }
      var models = Context.Customers.ToArray();
      this.countUserCache = models.Count();
      return models;
    }

    public int CountUsers() {
      //PrincipalPermission p = new PrincipalPermission(null, "Admin");
      //p.Demand();
      if (this.countUserCache == 0) {
        var models = GetAllUsers();
        return models.Count();
      } else {
        return this.countUserCache;
      }
    }


  }
}
