using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Transport;
using System.ServiceModel.Channels;
using Masieri.ServiceModel.WSDiscovery.Client;
using System.Threading;
using Masieri.ServiceModel.WSDiscovery.Messages;
using System.Xml;

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
namespace Masieri.ServiceModel.WSDiscovery.Diagnostics
{
  /// <summary>
  /// Diagnostic class to Test some Tasks
  /// </summary>
  public class DiagnosticHelper
  {
    private UnicastListener _probeProbeMatchListener;
    private ManualResetEvent _manualResetEvent;
    UniqueId _probeMessageID;
    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticHelper"/> class.
    /// </summary>
    public DiagnosticHelper()
    {
      _probeProbeMatchListener = UnicastListener.GetUdpUnicastListener();
      _probeProbeMatchListener.MessageArrived += new DiscoveryListener.MessageArrivedEventHandler(_probeProbeMatchListener_MessageArrived);
      //invio una probe con tipo vuoto e scopes vuoti
      StartProbeProcess();
      

    }
    /// <summary>
    /// Gets the memento list.
    /// </summary>
    /// <value>The memento list.</value>
    public List<ClientMemento> MementoList 
    {
      get { return ClientContext.Current.GetList(null); } 
    }
    private void StartProbeProcess()
    {
      new Thread(new ThreadStart(delegate()
      {
        //Invio il messaggio di probe
        _probeMessageID = new UniqueId(Guid.NewGuid());
        _manualResetEvent = new ManualResetEvent(false);
        EndpointReference replyTo = new EndpointReference { Address = string.Format("soap.udp://{0}:{1}/", _probeProbeMatchListener.Address, _probeProbeMatchListener.Port) };
        SoapEnvelope env;
        env = ProbeResolveBuilder.BuildProbeMessage(_probeMessageID, replyTo, "", null, Constants.ScopesMatchBy);
        UdpOutputChannel.SendMulticast(env);
       
      })).Start();
    }
    void _probeProbeMatchListener_MessageArrived(object sender, Message message)
    {
      //ProbeMatch ResolveMatch da servizi
      //Hello bye da proxy
      switch (message.Headers.Action)
      {
        case Constants.ProbeMatchAction:
          //in risposta dal servizio
          if (message.Headers.RelatesTo == this._probeMessageID)
          {
            //riposta al mio messaggio
            ProbeMatches pm = ProbeMatches.FromReader(message.GetReaderAtBodyContents());
            foreach (string t in pm.ProbeMatchValue.Types.Split(new string[] { Constants.TypesSeparator }, StringSplitOptions.RemoveEmptyEntries))
            {
              ClientContext.Current.AddDiscoveredEndpoint(t, new ClientMemento() { Address=pm.ProbeMatchValue.EndpointReferenceValue.Address, XAddrs=pm.ProbeMatchValue.XAddrs, Type=t, Scopes=new List<string>(pm.ProbeMatchValue.Scopes.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries)), ScopeMatchBy=pm.ProbeMatchValue.ScopeMatchBy, MetadataVersion=Convert.ToInt64(pm.ProbeMatchValue.MetadataVersion)});
            }
              _manualResetEvent.Set();
          }
          break;
        case Constants.ResolveMatchAction:
          //TODO: in risposta dal servizio
          break;
        case Constants.HelloAction:
          //TODO: Update List
          break;
        case Constants.ByeAction:
          //TODO: Update List

          break;
      }

    }
  }
}
