using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Seminar.DataSource;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Web;

namespace Seminar.Service.Identity {
  public class UserNameValidator : UserNamePasswordValidator {
    public override void Validate(string userName, string password) {

      using (var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(ContextFactory.GetContext()))) {
        if (userManager.Find(userName, password) == null) {
          var msg = String.Format($"Unknown User {userName} or incorrect password {password}");
          throw new FaultException(msg);
          //the client actually will receive MessageSecurityException. 
          // But if I throw MessageSecurityException, the runtime will give FaultException to client without clear message.
        }
      }
    }
  }

  public class RoleAuthorizationManager : ServiceAuthorizationManager {
    protected override bool CheckAccessCore(OperationContext opContext) {

      using (var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(ContextFactory.GetContext()))) {
        var user = userManager.FindByName(opContext.ServiceSecurityContext.PrimaryIdentity.Name);
        if (user == null) {
          var msg = String.Format($"Unknown Username {user.UserName} .");
          throw new FaultException(msg);
        }
        var roleNames = userManager.GetRoles(user.Id).ToArray();
        opContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] = new GenericPrincipal(opContext.ServiceSecurityContext.PrimaryIdentity, roleNames);
        return true;
      }
    }
  }

}