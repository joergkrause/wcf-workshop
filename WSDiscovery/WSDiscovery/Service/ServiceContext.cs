using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Masieri.ServiceModel.WSDiscovery.Behaviors;
using System.ServiceModel;
using Masieri.ServiceModel.WSDiscovery.Messages;
using Masieri.ServiceModel.WSDiscovery.Service;
using Masieri.ServiceModel.WSDiscovery.Helpers;
using Masieri.ServiceModel.WSDiscovery.Transport;
using System.Configuration;
using System.Threading;
using System.Diagnostics;

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
namespace Masieri.ServiceModel.WSDiscovery.Service
{
  /// <summary>
  /// ServiceContext
  /// </summary>
 class ServiceContext
  {
    MulticastListener _listener;
    public long MetadataVersion { get; set; }
    public int MessageNumber { get; set; }
    Dictionary<string, ServiceMemento> _discoverableEndpoints;
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceContext"/> class.
    /// </summary>
    private ServiceContext()
    {
      _discoverableEndpoints = new Dictionary<string, ServiceMemento>();
      _listener = new MulticastListener();
      _listener.MessageArrived += new MulticastListener.MessageArrivedEventHandler(_listener_MessageArrived);
      MetadataVersion = DateTime.Now.Ticks;
      MessageNumber = 0;
    }
    #region SingleTon
    static ServiceContext _instance;
    static object _lockObject = new object();
    /// <summary>
    /// Gets the current Instance Singleton
    /// </summary>
    /// <value>The current.</value>
    public static ServiceContext Current
    {
      get
      {
        if (_instance == null)
          lock (_lockObject)
          {
            if (_instance == null)
            {
              _instance = new ServiceContext();
            }
          }
        return _instance;
      }
    }
    #endregion
    /// <summary>
    /// Gets the discoverable enpoints.
    /// </summary>
    /// <value>The discoverable enpoints.</value>
    public Dictionary<string, ServiceMemento> DiscoverableEnpoints { get { return _discoverableEndpoints; } }
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<int, ServiceHostBase> _discoverableServiceHosts = new Dictionary<int, ServiceHostBase>();
    /// <summary>
    /// Adds the service from configuration
    /// </summary>
    /// <param name="serviceHostBase">The service host base.</param>
    /// <param name="scopes">The scopes.</param>
    /// <param name="scopeMatchBy">The scope match by.</param>
     public void AddService(System.ServiceModel.ServiceHostBase serviceHostBase, string[] scopes, string scopeMatchBy)
    {
      ServiceMetadataBehavior metadata = serviceHostBase.Description.Behaviors.Find<ServiceMetadataBehavior>();
      string XAddrs = string.Empty;
      //Store ServiceMetadataBehavior address
      if (metadata != null)
      {
        if (metadata.HttpGetEnabled)
        {
          if (metadata.HttpGetUrl == null)
          {
            foreach (Uri uri in serviceHostBase.BaseAddresses)
              if (uri.Scheme == "http")
                XAddrs = uri.ToString();
          }
          else
            XAddrs = metadata.HttpGetUrl.ToString();

        }
        else if (metadata.HttpsGetEnabled)
        {
          if (metadata.HttpsGetUrl == null)
          {
            foreach (Uri uri in serviceHostBase.BaseAddresses)
              if (uri.Scheme == "https")
                XAddrs = uri.ToString();
          }
          else
            XAddrs = metadata.HttpsGetUrl.ToString();

        }
      }
      else
      {
        ServiceEndpoint mdep = null;
        foreach (ServiceEndpoint ep in serviceHostBase.Description.Endpoints)
        {
          if (ep.Contract.Name == "IMetadataExchange")
            mdep = ep;
        }
        if (mdep == null)
          throw new ConfigurationErrorsException("WSDiscovery require to work a Metadataexchange endpoint");
        XAddrs = mdep.Address.ToString();
      }

      //Add the discoverable endpoints
      foreach (ServiceEndpoint se in serviceHostBase.Description.Endpoints)
      {
        if (se.Contract.Name != "IMetadataExchange")
        {
          //Get the ServiceMemento of the endpoint
          ServiceMemento memento = new ServiceMemento() { EndpointName = se.Name, Address = se.Address.ToString(), Type = ContractDescriptionsHelper.GetContractFullName(se), XAddrs = XAddrs, MetadataVersion = DateTime.Now.Ticks, Endpoint = se };
          memento.ScopeMatchBy = scopeMatchBy;
          memento.Scopes = (scopes==null)? new string[0]:new List<string>(scopes).ToArray();
          _discoverableEndpoints.Add(memento.Type, memento);
        }
      }
      serviceHostBase.Opened += new EventHandler(serviceHostBase_Opened);
      serviceHostBase.Closing += new EventHandler(serviceHostBase_Closing);
      _discoverableServiceHosts.Add(serviceHostBase.GetHashCode(), serviceHostBase);
    }
    #region Bye
    /// <summary>
    /// Closing the service (Bye Message)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void serviceHostBase_Closing(object sender, EventArgs e)
    {
      ServiceHost host = sender as ServiceHost;
      foreach (ServiceEndpoint ep in host.Description.Endpoints)
      {
        if (ep.Contract.Name != "IMetadataExchange")
          UdpOutputChannel.SendMulticast(HandshakeMessageBuilder.BuildByeMessage(DiscoverableEnpoints[ContractDescriptionsHelper.GetContractFullName(ep)]));
      }
      _discoverableServiceHosts.Remove(host.GetHashCode());
      if (_discoverableServiceHosts.Count == 0)
      {
        this.Dispose();
      }
    }
    #endregion
    private void Dispose()
    {
      _listener.Dispose();
    }
    #region Hello
    /// <summary>
    /// Service Opened (Hello Message)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void serviceHostBase_Opened(object sender, EventArgs e)
    {
      ServiceHost host = sender as ServiceHost;
      foreach (ServiceEndpoint ep in host.Description.Endpoints)
      {
        if (ep.Contract.Name != "IMetadataExchange")
          UdpOutputChannel.SendMulticast(HandshakeMessageBuilder.BuildHelloMessage(DiscoverableEnpoints[ContractDescriptionsHelper.GetContractFullName(ep)]));
      }
    }
    #endregion
    #region ProbeMatch/ResolveMatch
    void _listener_MessageArrived(object sender, Message message)
    {
      if (message.Headers.Action == Constants.ProbeAction)
      {
        Probe probe = new Probe();
        probe.ReadXml(message.GetReaderAtBodyContents());
        if (string.IsNullOrEmpty(probe.Types))
        {
          //scopes empty then I send the probeMatch
          foreach (ServiceMemento mem in _discoverableEndpoints.Values)
          {
            //Check the scopes
            bool scopeMatches = true;
            if (!string.IsNullOrEmpty(probe.Scopes))
            {
              List<string> scopesRequested = new List<string>(probe.Scopes.Split(new string[] { Constants.ScopesSeparator }, StringSplitOptions.RemoveEmptyEntries));
              scopeMatches = CheckScopes(new List<string>(mem.Scopes), scopesRequested);
            }
            if (scopeMatches)
            {
              //Probe Match 
              SoapEnvelope probeMatchResponse = ProbeResolveMatchBuilder.BuildProbeMatchMessage(mem, message.Headers.ReplyTo.ToString(), message.Headers.MessageId);
              OutputChannel.Send(probeMatchResponse, message.Headers.ReplyTo.Uri);
            }
          }



          return;
        }
        //ProbeMatch: check if I have the requested type
        if (_discoverableEndpoints.ContainsKey(probe.Types))
        {
          if (_discoverableEndpoints.ContainsKey(probe.Types))
          {
            ServiceMemento mem = _discoverableEndpoints[probe.Types];
            bool scopeMatches = true;
            if (!string.IsNullOrEmpty(probe.Scopes))
            {
              DiscoveryLogger.Debug(string.Format("Received Scopes: {0} | Implementatedi: {1}", probe.Scopes, string.Join(Constants.ScopesSeparator, mem.Scopes.ToArray())));
              List<string> scopesRequested = new List<string>(probe.Scopes.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
              scopeMatches = CheckScopes(new List<string>(mem.Scopes), scopesRequested);
            }
            //Check for scopes
            if (scopeMatches)
            {
              //Send Probe Match Message
              SoapEnvelope probeMatchResponse = ProbeResolveMatchBuilder.BuildProbeMatchMessage(mem, message.Headers.ReplyTo.ToString(), message.Headers.MessageId);
              OutputChannel.Send(probeMatchResponse, message.Headers.ReplyTo.Uri);
            }
          }
          else
            return;
        }
        else
          return;
      }
      else if (message.Headers.Action == Constants.ResolveAction)
      {
        Resolve resolve = new Resolve();
        resolve.ReadXml(message.GetReaderAtBodyContents());
        //ResolveMatch:check if I have the requesed type
        ServiceMemento mem = _discoverableEndpoints[resolve.Types];
        List<string> scopesRequested = new List<string>(resolve.Scopes.Split(new string[] { Constants.ScopesSeparator }, StringSplitOptions.RemoveEmptyEntries));
        //Check for scopes
        if (CheckScopes(mem.Scopes, scopesRequested))
        {
          //Send Resolve Match Message
          SoapEnvelope resolveMatchResponse = ProbeResolveMatchBuilder.BuildResolveMatchMessage(_discoverableEndpoints[resolve.Types], message.Headers.ReplyTo.ToString(), message.Headers.MessageId);
          OutputChannel.Send(resolveMatchResponse, message.Headers.ReplyTo.Uri);
        }
      }
    }
    private bool CheckScopes(string[] serviceScopes, List<string> scopesRequested)
    {
      return CheckScopes(new List<string>(serviceScopes), scopesRequested);
    }
    private bool CheckScopes(List<string> serviceScopes, List<string> scopesRequested)
    {
      foreach (string serviceScope in serviceScopes)
      {
        foreach (string scopeRequested in scopesRequested)
        {
          try
          {
            if (new Uri(scopeRequested).IsSameUri(new Uri(serviceScope)))
              return true;
          }
          catch (Exception ex)
          {
            DiscoveryLogger.Error("Exception throwed during comparition with remote scopes. Check if all remote scopes are Uri ", ex);
            return false;
          }
        }
      }
      return false;
    }
    #endregion
  }
}
