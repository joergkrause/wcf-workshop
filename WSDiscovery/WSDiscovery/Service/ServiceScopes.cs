using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using Masieri.ServiceModel.WSDiscovery.Helpers;

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
  /// ServiceScopes class
  /// </summary>
  public class ServiceScopes
  {
    /// <summary>
    /// Gets or sets the scope match by.
    /// </summary>
    /// <value>The scope match by.</value>
    public string ScopeMatchBy { get; set; }
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    public ScopeList Scopes { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceScopes"/> class.
    /// </summary>
    public ServiceScopes()
    {
      ScopeMatchBy = Constants.ScopesMatchBy;
      Scopes = new ScopeList();
    }
    /// <summary>
    /// Gets the service scopes.
    /// </summary>
    /// <param name="se">The se.</param>
    /// <returns></returns>
    public static ServiceScopes GetServiceScopes(ServiceEndpoint se)
    {
      ServiceMemento sm = ServiceContext.Current.DiscoverableEnpoints[ContractDescriptionsHelper.GetContractFullName(se)];
      return sm.ServiceScope;
    }
  }    
}
