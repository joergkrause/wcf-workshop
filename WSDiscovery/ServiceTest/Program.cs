using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.ServiceModel.Description;
using System.Reflection;

namespace ServiceTest
{
    class Program
    {
        
        static void Main(string[] args)
        {
          try
          {
            ServiceHost host = new ServiceHost(typeof(ServiceInstance));
            host.Open();
            Console.WriteLine("Ready....."); Console.ReadLine();
            host.Close();
          }
          catch (System.Configuration.ConfigurationErrorsException ex)
          {
              Console.WriteLine("Host cannot start for a configuration problem, probably you have to copy Discovery.dll in bin/($OutputDir)/");
              throw;
          }

        }

 
    }
}
