using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

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
namespace Masieri.ServiceModel.WSDiscovery.Helpers
{
  /// <summary>
  /// Class Helper for ContractDescriptions
  /// </summary>
  public class ContractDescriptionsHelper
  {
    /// <summary>
    /// Gets the full name of the contract.
    /// </summary>
    /// <param name="se">The se.</param>
    /// <returns></returns>
    public static string GetContractFullName(ServiceEndpoint se)
    {
      return GetContractFullName(se.Contract);
    }
    /// <summary>
    /// Gets the full name of the contract.
    /// </summary>
    /// <param name="contract">The contract.</param>
    /// <returns></returns>
    public static string GetContractFullName(ContractDescription contract)
    {
      string fullContractName = contract.Namespace;
      if (!fullContractName.EndsWith("/"))
        fullContractName = fullContractName + "/";
      fullContractName = fullContractName + contract.Name;
      return fullContractName;
    }
    /// <summary>
    /// Gets the full name of the contract.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <returns></returns>
    public static string GetContractFullName<TChannel>()
    {
      return GetContractFullName(ContractDescription.GetContract(typeof(TChannel)));
    }
    /// <summary>
    /// Get ContractDescription Froms the full name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static ContractDescription FromFullName(string name)
    {
      string contractName = name.Substring(name.LastIndexOf("/") + 1, name.Length - name.LastIndexOf("/") - 1);
      string contractNamespace = name.Substring(0, name.LastIndexOf("/") + 1);
      ContractDescription cd = new ContractDescription(contractName, contractNamespace);
      return cd;
    }
  }
}
