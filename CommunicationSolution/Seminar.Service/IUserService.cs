using Seminar.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Seminar.Service {

  [ServiceContract(Namespace = "http://svc.meinefirma.de/userService")]
  public interface IUserService {

    [OperationContract(Name = "GetAll", ReplyAction = "AllResult")]
    [FaultContract(typeof(UserFault))]
    [WebGet()]
    Customer[] GetAllUsers();

    [OperationContract(Name = "Count", ReplyAction = "CountResult")]
    [WebInvoke(Method = "POST")]
    int CountUsers();

  }



}
