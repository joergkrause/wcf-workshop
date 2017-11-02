using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHostConsole {
  class Program {
    static void Main(string[] args) {
      using (var host = new ServiceHost(typeof(Seminar.Service.UserService))) {
        Console.WriteLine("Host startet");
        host.Opened += Host_Opened;
        host.Open();
        Console.ReadLine();
      }
    }

    private static void Host_Opened(object sender, EventArgs e) {
      Console.WriteLine("Host geöffnet");
    }
  }
}
