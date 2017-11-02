using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using Masieri.ServiceModel.WSDiscovery.Service;
using System.ServiceModel;
using Masieri.ServiceModel.WSDiscovery.Helpers;

namespace Masieri.ServiceModel.WSDiscovery.Behaviors
{
  /// <summary>
  /// WSDiscovery ServiceBehavior  class
  /// </summary>
  public class ServiceDiscoverableBehavior : IServiceBehavior
  {
    ServiceHostBase _serviceHostBase = null;
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDiscoverableBehavior"/> class.
    /// </summary>
    public ServiceDiscoverableBehavior()
    {
      ScopesMatchBy = Constants.ScopesMatchBy;
    }
    /// <summary>
    /// Gets or sets the scopes match by.
    /// </summary>
    /// <value>The scopes match by.</value>
    public string ScopesMatchBy { get; set; }
    string[] _scopes;
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    public string[] Scopes
    {
      get { return _scopes; }
      set
      {
        _scopes = value;
        if (_serviceHostBase != null)
        {
          //il servizio è stato inizializzato devo aggiornare il memento   
          //Aggiungo i behaviors agli endpoint
          foreach (ServiceEndpoint se in _serviceHostBase.Description.Endpoints)
          {
            if (se.Contract.Name != "IMetadataExchange")
            {
              //aggiorno l'endpoint
              ServiceContext.Current.DiscoverableEnpoints[ContractDescriptionsHelper.GetContractFullName(se)].Scopes = new List<string>(value).ToArray();
              //se.Contract.ContractType
            }
          }
        }
      }
    }
    #region IServiceBehavior Members
    /// <summary>
    /// Provides the ability to pass custom data to binding elements to support the contract implementation.
    /// </summary>
    /// <param name="serviceDescription">The service description of the service.</param>
    /// <param name="serviceHostBase">The host of the service.</param>
    /// <param name="endpoints">The service endpoints.</param>
    /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
    public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
    {
      return;
    }
    /// <summary>
    /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
    /// </summary>
    /// <param name="serviceDescription">The service description.</param>
    /// <param name="serviceHostBase">The host that is currently being built.</param>
    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
    {
      return;
    }
    /// <summary>
    /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
    /// </summary>
    /// <param name="serviceDescription">The service description.</param>
    /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
    public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
    {
      _serviceHostBase = serviceHostBase;
      ServiceContext.Current.AddService(serviceHostBase, Scopes, ScopesMatchBy);
      return;
    }
    #endregion
  }
}
