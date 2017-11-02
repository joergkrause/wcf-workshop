using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Reflection;
using System.Globalization;

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
  /// Helper for ServiceContract
  /// </summary>
  static class ServiceContractHelper
  {
    /// <summary>
    /// Gets the physical type from logical one.
    /// </summary>
    /// <param name="logicalType">Type of the logical.</param>
    /// <returns></returns>
    public static Type GetPhysicalFromLogical(string logicalType)
    {

      try
      {
        if (Assembly.GetEntryAssembly() == null)
          return null;
        //devo risolvere il type fisico da quello logico
        var physicTypes = from pt in Assembly.GetEntryAssembly().GetTypes()
                          from a in pt.GetCustomAttributes(false)
                          where pt.IsInterface &&
                                a is ServiceContractAttribute &&
                                CompareLogicalNames(logicalType, ((ServiceContractAttribute)a), pt)
                          select pt;
        if (physicTypes.Count() == 0)
        {
          //provo ad estendere la ricerca
          physicTypes = from assName in Assembly.GetEntryAssembly().GetReferencedAssemblies()
                        from pt in Assembly.Load(assName).GetTypes()
                        from a in pt.GetCustomAttributes(false)
                        where pt.IsInterface &&
                              a is ServiceContractAttribute &&
                              CompareLogicalNames(logicalType, ((ServiceContractAttribute)a), pt)
                        select pt;
          ;
        }
        return physicTypes.FirstOrDefault();
      }
      catch (Exception)
      {

        return null;
      }
    }
    private static bool CompareLogicalNames(string logicalType, ServiceContractAttribute sca, Type pt)
    {
      string ns = "http://tempuri.org/";
      if (!string.IsNullOrEmpty(sca.Namespace))
        ns =(!sca.Namespace.EndsWith("/"))? sca.Namespace + "/":sca.Namespace;
      string name = string.IsNullOrEmpty(sca.Name) ? pt.Name : sca.Name;
      string fullName = ns + name;
      return fullName.ToLower() == logicalType.Trim().ToLower() ? true : false;
    }
  }
}
