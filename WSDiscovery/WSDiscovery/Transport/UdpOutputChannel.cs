using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Messages;
using System.Net.Sockets;
using Masieri.ServiceModel.WSDiscovery.Diagnostics;

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
  /// UdpOutputChannel
  /// </summary>
  class UdpOutputChannel
  {
    private static MessageTraceSource _traceSource= new MessageTraceSource();
    private readonly UdpClient _client;
    /// <summary>
    /// Initializes a new instance of the <see cref="UdpOutputChannel"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="port">The port.</param>
    private UdpOutputChannel(string address, int port)
    {
      _client = new UdpClient(address, port);
    }
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    public void Dispose()
    {
      _client.Close();
    }

    /// <summary>
    /// Writes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="msWaitIgnored">The ms wait ignored.</param>
    /// <returns></returns>
    public bool Write(byte[] data, int offset, int length, int msWaitIgnored)
    {
      if (offset == 0)
      {
        _client.Send(data, length);
      }
      else
      {
        byte[] bytes = new byte[length];
        System.Array.Copy(data, offset, bytes, 0, length);
        _client.Send(bytes, length);
      }
      return true;
    }
    /// <summary>
    /// Sends the multicast.
    /// </summary>
    /// <param name="envelope">The envelope.</param>
    public static void SendMulticast(SoapEnvelope envelope)
    {
      _traceSource.WriteMessageAtTrasportLevel(envelope.ToString(), "Masieri.ServiceModel.WSDiscovery.Transport.UdpOutputChannel+SendMulticast");
      byte[] bytesToSend = Encoding.ASCII.GetBytes(envelope.ToString());
      UdpOutputChannel source = new UdpOutputChannel(Constants.MulticastAddress, Constants.MulticastPort);
      source.Write(bytesToSend, 0, bytesToSend.Length, 0);
      source.Dispose();
    }
    /// <summary>
    /// Sends the unicast.
    /// </summary>
    /// <param name="envelope">The envelope.</param>
    /// <param name="ipAddress">The ip address.</param>
    /// <param name="port">The port.</param>
    public static void SendUnicast(SoapEnvelope envelope, string ipAddress, int port)
    {
      _traceSource.WriteMessageAtTrasportLevel(envelope.ToString(), "Masieri.ServiceModel.WSDiscovery.Transport.UdpOutputChannel+SendUnicast");
      byte[] bytesToSend = Encoding.ASCII.GetBytes(envelope.ToString());
      UdpOutputChannel source = new UdpOutputChannel(ipAddress, port);
      source.Write(bytesToSend, 0, bytesToSend.Length, 0);
      source.Dispose();
    }


  }
}
