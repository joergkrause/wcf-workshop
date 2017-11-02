using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.IO;
using System.Xml;
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
  /// asbstract DiscoveryListener
  /// </summary>
  public abstract class DiscoveryListener
  {
    private MessageTraceSource _messageTraceSource;
    /// <summary>
    /// delegate for MessageArrived Event
    /// </summary>
    public delegate void MessageArrivedEventHandler(object sender, Message message);
    /// <summary>
    /// Occurs when [message arrived].
    /// </summary>
    public event MessageArrivedEventHandler MessageArrived;
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveryListener"/> class.
    /// </summary>
    public DiscoveryListener()
    {
      _messageTraceSource = new MessageTraceSource();

    }
    /// <summary>
    /// Receives the data.
    /// </summary>
    /// <param name="endPoint">The end point.</param>
    /// <param name="data">The data.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    protected void ReceiveData(System.Net.EndPoint endPoint, byte[] data, int offset, int length)
    {
      Message msg;
      string envelope = "";
      try
      {
        envelope = Encoding.ASCII.GetString(data, offset, length);
        _messageTraceSource.WriteMessageAtTrasportLevel(envelope, "Masieri.ServiceModel.WSDiscovery.Transport.ReceiveData");
        StringReader sr = new StringReader(envelope);
        
        msg = Message.CreateMessage(new XmlTextReader(sr), Constants.Soap.MaxHeaderSize, Constants.SOAPMessageVersion);
      }
      catch (Exception)
      {
        //Log Messaggio non conforme
        _messageTraceSource.WriteMalformedMessage(envelope, "Masieri.ServiceModel.WSDiscovery.Transport.ReceiveData");
        msg = null;

      }
      if (msg != null)
      {
        OnMessageArrived(msg);
      }
    }
    /// <summary>
    /// Called when [message arrived].
    /// </summary>
    /// <param name="message">The message.</param>
    void OnMessageArrived(Message message)
    {
      if (MessageArrived != null)
        MessageArrived(this, message);
    }
  }
}
