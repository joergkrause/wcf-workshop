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
namespace Masieri.ServiceModel.WSDiscovery.Messages
{
  /// <summary>
  /// Hello message class
  /// </summary>
  class Hello
  {
    /// <summary>
    /// Gets or sets the endpoint reference value.
    /// </summary>
    /// <value>The endpoint reference value.</value>
    public EndpointReference EndpointReferenceValue { get; set; }
    /// <summary>
    /// Gets or sets the types.
    /// </summary>
    /// <value>The types.</value>
    public string Types { get; set; }
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public string[] Type
    {
      get { return (string.IsNullOrEmpty(Types)) ? null : Types.Split(new string[] { Constants.TypesSeparator }, StringSplitOptions.RemoveEmptyEntries); }
    }
    /// <summary>
    /// Gets or sets the scope match by.
    /// </summary>
    /// <value>The scope match by.</value>
    public string ScopeMatchBy { get; set; }
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    public string Scopes { get; set; }
    /// <summary>
    /// Gets or sets the X addr.
    /// </summary>
    /// <value>The X addr.</value>
    public string XAddrs { get; set; }
    /// <summary>
    /// Gets or sets the metadata version.
    /// </summary>
    /// <value>The metadata version.</value>
    public string MetadataVersion { get; set; }

    #region IXmlSerializable Members

    /// <summary>
    /// Gets the schema.
    /// </summary>
    /// <returns></returns>
    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    public void ReadXml(System.Xml.XmlReader reader)
    {
      while (reader.Read())
      {
        if (reader.IsStartElement() && reader.LocalName == "EndpointReference")
          this.EndpointReferenceValue = EndpointReference.FromReader(reader);

        if (reader.IsStartElement() && reader.LocalName == "Types")
          this.Types = reader.ReadElementContentAsString();
        if (reader.IsStartElement() && reader.LocalName == "XAddrs")
          this.XAddrs = reader.ReadElementContentAsString();
        if (reader.IsStartElement() && reader.LocalName == "Scopes")
        {
          if (reader.HasAttributes)
            this.ScopeMatchBy = reader.GetAttribute("MatchBy");
          this.Scopes = reader.ReadElementContentAsString();
        }
        if (reader.IsStartElement() && reader.LocalName == "MetadataVersion")
          this.MetadataVersion = reader.ReadElementContentAsString();
        if (!reader.IsStartElement() && reader.LocalName == "Hello")
        {
          return;
        }
      }
    }

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
