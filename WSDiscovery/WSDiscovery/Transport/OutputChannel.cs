using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Messages;

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
  /// OutputChannel class
  /// </summary>
    public class OutputChannel
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="to"></param>
        /// <returns>true if sent</returns> 
        public static bool Send(SoapEnvelope envelope, Uri to)
        {
            switch (to.Scheme.ToLower())
            { 
                case "soap.udp":
                    UdpOutputChannel.SendUnicast(envelope, to.Host, to.Port);
                    return true;
                case "http":
                    //TODO:
                    throw new NotImplementedException();
                default:
                    return false;

                
            }
        }
    }
}
