using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using Masieri.ServiceModel.WSDiscovery.Helpers;
using Masieri.ServiceModel.WSDiscovery.Service;

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
namespace Masieri.ServiceModel.WSDiscovery.Messages
{
  /// <summary>
  /// Builder class to build Handshake Messages
  /// </summary>
  class HandshakeMessageBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HandshakeMessageBuilder"/> class.
    /// </summary>
    private HandshakeMessageBuilder()
    {
    }
    /// <summary>
    /// Builds the hello message.
    /// </summary>
    /// <param name="se">The se.</param>
    /// <returns></returns>
    public static SoapEnvelope BuildHelloMessage(ServiceMemento se)
    {
      SoapEnvelope envelope = new SoapEnvelope(Constants.SOAPMessageVersion);
      envelope.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.HelloAction });
      envelope.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = new UniqueId().ToString() });
      envelope.AddHeader(new SoapHeader() { Name = "a:To", Value = Constants.TargetService });
      SoapHeader appSequence = new SoapHeader() { Name = "d:AppSequence" };
      appSequence.AddAttribute("InstanceId", ServiceContext.Current.MetadataVersion.ToString());
      appSequence.AddAttribute("MessageNumber", (++ServiceContext.Current.MessageNumber).ToString());
      envelope.AddHeader(appSequence);
      StringBuilder sb = new StringBuilder();
      sb.Append("<d:Hello>");
      EndpointReference er = new EndpointReference();
      er.Address = se.Address;
      //discoveryproxy è null
      if (se.Endpoint != null)
        er.ReferenceParameters = se.GetBindingExtensions();
      sb.AppendFormat(er.ToString());
      sb.AppendFormat("<d:Types>{0}</d:Types>", se.Type);
      sb.AppendFormat("<d:Scopes MatchBy=\"{1}\">{0}</d:Scopes>", String.Join(Constants.ScopesSeparator, se.Scopes.ToArray<string>()), se.ScopeMatchBy);
      if (se.XAddrs != null)
        sb.AppendFormat("<d:XAddrs>{0}</d:XAddrs>", se.XAddrs);
      sb.AppendFormat("<d:MetadataVersion>{0}</d:MetadataVersion>", ServiceContext.Current.MetadataVersion);
      sb.AppendLine("</d:Hello>");
      envelope.BodyContent = sb;
      //Message message = Message.CreateMessage(new XmlTextReader(new StringReader(envelope.ToString())),1024,MessageVersion.None);
      return envelope;
    }
    /// <summary>
    /// Builds the bye message.
    /// </summary>
    /// <param name="se">The se.</param>
    /// <returns></returns>
    public static SoapEnvelope BuildByeMessage(ServiceMemento se)
    {
      SoapEnvelope envelope = new SoapEnvelope(Constants.SOAPMessageVersion);
      envelope.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.ByeAction });
      envelope.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = new UniqueId().ToString() });
      envelope.AddHeader(new SoapHeader() { Name = "a:To", Value = Constants.TargetService });
      SoapHeader appSequence = new SoapHeader() { Name = "d:AppSequence" };
      appSequence.AddAttribute("InstanceId", ServiceContext.Current.MetadataVersion.ToString());
      appSequence.AddAttribute("MessageNumber", (++ServiceContext.Current.MessageNumber).ToString());
      envelope.AddHeader(appSequence);
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("<d:Bye>");
      EndpointReference er = new EndpointReference();
      er.Address = se.Address;
      sb.AppendFormat(er.ToString());
      sb.AppendLine("</d:Bye>");
      envelope.BodyContent = sb;
      return envelope;
    }
    /// <summary>
    /// HandShake class with discovery Proxy
    /// </summary>
    public static class DiscoveryProxy
    {
      /// <summary>
      /// Builds the hello multicast message.
      /// </summary>
      /// <param name="se">The se.</param>
      /// <returns></returns>
      public static SoapEnvelope BuildHelloMulticastMessage(ServiceMemento se)
      {
        //TODO:
        return null;
      }
      /// <summary>
      /// Builds the hello unicast message.
      /// </summary>
      /// <param name="se">The se.</param>
      /// <returns></returns>
      public static SoapEnvelope BuildHelloUnicastMessage(ServiceMemento se)
      {
        //TODO:
        return null;
      }
    }
  }
}
