using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Transport;
using System.ServiceModel.Channels;
using Masieri.ServiceModel.WSDiscovery.Messages;
using Masieri.ServiceModel.WSDiscovery.Service;
using Masieri.ServiceModel.WSDiscovery.Client;
using System.Net;

namespace Masieri.ServiceModel.WSDiscovery.Proxy
{
  /// <summary>
  /// Iniziata ma appena abbozzata
  /// </summary>
  public class ProxyContext
  {
    MulticastListener _listener;
    UnicastListener _unicastListener;
    //Dictionary ordinato per type, e dictionary come value che ha chiave address e valore il proxymemento
    Dictionary<string, Dictionary<string, ProxyMemento>> _discoveredEndpoints;
    //Modalità di configurazione in cui il proxy funziona in modo invisibile per la diagnostica;
    bool _passiveMode = false;
    private ProxyContext()
    {
      _discoveredEndpoints = new Dictionary<string, Dictionary<string, ProxyMemento>>();
      _listener = new MulticastListener();
      //creo il Listener unicast
      _unicastListener = UnicastListener.GetUdpUnicastListener();
      _listener.MessageArrived += new MulticastListener.MessageArrivedEventHandler(_listener_MessageArrived);
      _unicastListener.MessageArrived += new DiscoveryListener.MessageArrivedEventHandler(_unicastListener_MessageArrived);
      //a questo punto invio il messaggio di Hello del DP
      UdpOutputChannel.SendMulticast(HandshakeMessageBuilder.BuildHelloMessage(new ServiceMemento() { }));
    }

    #region SingleTon
    static ProxyContext _instance;
    static object _lockObject = new object();
    public static ProxyContext Current
    {
      get
      {
        if (_instance == null)
          lock (_lockObject)
          {
            if (_instance == null)
            {
              _instance = new ProxyContext();
            }
          }
        return _instance;
      }
    }
    #endregion
    /// <summary>
    /// Messaggi unicast dal client
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    void _unicastListener_MessageArrived(object sender, Message message)
    {
      if (message.Headers.Action == Constants.ProbeAction)
      {
        if (!_passiveMode)
        {
          Probe probe = new Probe();
          probe.ReadXml(message.GetReaderAtBodyContents());
          //prm ho l'entry richiesta la invio
          var service = from d in _discoveredEndpoints
                        from pm in d.Value
                        //probe.Types
                        where probe.Types.IndexOf(d.Key) >= 0
                          && this.CheckScopes(pm.Value.Scopes, probe.Scopes)
                        select d.Value[pm.Key]
                      ;
          ProxyMemento prm = service.FirstOrDefault<ProxyMemento>();
          if (prm != null)
          {
            //Rispondo con Probe Match 
            SoapEnvelope probeMatchResponse = ProbeResolveMatchBuilder.BuildProbeMatchMessage(prm, message.Headers.ReplyTo.ToString(), message.Headers.MessageId);
            OutputChannel.Send(probeMatchResponse, message.Headers.ReplyTo.Uri);
          }
          else
          {
            //invio una probe multicast con la richiesta
            //attendo una risposta per DP_MAX_TIMEOUT
            //prm ricevo la risposta invio la probeMatch
          }
        }
      }
      else if (message.Headers.Action == Constants.ResolveAction)
      {
        //invio la resolveMatch
        if (!_passiveMode)
        {
          Resolve resolve = new Resolve();
          resolve.ReadXml(message.GetReaderAtBodyContents());
          //prm ho l'entry richiesta la invio
          var service = from d in _discoveredEndpoints
                        from pm in d.Value
                        //probe.Types
                        where resolve.Types.IndexOf(d.Key) >= 0
                          && this.CheckScopes(pm.Value.Scopes, resolve.Scopes)
                        select d.Value[pm.Key]
                      ;
          ProxyMemento prm = service.FirstOrDefault<ProxyMemento>();
          if (prm != null)
          {
            //Rispondo con Resolve Match 
            SoapEnvelope probeMatchResponse = ProbeResolveMatchBuilder.BuildResolveMatchMessage(prm, message.Headers.ReplyTo.ToString(), message.Headers.MessageId);
            OutputChannel.Send(probeMatchResponse, message.Headers.ReplyTo.Uri);
          }
        }
      }
    }
    /// <summary>
    /// Messaggi multicast
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    void _listener_MessageArrived(object sender, Message message)
    {
      if (message.Headers.Action == Constants.ProbeAction)
      {
        //Probe multicast devo rispondere con un Hello
        SoapEnvelope helloResponse = HandshakeMessageBuilder.BuildHelloMessage(new ServiceMemento() { Address = string.Format("soap.udp://{0}:{1}", this._unicastListener.Address, this._unicastListener.Port), Type = "d:DiscoveryProxy d:TargetService" });
        //specificato SOLO dal discoveryProxy
        helloResponse.AddHeader(new SoapHeader() { Name = "RelatesTo", Value = message.Headers.MessageId.ToString() });
        OutputChannel.Send(helloResponse, message.Headers.ReplyTo.Uri);

      }
      else if (message.Headers.Action == Constants.ResolveAction)
      {
        //Resolve multicast devo rispondere con un Hello
        SoapEnvelope helloResponse = HandshakeMessageBuilder.BuildHelloMessage(new ServiceMemento() { Address = string.Format("soap.udp://{0}:{1}", this._unicastListener.Address, this._unicastListener.Port), Type = "d:DiscoveryProxy d:TargetService" });
        //specificato SOLO dal discoveryProxy
        helloResponse.AddHeader(new SoapHeader() { Name = "RelatesTo", Value = message.Headers.MessageId.ToString() });
        OutputChannel.Send(helloResponse, message.Headers.ReplyTo.Uri);


      }
      else if (message.Headers.Action == Constants.HelloAction)
      {
        //TODO: aggiungere il contratto alla lista prm presente
        Hello hello = new Hello();
        hello.ReadXml(message.GetReaderAtBodyContents());
        foreach (string t in hello.Type)
        {
          if (!_discoveredEndpoints.ContainsKey(t))
          {
            _discoveredEndpoints.Add(t, new Dictionary<string, ProxyMemento>());
          }
          if (!_discoveredEndpoints[t].ContainsKey(hello.EndpointReferenceValue.Address))
            _discoveredEndpoints[t].Add(hello.EndpointReferenceValue.Address, new ProxyMemento(t, hello));
          else if (Convert.ToInt32(_discoveredEndpoints[t][hello.EndpointReferenceValue.Address].MetadataVersion) < Convert.ToInt32(hello.MetadataVersion))
            //sostituisco
            _discoveredEndpoints[t][hello.EndpointReferenceValue.Address] = new ProxyMemento(t, hello);
        }
      }
      else if (message.Headers.Action == Constants.ByeAction)
      {
        //elimino il contratto dalla lista prm presente
        Bye bye = new Bye();
        bye.ReadXml(message.GetReaderAtBodyContents());
        //recupero una lista di chiavi (type) che hanno all'interno l'indirizzo del servizio che si è spento
        var list = from d in _discoveredEndpoints
                   where d.Value.ContainsKey(bye.EndpointReferenceValue.Address)
                   select d.Key;
        foreach (var key in list)
        {
          _discoveredEndpoints[key].Remove(bye.EndpointReferenceValue.Address);
        }
      }
    }
    private bool CheckScopes(string scopes, string requestedScopes)
    {
      List<string> serviceScopes = new List<string>(scopes.Split(new string[] { Constants.ScopesSeparator }, StringSplitOptions.RemoveEmptyEntries));
      List<string> clientRequestedScopes = new List<string>(requestedScopes.Split(new string[] { Constants.ScopesSeparator }, StringSplitOptions.RemoveEmptyEntries));
      return CheckScopes(serviceScopes, clientRequestedScopes); 
    }
    //Nella specifica si dice che il servizio risponde basta che uno scope coincida, ma nella risposta
    //la probe match DEVE inviare tutti gli scopes che sono posseduti => il client DOVREBBE poter eventualmente
    //scaratare eventuali probeMatch, qualora volesse una politica particolare (tipo voglio i servizi che implementano tutti gli scopes richiesti)
    private bool CheckScopes(List<string> serviceScopes, List<string> scopesRequested)
    {
      int scopesMatched = 0;
      foreach (string serviceScope in serviceScopes)
      {
        foreach (string scopeRequested in scopesRequested)
        {
          if (scopeRequested.ToLower() == serviceScope.ToLower())
            return true;
          //scopesMatched++;
        }
      }
      return false;
      //return scopesMatched == scopesRequested.Count ? true : false;
    }

  }

}
