using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

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
  /// ProbeResolveBuilder
  /// </summary>
  class ProbeResolveBuilder
  {
    /// <summary>
    /// Builds the probe message.
    /// </summary>
    /// <param name="probeMessageID">The probe message ID.</param>
    /// <param name="replyTo">The reply to.</param>
    /// <param name="type">The type.</param>
    /// <param name="scopes">The scopes.</param>
    /// <param name="scopeMatchBy">The scope match by.</param>
    /// <returns></returns>
    public static SoapEnvelope BuildProbeMessage(System.Xml.UniqueId probeMessageID, EndpointReference replyTo, string type, List<string> scopes, string scopeMatchBy)
    {
      return BuildProbeMessage(probeMessageID, new Uri(Constants.Addressing.AnonymousDestination), replyTo, type, scopes, scopeMatchBy);
    }

    /// <summary>
    /// Builds the probe message.
    /// </summary>
    /// <param name="probeMessageID">The probe message ID.</param>
    /// <param name="url">The URL.</param>
    /// <param name="replyTo">The reply to.</param>
    /// <param name="type">The type.</param>
    /// <param name="scopes">The scopes.</param>
    /// <param name="scopeMatchBy">The scope match by.</param>
    /// <returns></returns>
    public static SoapEnvelope BuildProbeMessage(System.Xml.UniqueId probeMessageID, Uri url, EndpointReference replyTo, string type, List<string> scopes, string scopeMatchBy)
    {
      SoapEnvelope env = new SoapEnvelope(Constants.SOAPMessageVersion);
      env.AddHeader(new SoapHeader() { Name = "a:Action", Value = Constants.ProbeAction });
      env.AddHeader(new SoapHeader() { Name = "a:MessageID", Value = probeMessageID.ToString() });
      env.Headers.Add(new SoapHeader() { Name = "a:To", Value = url.ToString() });
      if (replyTo != null)
        env.Headers.Add(new SoapHeader() { Name = "a:ReplyTo", Value = string.Format("<a:Address>{0}</a:Address>", replyTo.Address) });

      Probe probe = new Probe();
      probe.Types = type;
      if (scopes != null)
      {
        probe.ScopeMatchBy = scopeMatchBy;
        probe.Scopes = String.Join("\r\n", scopes.ToArray());
      }
      env.BodyContent = probe.ToString();
      return env;

    }

  }
}
