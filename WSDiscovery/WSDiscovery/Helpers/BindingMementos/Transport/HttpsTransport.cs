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
  /// HttpsTransport
  /// </summary>
  class HttpsTransport
  {
    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static HttpsTransportBindingElement ReadXml(XmlReader reader)
    {
      HttpsTransportBindingElement tran = new HttpsTransportBindingElement();
      if (reader.LocalName != "HttpsTransportBindingElement")
        reader.ReadStartElement("HttpsTransportBindingElement");
      do
      {
        if ((!reader.IsStartElement() || reader.IsEmptyElement) && reader.LocalName == "HttpsTransportBindingElement")
          break;
        else
        {
          if (reader.IsStartElement() && reader.LocalName == "AllowCookies")
          {
            tran.AllowCookies = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "AuthenticationScheme")
          {
            tran.AuthenticationScheme = GetAuthenticationScheme(reader.ReadContentAsString());
          }
          else if (reader.IsStartElement() && reader.LocalName == "BypassProxyOnLocal")
          {
            tran.BypassProxyOnLocal = reader.ReadContentAsBoolean();
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
          else if (reader.IsStartElement() && reader.LocalName == "KeepAliveEnabled")
          {
            tran.KeepAliveEnabled = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "ManualAddressing")
          {
            tran.ManualAddressing = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxBufferSize")
          {
            tran.MaxBufferSize = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxReceivedMessageSize")
          {
            tran.MaxReceivedMessageSize = reader.ReadContentAsLong();
          }
          else if (reader.IsStartElement() && reader.LocalName == "ProxyAddress")
          {
            string proxyAdd = reader.ReadContentAsString();
            tran.ProxyAddress = (string.IsNullOrEmpty(proxyAdd)) ? null : new Uri(proxyAdd);
          }
          else if (reader.IsStartElement() && reader.LocalName == "ProxyAuthenticationScheme")
          {
            string scheme = reader.ReadContentAsString();
            tran.ProxyAuthenticationScheme = GetAuthenticationScheme(scheme);
          }
          else if (reader.IsStartElement() && reader.LocalName == "Realm")
          {
            tran.Realm = reader.ReadContentAsString();
          }
          else if (reader.IsStartElement() && reader.LocalName == "RequireClientCertificate")
          {
            tran.RequireClientCertificate = reader.ReadContentAsBoolean();
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
          else if (reader.IsStartElement() && reader.LocalName == "UnsafeConnectionNtlmAuthentication")
          {
            tran.UnsafeConnectionNtlmAuthentication = reader.ReadContentAsBoolean();
          }
          else if (reader.IsStartElement() && reader.LocalName == "UseDefaultWebProxy")
          {
            tran.UseDefaultWebProxy = reader.ReadContentAsBoolean();
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
    public static string WriteXml(HttpsTransportBindingElement tran)
    {
      StringBuilder sb = new StringBuilder();
      //per comprimere l'xml controllò i valori di defualt (el) ed elimino le proprietà inutili
      HttpsTransportBindingElement el = new HttpsTransportBindingElement();
      using (XmlTextWriter w = new XmlTextWriter(new StringWriter(sb)))
      {
        w.WriteStartElement("HttpTransportBindingElement");
        if (tran.AllowCookies != el.AllowCookies)
          w.WriteElementString("AllowCookies", tran.AllowCookies.ToString());
        if (tran.AuthenticationScheme != el.AuthenticationScheme)
          w.WriteElementString("AuthenticationScheme", tran.AuthenticationScheme.ToString());
        if (tran.BypassProxyOnLocal != el.BypassProxyOnLocal)
          w.WriteElementString("BypassProxyOnLocal", tran.BypassProxyOnLocal.ToString());
        if (tran.HostNameComparisonMode != el.HostNameComparisonMode)
          w.WriteElementString("HostNameComparisonMode", tran.HostNameComparisonMode.ToString());
        if (tran.KeepAliveEnabled != el.KeepAliveEnabled)
          w.WriteElementString("KeepAliveEnabled", tran.KeepAliveEnabled.ToString());
        if (tran.ManualAddressing != el.ManualAddressing)
          w.WriteElementString("ManualAddressing", tran.ManualAddressing.ToString());
        if (tran.MaxBufferPoolSize != el.MaxBufferPoolSize)
          w.WriteElementString("MaxBufferPoolSize", tran.MaxBufferPoolSize.ToString());
        if (tran.MaxBufferSize != el.MaxBufferSize)
          w.WriteElementString("MaxBufferSize", tran.MaxBufferSize.ToString());
        if (tran.MaxReceivedMessageSize != el.MaxReceivedMessageSize)
          w.WriteElementString("MaxReceivedMessageSize", tran.MaxReceivedMessageSize.ToString());
        if (tran.ProxyAddress != null && tran.ProxyAddress != el.ProxyAddress)
          w.WriteElementString("ProxyAddress", tran.ProxyAddress.ToString());
        if (tran.ProxyAuthenticationScheme != el.ProxyAuthenticationScheme)
          w.WriteElementString("ProxyAuthenticationScheme", tran.ProxyAuthenticationScheme.ToString());
        if (tran.Realm != el.Realm)
          w.WriteElementString("Realm", tran.Realm.ToString());
        if (tran.RequireClientCertificate != el.RequireClientCertificate)
          w.WriteElementString("RequireClientCertificate", tran.Realm.ToString());
        if (tran.Scheme != el.Scheme)
          w.WriteElementString("Scheme", tran.Scheme.ToString());
        if (tran.TransferMode != el.TransferMode)
          w.WriteElementString("TransferMode", tran.TransferMode.ToString());
        if (tran.UnsafeConnectionNtlmAuthentication != el.UnsafeConnectionNtlmAuthentication)
          w.WriteElementString("UnsafeConnectionNtlmAuthentication", tran.UnsafeConnectionNtlmAuthentication.ToString());
        if (tran.UseDefaultWebProxy != el.UseDefaultWebProxy)
          w.WriteElementString("UseDefaultWebProxy", tran.UseDefaultWebProxy.ToString());
        w.WriteEndElement();
      }
      return sb.ToString();
    }
  }
}
