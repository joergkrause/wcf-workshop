using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
  /// 
  /// </summary>
  static class UriExtension
  {
    /// <summary>
    /// Determines whether [is same URI] [the specified URI].
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="uri2">The uri2.</param>
    /// <returns>
    /// 	<c>true</c> if [is same URI] [the specified URI]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSameUri(this Uri uri, Uri uri2)
    {
      string[] segments = uri.ToString().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
      string[] segments2 = uri2.ToString().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
      if (segments2.Length != segments.Length)
        return false;
      for (int i = 0; i < segments.Length; i++)
        if (segments[i].ToLower() != segments2[i].ToLower())
          return false;
      return true;
    }
  }
}
