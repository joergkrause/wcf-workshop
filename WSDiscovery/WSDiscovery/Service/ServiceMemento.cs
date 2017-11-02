using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Masieri.ServiceModel.WSDiscovery.Helpers;
using System.Threading;
using Masieri.ServiceModel.WSDiscovery.Messages;
using Masieri.ServiceModel.WSDiscovery.Transport;

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
  /// Service Memento class
  /// </summary>
  public class ServiceMemento
  {
    /// <summary>
    /// Gets or sets the name of the endpoint.
    /// </summary>
    /// <value>The name of the endpoint.</value>
    public string EndpointName { get; set; }
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>The address.</value>
    public string Address { get; set; }
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }
    /// <summary>
    /// Gets or sets the scope match by.
    /// </summary>
    /// <value>The scope match by.</value>
    public string ScopeMatchBy
    {
      get { return _serviceScopes.ScopeMatchBy; }
      set
      {
        _serviceScopes.ScopeMatchBy = value;
      }
    }
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    public string[] Scopes
    {
      get { return _serviceScopes.Scopes.ToArray(); }
      set
      {
        _serviceScopes.Scopes.Clear();
        _serviceScopes.Scopes.AddRange(value);
      }
    }
    /// <summary>
    /// Gets the service scope.
    /// </summary>
    /// <value>The service scope.</value>
    internal ServiceScopes ServiceScope
    {
      get { return _serviceScopes; }
    }
    /// <summary>
    /// Gets or sets the X addr.
    /// </summary>
    /// <value>The X addr.</value>
    public string XAddrs { get; set; }
    /// <summary>
    /// Gets or sets the metadata version.
    /// </summary>
    /// <value>The metadata version.</value>
    public long MetadataVersion { get; set; }
    ServiceEndpoint _endpoint;
    private ServiceScopes _serviceScopes;
    /// <summary>
    /// Eventuale endpoint
    /// </summary>
    /// <value>The endpoint.</value>
    public ServiceEndpoint Endpoint
    {
      get { return _endpoint; }
      set
      {
        _endpoint = value;
        Thread extensions = new Thread(new ThreadStart(delegate()
            {
              lock (_lockObject)
              {
                DiscoveryLogger.Debug("Start to generate binding extensions");
                _bindingExtensions = BindingMemento.GetMemento(_endpoint.Binding);
                DiscoveryLogger.Debug("End to generate binding extensions");
              }
            }));
        extensions.IsBackground = true;
        extensions.Start();
      }
    }
    object _lockObject = new object();
    string _bindingExtensions = null;
    /// <summary>
    /// Gets the binding extensions.
    /// </summary>
    /// <returns></returns>
    public string GetBindingExtensions()
    {

      if (_bindingExtensions == null)
      {
        lock (_lockObject)
        {
          if (_bindingExtensions == null)
          {
            DiscoveryLogger.Debug("Start to generate binding extensions");
            _bindingExtensions = BindingMemento.GetMemento(_endpoint.Binding);
            DiscoveryLogger.Debug("End to generate binding extensions");
          }
        }
      }
      return _bindingExtensions;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMemento"/> class.
    /// </summary>
    public ServiceMemento()
    {
      _serviceScopes = new ServiceScopes();
      _serviceScopes.Scopes.ScopeListModified += new EventHandler(Scopes_ScopeListModified);
    }

    void Scopes_ScopeListModified(object sender, EventArgs e)
    {
      MetadataVersion++;
      UdpOutputChannel.SendMulticast(HandshakeMessageBuilder.BuildHelloMessage(this));
    }
  }
}
