using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel.Description;
using Masieri.ServiceModel.WSDiscovery.Helpers;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Masieri.ServiceModel.WSDiscovery.Messages;

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
namespace Masieri.ServiceModel.WSDiscovery.Client
{
  /// <summary>
  /// Memento of a service
  /// </summary>
  public class ClientMemento
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientMemento"/> class.
    /// </summary>
    public ClientMemento()
    {
      ScopeMatchBy = Constants.ScopesMatchBy;
      Scopes = new List<string>();
      _mexWaiting = new System.Threading.ManualResetEvent(false);
    }


    //endpoint recuperato con il mex e mantenuto per evitare duplicazioni di richiesta
    private ServiceEndpoint _endpoint = null;
    System.Threading.ManualResetEvent _mexWaiting;

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
    public string ScopeMatchBy { get; set; }
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    public List<string> Scopes { get; set; }
    string _XAddrs;
    /// <summary>
    /// Gets or sets the Metadata Address
    /// </summary>
    /// <value>The Metadata Address</value>
    public string XAddrs
    {
      get { return _XAddrs; }
      set
      {
        _XAddrs = value;
        if (_endpoint == null)
        { GetMetadata(); }
      }

    }
    /// <summary>
    /// Gets or sets the endpoint.
    /// </summary>
    /// <value>The endpoint.</value>
    public ServiceEndpoint Endpoint
    {
      get
      {
        if (_endpoint == null)
        {
          _mexWaiting.WaitOne(5000, true);
          return _endpoint;
        }
        else
          return _endpoint;
      }
      set
      {
        if (value != null)
          _mexWaiting.Set();
        _endpoint = value;
      }
    }
    /// <summary>
    /// Gets or sets the metadata version.
    /// </summary>
    /// <value>The metadata version.</value>
    public long MetadataVersion { get; set; }
    
    /// <summary>
    /// Get a string with a unique Hashcode for the memento
    /// </summary>
    /// <returns>string unique identifier for memento</returns>
    public  string GetUniqueIdentifier()
    {
      //I sort the scopes befor join it
      Scopes.Sort();
      return string.Format("{0}|{1}|{2}|{3}", this.Address, this.Type, ScopeMatchBy, string.Join(",", Scopes.ToArray())).ToLower();
    }
    private void GetMetadata()
    {
      Thread threadMex = new Thread(new ThreadStart(delegate()
      {
        MetadataExchangeClient resolver;
        //return if there isn't Metadata Address
        if (string.IsNullOrEmpty(_XAddrs))
          return;
        resolver = new MetadataExchangeClient(new Uri(_XAddrs), MetadataExchangeClientMode.HttpGet);
        MetadataSet metadata;
        try
        {
          DiscoveryLogger.Debug("GetMetadata from address: " + _XAddrs);
          metadata = resolver.GetMetadata();
          DiscoveryLogger.Info(String.Format("GetMetadata from {0} works correctly", _XAddrs));
        }
        catch (Exception ex)
        {
          DiscoveryLogger.Error(String.Format("Can't connect to metadata endpoint {0}", _XAddrs), ex);
          throw new Exception(String.Format("Can't connect to metadata endpoint {0}", _XAddrs));
        }
        WsdlImporter importer = new WsdlImporter(metadata);
        ServiceEndpointCollection serviceCollection = importer.ImportAllEndpoints();
        if (importer.Errors.Count > 0)
        {
          foreach (MetadataConversionError err in importer.Errors)
          {
            DiscoveryLogger.Warn("Exception during metatada import: " + err.Message);
          }
        }
        string contractName = Type.Substring(Type.LastIndexOf("/") + 1, Type.Length - Type.LastIndexOf("/") - 1);
        string contractNamespace = Type.Substring(0, Type.LastIndexOf("/") + 1);
        foreach (ServiceEndpoint se in serviceCollection)
        {

          if (se.Contract.Name == contractName && se.Contract.Namespace == contractNamespace)
          {
            Endpoint = se;
            break;
          }
        }
      }));
      threadMex.IsBackground = true;
      threadMex.Start();
    }
    /// <summary>
    /// Determines whether the specified scope contains scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="scopeMatchBy">The scope match by.</param>
    /// <returns>
    /// 	<c>true</c> if the specified scope contains scope; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsScope(string scope, string scopeMatchBy)
    {
      if (this.ScopeMatchBy != scopeMatchBy)
        return false;
      foreach (string s in Scopes)
      {
        if (new Uri(scope).IsSameUri(new Uri(s)))
          return true;
      }
      return false;
    }
    /// <summary>
    /// Sets the endpoint from endpoint reference value.
    /// </summary>
    /// <param name="epr">The epr.</param>
    /// <param name="contractType">Type of the contract.</param>
    /// <returns>true if operation successful</returns>
    internal bool SetEndpointFromEndpointReferenceValue(EndpointReference epr, Type contractType)
    {
      if (epr.ReferenceParameters != null && epr.ReferenceParameters is CustomBinding)
      {
        //devo costruire l'endpoint
        Binding binding = epr.ReferenceParameters as CustomBinding;
        EndpointAddress address = new EndpointAddress(epr.Address);
        this._endpoint = new ServiceEndpoint(ContractDescription.GetContract(contractType), binding, address);
        return true;
      }
      else
        return false;
    }
    /// <summary>
    /// Get the ClientMemento form the ProbeMatch.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <param name="pm">The pm.</param>
    /// <returns></returns>
    internal static ClientMemento FromProbeMatch<TChannel>(Masieri.ServiceModel.WSDiscovery.Messages.ProbeMatches.ProbeMatch pm)
    {
      ClientMemento mem = new ClientMemento();
      mem.Type = pm.Types;
      mem.Scopes.AddRange(pm.Scopes.Split(new string[] { Constants.ScopesSeparator }, StringSplitOptions.RemoveEmptyEntries));
      mem.ScopeMatchBy = (string.IsNullOrEmpty(pm.ScopeMatchBy)) ? Constants.ScopesMatchBy : pm.ScopeMatchBy;
      mem.Address = pm.EndpointReferenceValue.Address;
      mem.MetadataVersion = Convert.ToInt64(pm.MetadataVersion);
      mem.SetEndpointFromEndpointReferenceValue(pm.EndpointReferenceValue, typeof(TChannel));
      mem.XAddrs = pm.XAddrs;
      return mem;
    }
  }
}
