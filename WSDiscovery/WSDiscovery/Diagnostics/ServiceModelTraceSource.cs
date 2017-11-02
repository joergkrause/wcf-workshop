using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
namespace Masieri.ServiceModel.WSDiscovery.Diagnostics
{
  /// <summary>
  /// TraceSource class for Discovery log output
  /// </summary>
  class ServiceModelTraceSource:TraceSource
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceModelTraceSource"/> class.
    /// </summary>
    public ServiceModelTraceSource()
      : base("System.ServiceModel")
    {

    }

  }
}
