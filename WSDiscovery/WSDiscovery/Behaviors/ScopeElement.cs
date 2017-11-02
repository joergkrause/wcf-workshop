using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

/*****************************************************************************
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
//*****************************************************************************/
namespace Masieri.ServiceModel.WSDiscovery.Behaviors
{
  /// <summary>
  /// ScopeElement Class
  /// </summary>
  public class ScopeElement : ConfigurationElement
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeElement"/> class.
    /// </summary>
    public ScopeElement() { }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    /// <value>The URL.</value>
    [ConfigurationProperty("url", IsRequired = false, DefaultValue = " http://www.dotnetslackers.com")]
    public string Url
    {
      get { return (string)this["url"]; }
      set { this["url"] = value; }
    }
  }
}
