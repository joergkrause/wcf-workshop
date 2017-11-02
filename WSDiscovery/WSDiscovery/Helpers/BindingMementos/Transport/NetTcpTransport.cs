using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Net;
using System.ServiceModel;

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
namespace Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.Transport
{
  /// <summary>
  /// NetTcpTransport
  /// </summary>
  class NetTcpTransport
  {
    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static TcpTransportBindingElement ReadXml(XmlReader reader)
    {
      TcpTransportBindingElement tran = new TcpTransportBindingElement();
      if (reader.LocalName != "TcpTransportBindingElement")
        reader.ReadStartElement("TcpTransportBindingElement");
      do
      {
        if ((!reader.IsStartElement() || reader.IsEmptyElement) && reader.LocalName == "TcpTransportBindingElement")
          break;
        else
        {
          if (reader.IsStartElement() && reader.LocalName == "ChannelInitializationTimeout")
          {
            tran.ChannelInitializationTimeout = TimeSpan.FromMilliseconds(reader.ReadContentAsLong());
          }
          else if (reader.IsStartElement() && reader.LocalName == "ConnectionBufferSize")
          {
            tran.ConnectionBufferSize = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "ConnectionPoolSettings")
          {
            //TODO:Read e Write
            while (true)
            {
              if ((!reader.IsStartElement() || reader.IsEmptyElement) && reader.LocalName == "ConnectionPoolSettings")
                break;
              else if (!reader.IsStartElement() && reader.LocalName == "GroupName")
                tran.ConnectionPoolSettings.GroupName = reader.ReadContentAsString();
              else if (!reader.IsStartElement() && reader.LocalName == "IdleTimeout")
              {
                long timespan = reader.ReadContentAsLong();
                if (timespan > 0)
                {
                  tran.ConnectionPoolSettings.IdleTimeout = TimeSpan.FromMilliseconds(timespan);
                }
              }
              else if (!reader.IsStartElement() && reader.LocalName == "LeaseTimeout")
              {
                long timespan = reader.ReadContentAsLong();
                if (timespan > 0)
                {
                  tran.ConnectionPoolSettings.LeaseTimeout = TimeSpan.FromMilliseconds(timespan);
                }
              }
              else if (!reader.IsStartElement() && reader.LocalName == "MaxOutboundConnectionsPerEndpoint")
                tran.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint = reader.ReadContentAsInt();
              else
                reader.Read();
            }
          }
          else if (reader.IsStartElement() && reader.LocalName == "HostNameComparisonMode")
          {
            string val = reader.ReadContentAsString();
            if (val == "Exact")
              tran.HostNameComparisonMode = HostNameComparisonMode.Exact;
            else if (val == "Exact")
              tran.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            else if (val == "Exact")
              tran.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;

          }
          else if (reader.IsStartElement() && reader.LocalName == "ListenBacklog")
          {
            tran.ListenBacklog = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "ManualAddressing")
          {
            tran.ManualAddressing = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxBufferPoolSize")
          {
            tran.MaxBufferPoolSize = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxBufferSize")
          {
            tran.MaxBufferSize = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxOutputDelay")
          {
            tran.MaxOutputDelay = TimeSpan.FromMilliseconds(reader.ReadContentAsInt());
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxPendingAccepts")
          {
            tran.MaxPendingAccepts = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxPendingConnections")
          {
            tran.MaxPendingConnections = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxReceivedMessageSize")
          {
            tran.MaxReceivedMessageSize = reader.ReadContentAsLong();
          }
          else if (reader.IsStartElement() && reader.LocalName == "PortSharingEnabled")
          {
            tran.PortSharingEnabled = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "TeredoEnabled")
          {
            tran.TeredoEnabled = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "TransferMode")
          {
            string val = reader.ReadContentAsString();
            if (val == "Buffered")
              tran.TransferMode = TransferMode.Buffered;
            else if (val == "Streamed")
              tran.TransferMode = TransferMode.Streamed;
            else if (val == "StreamedRequest")
              tran.TransferMode = TransferMode.StreamedRequest;
            else if (val == "StreamedResponse")
              tran.TransferMode = TransferMode.StreamedResponse;
          }
        }

      }
      while (reader.Read());
      return tran;
    }
    /// <summary>
    /// Gets the authentication scheme.
    /// </summary>
    /// <param name="scheme">The scheme.</param>
    /// <returns></returns>
    private static AuthenticationSchemes GetAuthenticationScheme(string scheme)
    {
      if (scheme == "Anonymous")
        return AuthenticationSchemes.Anonymous;
      if (scheme == "Basic")
        return AuthenticationSchemes.Basic;
      if (scheme == "Digest")
        return AuthenticationSchemes.Digest;
      if (scheme == "IntegratedWindowsAuthentication")
        return AuthenticationSchemes.IntegratedWindowsAuthentication;
      if (scheme == "Negotiate")
        return AuthenticationSchemes.Negotiate;

      if (scheme == "Ntlm")
        return AuthenticationSchemes.Ntlm;
      return AuthenticationSchemes.None;
    }
    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="tran">The tran.</param>
    /// <returns></returns>
    public static string WriteXml(TcpTransportBindingElement tran)
    {
      StringBuilder sb = new StringBuilder();
      //per comprimere l'xml controllò i valori di defualt (el) ed elimino le proprietà inutili
      TcpTransportBindingElement el = new TcpTransportBindingElement();
      using (XmlTextWriter w = new XmlTextWriter(new StringWriter(sb)))
      {
        w.WriteStartElement("TcpTransportBindingElement");
        if (tran.ChannelInitializationTimeout != el.ChannelInitializationTimeout)
          w.WriteElementString("ChannelInitializationTimeout", tran.ChannelInitializationTimeout.Milliseconds.ToString());
        if (tran.ConnectionBufferSize != el.ConnectionBufferSize)
          w.WriteElementString("ConnectionBufferSize", tran.ConnectionBufferSize.ToString());
        if (tran.ConnectionPoolSettings.GroupName != el.ConnectionPoolSettings.GroupName)
        {
          w.WriteStartElement("ConnectionPoolSettings");
          w.WriteElementString("ConnectionBufferSize", tran.ConnectionPoolSettings.GroupName);
          if (tran.ConnectionPoolSettings.IdleTimeout != el.ConnectionPoolSettings.IdleTimeout)
            w.WriteElementString("IdleTimeout", tran.ConnectionPoolSettings.IdleTimeout.TotalMilliseconds.ToString());
          if (tran.ConnectionPoolSettings.LeaseTimeout != el.ConnectionPoolSettings.LeaseTimeout)
            w.WriteElementString("LeaseTimeout", tran.ConnectionPoolSettings.LeaseTimeout.TotalMilliseconds.ToString());
          if (tran.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint != el.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint)
            w.WriteElementString("IdleTimeout", tran.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint.ToString());
          w.WriteEndElement();
        }
        if (tran.HostNameComparisonMode != el.HostNameComparisonMode)
          w.WriteElementString("HostNameComparisonMode", tran.HostNameComparisonMode.ToString());
        if (tran.ListenBacklog != el.ListenBacklog)
          w.WriteElementString("ListenBacklog", tran.ListenBacklog.ToString());
        if (tran.ManualAddressing != el.ManualAddressing)
          w.WriteElementString("ManualAddressing", tran.ManualAddressing.ToString());
        if (tran.MaxBufferPoolSize != el.MaxBufferPoolSize)
          w.WriteElementString("MaxBufferPoolSize", tran.MaxBufferPoolSize.ToString());
        if (tran.MaxBufferSize != el.MaxBufferSize)
          w.WriteElementString("MaxBufferSize", tran.MaxBufferSize.ToString());
        if (tran.MaxOutputDelay != el.MaxOutputDelay)
          w.WriteElementString("MaxOutputDelay", tran.MaxOutputDelay.TotalMilliseconds.ToString());
        if (tran.MaxPendingAccepts != el.MaxPendingAccepts)
          w.WriteElementString("MaxPendingAccepts", tran.MaxPendingAccepts.ToString());
        if (tran.MaxPendingConnections != el.MaxPendingConnections)
          w.WriteElementString("MaxPendingConnections", tran.MaxPendingConnections.ToString());
        if (tran.MaxReceivedMessageSize != el.MaxReceivedMessageSize)
          w.WriteElementString("MaxReceivedMessageSize", tran.MaxReceivedMessageSize.ToString());
        if (tran.PortSharingEnabled != el.PortSharingEnabled)
          w.WriteElementString("PortSharingEnabled", tran.PortSharingEnabled.ToString());
        if (tran.TeredoEnabled != el.TeredoEnabled)
          w.WriteElementString("TeredoEnabled", tran.TeredoEnabled.ToString());
        if (tran.TransferMode != el.TransferMode)
          w.WriteElementString("TransferMode", tran.TransferMode.ToString());

        w.WriteEndElement();
      }
      return sb.ToString();
    }
  }
}
