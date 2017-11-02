using Seminar.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seminar.Service {
  internal static class ContextFactory {

    internal static UserContext GetContext() {
      if (HttpContext.Current != null) {
        UserContext ctx;
        var id = $"__XX__{HttpContext.Current.GetHashCode().ToString()}";
        if (HttpContext.Current.Items.Contains(id)) {
          ctx = HttpContext.Current.Items[id] as UserContext;
        } else {
          ctx = new UserContext();
          HttpContext.Current.Items.Add(id, ctx);
        }
        return ctx;
      } else {
        // kein HTTP?
        throw new NotSupportedException();        
      }
    }

  }
}