using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seminar.Service {

  [DataContract]
  public class UserFault {

    [DataMember]
    public int Count { get; set; }


  }
}