using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

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
  /// MulticastListener class
  /// </summary>
    public class MulticastListener : DiscoveryListener,IDisposable
    {
        Thread _threadListener;
        bool _disposing=false;
        /// <summary>
        /// Initializes a new instance of the <see cref="MulticastListener"/> class.
        /// </summary>
        public MulticastListener()
        {
            // Create the Socket
            Socket sock = new Socket(AddressFamily.InterNetwork,
                              SocketType.Dgram,
                              ProtocolType.Udp);

            // Set the reuse address option
            sock.SetSocketOption(SocketOptionLevel.Socket,
                                 SocketOptionName.ReuseAddress, 1);

            // Create an IPEndPoint and bind to it
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Constants.MulticastPort);
            sock.Bind(ipep);

            IPAddress multicastAddress = IPAddress.Parse(Constants.MulticastAddress);
            try
            {
              // Add membership in the multicast group
              sock.SetSocketOption(SocketOptionLevel.IP,
                                   SocketOptionName.AddMembership,
                                   new MulticastOption(multicastAddress, IPAddress.Any));

            }
            catch (SocketException sex)
            {
               
              throw new Exception("Network does not support multicast communication, check your network settings",sex);
            }
            // Create the EndPoint class
            IPEndPoint receivePoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempReceivePoint = (EndPoint)receivePoint;
            _threadListener=new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        byte[] recData = new byte[Constants.Soap.MaxMessageSize];

                        try
                        {
                            // Receive the multicast packets
                            int length = sock.ReceiveFrom(recData, 0, Constants.Soap.MaxMessageSize,
                                                          SocketFlags.None,
                                                          ref tempReceivePoint);
                            //Invio alla classe base che lo trasforma in un messaggio SOAP
                            base.ReceiveData(null, recData, 0, length);
                        }
                        catch (ThreadAbortException)
                        {
                            // Drop membership
                            sock.SetSocketOption(SocketOptionLevel.IP,
                                                SocketOptionName.DropMembership,
                                                 new MulticastOption(multicastAddress,
                                                                     IPAddress.Any));
                            // Close the socket
                            sock.Close();
                        }
                        catch (SocketException ex)
                        {
                          throw new Exception("An exception is throwed during ReceiveData from multicast socket", ex);
                        }
                    }
                    
                }));
            _threadListener.IsBackground = true;
            _threadListener.Start();
        }
        
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
          if (!_disposing && _threadListener.ThreadState == ThreadState.Running)
          {
            _disposing = true;
            _threadListener.Abort();
          }
        }

        #endregion
    }
}
