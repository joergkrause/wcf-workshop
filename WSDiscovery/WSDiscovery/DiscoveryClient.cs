using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using Masieri.ServiceModel.WSDiscovery.Activation;
using Masieri.ServiceModel.WSDiscovery.Transport;
using System.Xml;
using Masieri.ServiceModel.WSDiscovery.Messages;
using Masieri.ServiceModel.WSDiscovery.Client;
using Masieri.ServiceModel.WSDiscovery.Helpers;
using System.ServiceModel.Description;
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
namespace Masieri.ServiceModel.WSDiscovery
{
  /// <summary>
  /// DiscoveryClient class is the proxy to ther web service
  /// </summary>
  /// <typeparam name="TChannel">The type of the channel.</typeparam>
  public class DiscoveryClient<TChannel> : IDisposable where TChannel : class
  {
    /// <summary>
    /// Max Timeout before NoEndpointDiscoveredException
    /// </summary>
    public static int DiscoveryTimeout = Constants.DiscoveryTimeout;
    private ManualResetEvent _manualResetEvent;
    private IDynamicProxy _dynamicProxy;
    private UnicastListener _probeProbeMatchListener;
    private static MulticastListener _multicastListener;
    private static EndpointAddress _discoveryProxyAddress=null;
    private ChannelFactory<TChannel> _channelFactory;
    List<string> _scopes;
    UniqueId _probeMessageID;
    TChannel _lastUsedChannel = null;
    #region ctors and initialization
    static object _lock = new object();

    static DiscoveryClient()
    {
      _multicastListener = new MulticastListener();
    }
    private static void InitializeIstance(DiscoveryClient<TChannel> instance)
    {
      instance._manualResetEvent = new ManualResetEvent(false);
      instance._scopes = new List<string>();
      //Default values
      instance.ScopeMatchBy = Constants.ScopesMatchBy;
      instance._channelFactory = null;
      if (instance.GetBestMemento() == null)
      {
        DiscoveryLogger.Info("Opening upd unicast listener for the answers");
        //Unicast listener for answeres
        instance._probeProbeMatchListener = UnicastListener.GetUdpUnicastListener();
        instance._probeProbeMatchListener.MessageArrived += new DiscoveryListener.MessageArrivedEventHandler(instance._probeProbeMatchListener_MessageArrived);
      
      }
      //Multicast listener for Hello Bye Messages
      _multicastListener.MessageArrived += new DiscoveryListener.MessageArrivedEventHandler(instance._multicastListener_MessageArrived);
      
      DiscoveryLogger.Info("Discovery client created and initialized successful");
    }
    /// <summary>
    /// Base ctor DiscoveryClient
    /// </summary>
    public DiscoveryClient()
    {
      InitializeIstance(this);
      StartProbeProcess();
    }
    /// <summary>
    /// DiscoveryClient
    /// </summary>
    public DiscoveryClient(string scope)
    {
      InitializeIstance(this);
      Scopes.Add(scope);
      StartProbeProcess();
    }
    /// <summary>
    /// DiscoveryClient
    /// </summary>
    public DiscoveryClient(string scope, string matchBy)
    {
      InitializeIstance(this);
      ScopeMatchBy = matchBy;
      Scopes.Add(scope);
      StartProbeProcess();
    }
    /// <summary>
    /// DiscoveryClient
    /// </summary>
    public DiscoveryClient(string[] scopes)
    {
      InitializeIstance(this);
      Scopes.AddRange(scopes);
      StartProbeProcess();
    }
    /// <summary>
    /// DiscoveryClient
    /// </summary>
    public DiscoveryClient(string[] scopes, string matchBy)
    {
      InitializeIstance(this);
      Scopes.AddRange(scopes);
      StartProbeProcess();
    }
    #endregion
    #region proprieta
    //public IContextChannel ContextChannel{ get; set; }
    /// <summary>
    /// Discovered Services
    /// </summary>
    public List<ClientMemento> DiscoveredServices
    {
      get { return ClientContext.Current.GetList(Helpers.ContractDescriptionsHelper.GetContractFullName<TChannel>()); }
    }
    /// <summary>
    /// Scopes List
    /// </summary>
    public List<string> Scopes { get { return _scopes; } }
    /// <summary>
    /// Real Channel
    /// </summary>
    public TChannel Channel
    {
      get
      {
        if (_lastUsedChannel == null)
        {
          if (_dynamicProxy == null)
            _dynamicProxy = (IDynamicProxy)DynamicProxyFactory.Instance.CreateProxy(new object(),
                                                                                    new InvocationDelegate(InvocationHandler));
          return (TChannel)_dynamicProxy;
        }
        else
        {
          return (TChannel)_lastUsedChannel;
        }
      }
    }
    /// <summary>
    /// Gets or sets the scope match by.
    /// </summary>
    /// <value>The scope match by.</value>
    public string ScopeMatchBy { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [automatic change channel when faulted].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [automatic change channel when faulted]; otherwise, <c>false</c>.
    /// </value>
    public bool AutomaticChangeChannelWhenFaulted { get; set; }
    #endregion
    #region metodi
    private void StartProbeProcess()
    {
      if (GetBestMemento() == null)
      {
       Thread thread=new Thread(new ThreadStart(delegate()
        {
          //Sending the probe message
          _probeMessageID = new UniqueId(Guid.NewGuid());
          if (_probeProbeMatchListener == null)
          {
            DiscoveryLogger.Warn("No unicast trasport active");
            return;
          }
          EndpointReference replyTo = new EndpointReference { Address = string.Format("soap.udp://{0}:{1}/", _probeProbeMatchListener.Address, _probeProbeMatchListener.Port) };
          SoapEnvelope env;
          if (_discoveryProxyAddress == null)
          {
            env = ProbeResolveBuilder.BuildProbeMessage(_probeMessageID, replyTo, Helpers.ContractDescriptionsHelper.GetContractFullName<TChannel>(), Scopes, ScopeMatchBy);
            UdpOutputChannel.SendMulticast(env);
          }
          else
          {
            env = ProbeResolveBuilder.BuildProbeMessage(_probeMessageID, _discoveryProxyAddress.Uri, replyTo, Helpers.ContractDescriptionsHelper.GetContractFullName<TChannel>(), Scopes, ScopeMatchBy);
            UdpOutputChannel.SendUnicast(env, _discoveryProxyAddress.Uri.Host, _discoveryProxyAddress.Uri.Port);
          }
        }));
       thread.IsBackground = true;
       thread.Start();
      }
      else
        //a contract already exists 
        _manualResetEvent.Set();
    }
    #region Message handlers
    void _multicastListener_MessageArrived(object sender, Message message)
    {
      //Hello Bye
      switch (message.Headers.Action)
      {
        case Constants.HelloAction:
          //from service or DiscoveryProxy
          Hello hello = new Hello();
          if (message.State != MessageState.Created)
            return;
          hello.ReadXml(message.GetReaderAtBodyContents());
          //Add all the knowed contratcs
          foreach (string t in hello.Types.Split(new string[] {Constants.TypesSeparator}, StringSplitOptions.RemoveEmptyEntries))
          {
            //foreach logical type
            ClientMemento mem = new ClientMemento();
            if(!string.IsNullOrEmpty(hello.XAddrs))
              mem.XAddrs = hello.XAddrs;
            else
            {
              //if it is not specified mex address, it have to be my binding extensions
              if (hello.EndpointReferenceValue.ReferenceParameters == null || !(hello.EndpointReferenceValue.ReferenceParameters is CustomBinding))
              {
                //No binding extentions specified, no mex address. Return without storing
                return;
              }
            }
            mem.Type = t;
            mem.Address = hello.EndpointReferenceValue.Address;
            mem.MetadataVersion = Convert.ToInt64(hello.MetadataVersion);
            mem.Scopes = new List<string>(hello.Scopes.Split(new string[] { Constants.ScopesSeparator }, StringSplitOptions.RemoveEmptyEntries));
            mem.ScopeMatchBy = (string.IsNullOrEmpty(hello.ScopeMatchBy)) ? Constants.ScopesMatchBy : hello.ScopeMatchBy; 
            //I have to translate form logical type to phisycal one
            Type physicType = ServiceContractHelper.GetPhysicalFromLogical(t);
            
            if (physicType!=null)
            {
              //Builid EnpointRefenece
              mem.SetEndpointFromEndpointReferenceValue(hello.EndpointReferenceValue, physicType);
              //Add the memento
              ClientContext.Current.AddDiscoveredEndpoint(Helpers.ContractDescriptionsHelper.GetContractFullName(ContractDescription.GetContract(physicType)), mem);
              if (physicType == typeof(TChannel))
              {
                _manualResetEvent.Set();
              }
              
            }
          }
          break;
        case Constants.ByeAction:
          //from service or DiscoveryProxy
          Bye bye = new Bye();
          if (message.State != MessageState.Created)
            return;
          bye.ReadXml(message.GetReaderAtBodyContents());
          //Tolgo dai contratti conosciuti
          ClientContext.Current.RemoveDiscoveredEndpoint(bye.EndpointReferenceValue);
          break;
      }
    }

    void _probeProbeMatchListener_MessageArrived(object sender, Message message)
    {
      //ProbeMatch ResolveMatch da servizi
      //Hello bye da proxy
      switch (message.Headers.Action)
      {
        case Constants.ProbeMatchAction:
          //from service or DiscoveryProxy
          if (message.Headers.RelatesTo == this._probeMessageID)
          {
            ProbeMatches pm = ProbeMatches.FromReader(message.GetReaderAtBodyContents());
            ClientContext.Current.AddDiscoveredEndpoint(Helpers.ContractDescriptionsHelper.GetContractFullName<TChannel>(), ClientMemento.FromProbeMatch<TChannel>(pm.ProbeMatchValue));
            _manualResetEvent.Set();
          }
          break;
        case Constants.ResolveMatchAction:
          //from service or DiscoveryProxy
          ResolveMatches rm = ResolveMatches.FromReader(message.GetReaderAtBodyContents());
          break;
        case Constants.HelloAction:
          //from DiscoveryProxy
          //TODO: Discovery Proxy
          throw new NotImplementedException();
        case Constants.ByeAction:
          //from DiscoveryProxy
          //TODO: Discovery Proxy
          throw new NotImplementedException();
      }

    }
    #endregion
    /// <summary>
    /// Handler to invoke the interface TChannel 
    /// </summary>
    private object InvocationHandler(object target, MethodBase method, object[] parameters)
    {
      ClientMemento mem = null;
      Exception originalException = null;
      //Get the best channel
      if (_lastUsedChannel == null)
      {
        lock (_lock)
        {
          if (_lastUsedChannel == null)
          {
            mem = GetBestMemento();
            if (mem == null)
            {
              //No channels available.I'm waiting for a probematch message
              _manualResetEvent.WaitOne(DiscoveryTimeout, true);
              //I try to get if in the meanwhile something is arrived
              mem = GetBestMemento();
              if (mem == null)
                throw new NoDiscoveredEndpointException();
            }
            _lastUsedChannel = CreateChannel(mem);
          }
        }
      }

      do
      {
        try
        {
          OnInvoking();
          object ret = method.Invoke(_lastUsedChannel, parameters);
          OnInvoked();
          DiscoveryLogger.Info("Method invoke correctly");
          return ret;
        }
        catch (Exception ex)
        {
          DiscoveryLogger.Error("Error invoking the remote method", ex);

            //Endpoint don't work
            ClientContext.Current.RemoveDiscoveredEndpoint(Helpers.ContractDescriptionsHelper.GetContractFullName<TChannel>(), mem);
            //Look if exist a different one
            mem = GetBestMemento();
            if (mem == null)
            {
              //No other endpoints. I have to throw the Exception but first I send a Probe for the future
              StartProbeProcess();
              DiscoveryLogger.Warn("The DiscoveryClient can't scale on another service, I throw the Exception");
              //Nothing to do ... throw Exception
              if (originalException == null)
                throw ex;
              else
                throw originalException;
            }
            //I retry on another service
            if (originalException == null)
              originalException = ex;
          
        }
      } while (AutomaticChangeChannelWhenFaulted);
      return null;
    }
    internal event EventHandler Invoking;
    internal event EventHandler Invoked;

    private void OnInvoking()
    {
      if (Invoking != null) Invoking(this, new EventArgs());
    }

    private void OnInvoked()
    {
      if (Invoked != null) Invoked(this, new EventArgs());
    }
    /// <summary>
    /// Virtual GetBestMemento. Override it if you need to add a business logic to sort the services differently
    /// </summary>
    /// <returns></returns>
    protected virtual ClientMemento GetBestMemento()
    {
      foreach (ClientMemento mem in this.DiscoveredServices)
      {
        //if no scopes are configured then I get the first
        if (Scopes.Count == 0)
          return mem;
        else
          foreach (string scope in Scopes)
          {
            if (mem.ContainsScope(scope, this.ScopeMatchBy))
              return mem;
          }
      }
      return null;
    }
    private TChannel CreateChannel(ClientMemento mem)
    {
      DiscoveryLogger.Info("Utilizzo il canale con inditizzo: " + mem.Address);
      return GetFactory(mem.Endpoint.Binding).CreateChannel(mem.Endpoint.Address);
    }
    Binding _lastBinding = null;
    private ChannelFactory<TChannel> GetFactory(Binding binding)
    {
      if (_channelFactory == null)
      {
        _channelFactory = new ChannelFactory<TChannel>(binding);
        _lastBinding = binding;
      }
      else
      {
        if (_lastBinding.GetType() != binding.GetType())
        {
          _channelFactory.Close();
          _channelFactory = new ChannelFactory<TChannel>(binding);
          _lastBinding = binding;
        }
      }
      return _channelFactory;
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (_lastUsedChannel != null)
      {
        lock (_lock)
        {
          // closing the real channel
          if(_lastUsedChannel!=null)
            ((IChannel)_lastUsedChannel).Close();
          if (_channelFactory != null)
            _channelFactory.Close();
          _channelFactory = null;
          _lastBinding = null;
          _lastUsedChannel = null;
        }
      }
    }

    #endregion
    #endregion
  }
}
