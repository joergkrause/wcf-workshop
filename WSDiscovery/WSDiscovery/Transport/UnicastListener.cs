using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


//*****************************************************************************
//    Description.....WS-Discovey for WCF
//                                
//    Author..........Claudio Masieri, claudio@claudiomasieri.it
//    Copyright © 2008 ing.Masieri Claudio. (see included license.rtf file)    
//                        
//    Date Created:    06/06/06
//
//    Date        Modified By     Description
//-----------------------------------------------------------------------------
//    01/10/08    Claudio Masieri     First Release
//*****************************************************************************
namespace Masieri.ServiceModel.WSDiscovery.Transport
{
  /// <summary>
  /// UnicastListener class
  /// </summary>
  public class UnicastListener : DiscoveryListener, IDisposable
  {
    private readonly UdpClient _client;
    private readonly IPAddress _address;
    private bool _disposed = false;
    private readonly AsyncCallback _runner;
    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>The port.</value>
    public int Port { get; set; }
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>The address.</value>
    public string Address { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnicastListener"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="port">The port.</param>
    public UnicastListener(IPAddress address, int port)
    {
      Address = address.ToString();
      Port = port;
      _client = new UdpClient(port);
      _address = address;
      _runner = delegate(IAsyncResult ar)
      {
        try
        {
          EndReceive(ar);
        }
        catch (Exception failed)
        {
          DiscoveryLogger.Warn("read failed. ending connection: " + _address, failed);
        }
      };
      _client.BeginReceive(_runner, null);
    }

    private UdpClient Client
    {
      get { return _client; }
    }

    private void EndReceive(IAsyncResult result)
    {
      IPEndPoint endpoint = null;
      byte[] packet = _client.EndReceive(result, ref endpoint);

      base.ReceiveData(endpoint, packet, 0, packet.Length);
      _client.BeginReceive(_runner, null);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (!_disposed)
      {
        _client.Close();
        _disposed = true;
      }
    }
    #region static
    /// <summary>
    /// Gets the UDP unicast listener.
    /// </summary>
    /// <returns></returns>
    public static UnicastListener GetUdpUnicastListener()
    {
      UnicastListener receiver;
      string ipaddress = string.Empty;
      IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
      foreach (IPAddress address in addresses)
      {
        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
        {
          DiscoveryLogger.Info("Using the address  {0} for the unicast response", address.ToString());
          ipaddress = address.ToString();
        }
      }
      //try to use a random port among the dynamic port range (49152-65535) http://www.iana.org/assignments/port-numbers
      int port = new System.Random().Next(65535 - 49152) + 49152;
      for (; ; )
      {
        try
        {
          //Lookin for an opened door
          receiver = new UnicastListener(IPAddress.Parse(ipaddress), port++);
          DiscoveryLogger.Info("Using the address  {0}:{1} for the unicast response", ipaddress, port);
          return receiver;

        }
        catch
        {
          //go ahead
          DiscoveryLogger.Info("The address for unicast response {0}:{1} is unavailable", ipaddress, port);
        }
      }
    }
    #endregion
  }

}
