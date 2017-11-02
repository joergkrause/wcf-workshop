using Seminar.Service.ConsoleClient.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.Service.ConsoleClient {
  class Program {
    static void Main(string[] args) {
      var client = new UserServiceClient();
      client.ClientCredentials.UserName.UserName = "UlrichUser";
      client.ClientCredentials.UserName.Password = "p@ssw0rd";
      ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, spe) => {
        return true;
      };
      try {
        var users = client.GetAll().ToList();
        users.ForEach(u => Console.WriteLine($"User = {u.N}"));
      } catch (FaultException<UserFault> fault) {
        Console.WriteLine($"{fault.Action} sagt {fault.Reason} mit cnt = {fault.Detail.Count}");
      }

      Console.ReadLine();
      
    }
  }
}
