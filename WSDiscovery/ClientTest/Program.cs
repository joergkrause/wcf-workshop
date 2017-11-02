using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Masieri.ServiceModel.WSDiscovery;
using Masieri.ServiceModel.WSDiscovery.Transport;
using Masieri.ServiceModel.WSDiscovery.Messages;
using System.IO;
using System.Xml;
using System.ServiceModel.Channels;
using System.Threading;
using System.ServiceModel.Description;
using Masieri.ServiceModel.WSDiscovery.Diagnostics;
using Masieri.ServiceModel.WSDiscovery.Client;

namespace ClientTest
{
  [ServiceContract()]
  public interface IServiceSample
  {
    [OperationContract()]
    string GetString(string parToReturn);
  }
  [ServiceContract()]
  public interface IServiceSample2
  {
    [OperationContract()]
    int GetNumber(int parToReturn);
  }
  class Program
  {
    static void Main(string[] args)
    {
      try
      {

        //TestCatchAll();
        TestSample1();

      }
      catch (NoDiscoveredEndpointException)
      {
        Console.WriteLine("NoDiscoveredEndpointException");
        Console.ReadLine();
        return;
      }
      Console.ReadLine();

    }
    private static void TestCatchAll()
    {
      DiagnosticHelper helper = new DiagnosticHelper();
      //Wait 5 sec
      Thread.Sleep(5000);
      List<ClientMemento> list= helper.MementoList;
    }
    private static void TestSample1()
    {
      DiscoveryClient<IServiceSample> proxy = new DiscoveryClient<IServiceSample>("http://myscope.tempuri.org/");
      for (int i = 0; i < 10; i++)
      {
        //Add custom header in the request to test it
        using (DiscoveryOperationContextScope<IServiceSample> os = new DiscoveryOperationContextScope<IServiceSample>(proxy))
        {
          os.OutgoingHeaders.Add(MessageHeader.CreateHeader("SampleHeader", "", "http://www.masieri.com/ciao"));
          Console.WriteLine("Requested string " + proxy.Channel.GetString("qqq"));
        }
        Console.WriteLine("Waiting 30 seconds to retry");
        Thread.Sleep(30000);
      }
    }

  }
}
