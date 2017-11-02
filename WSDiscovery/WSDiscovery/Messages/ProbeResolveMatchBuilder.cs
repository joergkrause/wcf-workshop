using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
  /// ProbeResolveMatchBuilder
  /// </summary>
  class ProbeResolveMatchBuilder
  {
    /// <summary>
    /// Builds the probe match message.
    /// </summary>
    /// <param name="se">The se.</param>
    /// <param name="to">To.</param>
    /// <param name="relatesTo">The relates to.</param>
    /// <returns></returns>
    public static SoapEnvelope BuildProbeMatchMessage(ServiceMemento se, string to, UniqueId relatesTo)
    {
      SoapEnvelope envelope = new SoapEnvelope(Constants.SOAPMessageVersion);
      //Headers
      envelope.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.ProbeMatchAction });
      envelope.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = new UniqueId().ToString() });
      envelope.AddHeader(new SoapHeader() { Name = "a:To", Value = to });
      envelope.AddHeader(new SoapHeader() { Name = "a:RelatesTo", Value = relatesTo.ToString() });
      ProbeMatches pm = new ProbeMatches();
      pm.ProbeMatchValue = new ProbeMatches.ProbeMatch();
      pm.ProbeMatchValue.EndpointReferenceValue = new EndpointReference();
      pm.ProbeMatchValue.EndpointReferenceValue.Address = se.Address;
      pm.ProbeMatchValue.EndpointReferenceValue.ReferenceParameters = se.GetBindingExtensions();
      pm.ProbeMatchValue.Types = se.Type;
      pm.ProbeMatchValue.ScopeMatchBy = se.ScopeMatchBy;
      pm.ProbeMatchValue.Scopes = String.Join(Constants.ScopesSeparator, se.Scopes.ToArray<string>());
      pm.ProbeMatchValue.XAddrs = se.XAddrs;
      pm.ProbeMatchValue.MetadataVersion = se.MetadataVersion.ToString();
      envelope.BodyContent = pm;
      return envelope;

    }
    /// <summary>
    /// Builds the resolve match message.
    /// </summary>
    /// <param name="se">The se.</param>
    /// <param name="to">To.</param>
    /// <param name="relatesTo">The relates to.</param>
    /// <returns></returns>
    public static SoapEnvelope BuildResolveMatchMessage(ServiceMemento se, string to, UniqueId relatesTo)
    {
      SoapEnvelope envelope = new SoapEnvelope(Constants.SOAPMessageVersion);
      //Headers
      envelope.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.ResolveMatchAction });
      envelope.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = new UniqueId().ToString() });
      envelope.AddHeader(new SoapHeader() { Name = "a:To", Value = to });
      envelope.AddHeader(new SoapHeader() { Name = "a:RelatesTo", Value = relatesTo.ToString() });
      SoapHeader appSequence = new SoapHeader() { Name = "d:AppSequence" };
      appSequence.AddAttribute("InstanceId", ServiceContext.Current.MetadataVersion.ToString());
      appSequence.AddAttribute("MessageNumber", (++ServiceContext.Current.MessageNumber).ToString());
      envelope.AddHeader(appSequence);
      ResolveMatches rm = new ResolveMatches();
      rm.ResolveMatchValue = new ResolveMatches.ResolveMatch();
      rm.ResolveMatchValue.EndpointReferenceValue = new EndpointReference();
      rm.ResolveMatchValue.EndpointReferenceValue.Address = se.Address;
      rm.ResolveMatchValue.Types = se.Type;
      rm.ResolveMatchValue.Scopes = string.Join(Constants.ScopesSeparator, se.Scopes.ToArray<string>());
      rm.ResolveMatchValue.XAddrs = se.XAddrs;
      rm.ResolveMatchValue.MetadataVersion = se.MetadataVersion.ToString();
      envelope.BodyContent = rm;
      return envelope;
    }
    /*
    /// <summary>
    /// Builds the probe match message.
    /// </summary>
    /// <param name="prm">The PRM.</param>
    /// <param name="to">To.</param>
    /// <param name="relatesTo">The relates to.</param>
    /// <returns></returns>
    internal static SoapEnvelope BuildProbeMatchMessage(ProxyMemento prm, string to, UniqueId relatesTo)
    {
      SoapEnvelope envelope = new SoapEnvelope(Constants.SOAPMessageVersion);
      //Headers
      envelope.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.ProbeMatchAction });
      envelope.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = new UniqueId().ToString() });
      envelope.AddHeader(new SoapHeader() { Name = "a:To", Value = to });
      envelope.AddHeader(new SoapHeader() { Name = "a:RelatesTo", Value = relatesTo.ToString() });
      ProbeMatches pm = new ProbeMatches();
      pm.ProbeMatchValue = new ProbeMatches.ProbeMatch();
      pm.ProbeMatchValue.EndpointReferenceValue = new EndpointReference();
      pm.ProbeMatchValue.EndpointReferenceValue.Address = prm.EndpointReferenceValue.Address;
      pm.ProbeMatchValue.EndpointReferenceValue.ReferenceParameters = prm.EndpointReferenceValue.ReferenceParameters;
      pm.ProbeMatchValue.Types = prm.Type;
      pm.ProbeMatchValue.ScopeMatchBy = prm.ScopeMatchBy;
      pm.ProbeMatchValue.Scopes = prm.Scopes;
      pm.ProbeMatchValue.XAddrs = prm.XAddrs;
      pm.ProbeMatchValue.MetadataVersion = prm.MetadataVersion.ToString();
      envelope.BodyContent = pm;
      return envelope;

    }
    /// <summary>
    /// Builds the resolve match message.
    /// </summary>
    /// <param name="prm">The PRM.</param>
    /// <param name="to">To.</param>
    /// <param name="relatesTo">The relates to.</param>
    /// <returns></returns>
    internal static SoapEnvelope BuildResolveMatchMessage(ProxyMemento prm, string to, UniqueId relatesTo)
    {
      SoapEnvelope envelope = new SoapEnvelope(Constants.SOAPMessageVersion);
      //Headers
      envelope.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.ResolveMatchAction });
      envelope.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = new UniqueId().ToString() });
      envelope.AddHeader(new SoapHeader() { Name = "a:To", Value = to });
      envelope.AddHeader(new SoapHeader() { Name = "a:RelatesTo", Value = relatesTo.ToString() });
      SoapHeader appSequence = new SoapHeader() { Name = "d:AppSequence" };
      appSequence.AddAttribute("InstanceId", ServiceContext.Current.MetadataVersion.ToString());
      appSequence.AddAttribute("MessageNumber", (++ServiceContext.Current.MessageNumber).ToString());
      envelope.AddHeader(appSequence);
      ResolveMatches rm = new ResolveMatches();
      rm.ResolveMatchValue = new ResolveMatches.ResolveMatch();
      rm.ResolveMatchValue.EndpointReferenceValue = new EndpointReference();
      rm.ResolveMatchValue.EndpointReferenceValue.Address = prm.EndpointReferenceValue.Address;
      rm.ResolveMatchValue.Types = prm.Type;
      rm.ResolveMatchValue.Scopes = prm.Scopes;
      rm.ResolveMatchValue.XAddrs = prm.XAddrs;
      rm.ResolveMatchValue.MetadataVersion = prm.MetadataVersion.ToString();
      envelope.BodyContent = rm;
      return envelope;
    }
    */
  }
}
