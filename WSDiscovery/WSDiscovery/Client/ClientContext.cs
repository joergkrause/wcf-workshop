using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Transport;
using System.Net;
using Masieri.ServiceModel.WSDiscovery.Messages;
using Masieri.ServiceModel.WSDiscovery.Helpers;
using System.Threading;

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
  /// Class singleton, contains ClientMemento
  /// </summary>
  internal class ClientContext
  {
    Dictionary<string, List<ClientMemento>> _discoveredEndpoints;
    ReaderWriterLockSlim _rwLock;
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientContext"/> class.
    /// </summary>
    private ClientContext()
    {
     _discoveredEndpoints = new Dictionary<string, List<ClientMemento>>();
      _rwLock = new ReaderWriterLockSlim();
    }

    /// <summary>
    /// Gets the list of ClientMementos
    /// </summary>
    /// <param name="contractFullName">Full name of the contract.</param>
    /// <returns></returns>
    public List<ClientMemento> GetList(string contractFullName)
    {

      try
      {
        _rwLock.EnterReadLock();

        List<ClientMemento> outList;
        if (string.IsNullOrEmpty(contractFullName))
        {
          var mems = from v in _discoveredEndpoints.Values from m in v select m;
          outList = new List<ClientMemento>(mems);
        }
        else if (_discoveredEndpoints.ContainsKey(contractFullName))
          outList = new List<ClientMemento>(_discoveredEndpoints[contractFullName]);
        else
          outList = new List<ClientMemento>();
        return outList;
      }
      finally
      {
        _rwLock.ExitReadLock();
      }

    }
    /// <summary>
    /// Adds the discovered endpoint.
    /// </summary>
    /// <param name="contractFullName">Full name of the contract.</param>
    /// <param name="mem">The mem.</param>
    public void AddDiscoveredEndpoint(string contractFullName, ClientMemento mem)
    {
      try
      {
        _rwLock.EnterWriteLock();
        if (!_discoveredEndpoints.ContainsKey(contractFullName))
          _discoveredEndpoints.Add(contractFullName, new List<ClientMemento>());
        //ora devo controllare che non sia gia presente lo stesso ClientMemento
        if (_discoveredEndpoints[contractFullName].Count(m => mem.GetUniqueIdentifier() == m.GetUniqueIdentifier()) == 0)
        {
          _discoveredEndpoints[contractFullName].Add(mem);
        }
      }
      finally
      {
        _rwLock.ExitWriteLock();
      }
    }
    /// <summary>
    /// Removes the discovered endpoint.
    /// </summary>
    /// <param name="contractFullName">Full name of the contract.</param>
    /// <param name="mem">The mem.</param>
    public void RemoveDiscoveredEndpoint(string contractFullName, ClientMemento mem)
    {
      try
      {
        _rwLock.EnterWriteLock();
        if (!_discoveredEndpoints.ContainsKey(contractFullName))
          _discoveredEndpoints.Add(contractFullName, new List<ClientMemento>());
        List<ClientMemento> listDeleting = new List<ClientMemento>();
        foreach (ClientMemento m in _discoveredEndpoints[contractFullName])
        {
          if (m.Address == mem.Address)
            listDeleting.Add(m);
        }
        foreach (ClientMemento m in listDeleting)
        {
          _discoveredEndpoints[contractFullName].Remove(m);
        }
      }
      finally
      {
        _rwLock.ExitWriteLock();
      }
    }
    /// <summary>
    /// Removes the discovered endpoint.
    /// </summary>
    /// <param name="epr">The EndpointReference fo the endpoint</param>
    public void RemoveDiscoveredEndpoint(EndpointReference epr)
    {
      List<ClientMemento> listDeleting;
      try
      {
        _rwLock.EnterReadLock();

        var mems = from ml in _discoveredEndpoints.Values from m in ml where m.Address == epr.Address select m;
        listDeleting = new List<ClientMemento>(mems.ToArray());
      }
      finally
      {
        _rwLock.ExitReadLock();
      }
      try
      {
        _rwLock.EnterWriteLock();
        foreach (ClientMemento m in listDeleting)
        {
          string contractFullName = ContractDescriptionsHelper.GetContractFullName(m.Endpoint);

          if (_discoveredEndpoints.ContainsKey(contractFullName))
            _discoveredEndpoints[contractFullName].Remove(m);
          if (_discoveredEndpoints.ContainsKey(contractFullName) &&
              _discoveredEndpoints[contractFullName].Count == 0)
            //I remove also the key 
            _discoveredEndpoints.Remove(contractFullName);
        }
      }
      finally
      {
        _rwLock.ExitWriteLock();
      }
    }
    #region SingleTon
    static ClientContext _instance;
    static object _lockObject = new object();
    /// <summary>
    /// Gets the current Instance
    /// </summary>
    /// <value>The current Instance.</value>
    public static ClientContext Current
    {
      get
      {
        if (_instance == null)
          lock (_lockObject)
          {
            if (_instance == null)
            {
              _instance = new ClientContext();
            }
          }
        return _instance;
      }
    }
    #endregion

  }
}
