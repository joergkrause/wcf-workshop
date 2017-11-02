using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ServiceModel.Configuration;

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
  /// DiscoveryBehaviorSection class
  /// </summary>
  public class DiscoveryBehaviorSection : System.ServiceModel.Configuration.BehaviorExtensionElement
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveryBehaviorSection"/> class.
    /// </summary>
    public DiscoveryBehaviorSection()
    {

    }
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    [ConfigurationProperty("scopes", DefaultValue = "", IsRequired = true)]
    [ConfigurationCollectionAttribute(typeof(ScopesCollection), AddItemName = "addScope")]
    public ScopesCollection Scopes
    {
      get { return (ScopesCollection)base["scopes"]; }
      set { base["scopes"] = value; }
    }

    /// <summary>
    /// Notice to add to the service
    /// WSDL and XSD
    /// </summary>
    [ConfigurationProperty("scopesMatchBy", DefaultValue = "", IsRequired = true)]
    public string ScopesMatchBy
    {
      get { return (string)base["scopesMatchBy"]; }
      set { base["scopesMatchBy"] = value; }
    }
    /// <summary>
    /// Return the type of the behavior we configure
    /// </summary>
    public override Type BehaviorType
    {
      get { return typeof(ServiceDiscoverableBehavior); }
    }
    /// <summary>
    /// Create an instance of the behavior
    /// we represent
    /// </summary>
    /// <returns>The WsdlNoticeBehavior instance</returns>
    protected override object CreateBehavior()
    {
      List<string> scopes = new List<string>();
      foreach (ScopeElement el in this.Scopes)
      {
        scopes.Add(el.Url.ToString());
      }
      return new ServiceDiscoverableBehavior() { ScopesMatchBy = ScopesMatchBy, Scopes = scopes.ToArray() };
    }
    private ConfigurationPropertyCollection _properties;

    /// <summary>
    /// Return a collection of all our properties
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> of properties for the element.
    /// </returns>
    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (_properties == null)
        {
          _properties = new ConfigurationPropertyCollection();
          _properties.Add(new ConfigurationProperty(
             "scopesMatchBy", typeof(string), "",
             ConfigurationPropertyOptions.None
             ));
          _properties.Add(new ConfigurationProperty(
             "scopes", typeof(ScopesCollection), null,
             ConfigurationPropertyOptions.None
             ));
        }
        return _properties;
      }
    }

    /// <summary>
    /// Copy the information of another element into
    /// ourselves
    /// </summary>
    /// <param name="from">The element from which to copy</param>
    public override void CopyFrom(ServiceModelExtensionElement from)
    {
      base.CopyFrom(from);
      DiscoveryBehaviorSection element = (DiscoveryBehaviorSection)from;
      ScopesMatchBy = element.ScopesMatchBy;
      Scopes = element.Scopes;
    }
  }
}
