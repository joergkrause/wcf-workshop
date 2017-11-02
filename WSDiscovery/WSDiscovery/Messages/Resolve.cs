using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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
  /// Resolve class
  /// </summary>
  class Resolve
  {
    /// <summary>
    /// Gets or sets the type of the endpoint reference.
    /// </summary>
    /// <value>The type of the endpoint reference.</value>
    public EndpointReference EndpointReferenceType { get; set; }
    /// <summary>
    /// Gets or sets the types.
    /// </summary>
    /// <value>The types.</value>
    public string Types { get; set; }
    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>The scopes.</value>
    public string Scopes { get; set; }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("<Resolve targetns=\"{0}\">", Constants.NamespaceUri);
      sb.Append(EndpointReferenceType.ToString());
      if (!string.IsNullOrEmpty(Types))
        sb.AppendFormat("<Types>{0}</Types>", Types);
      if (!string.IsNullOrEmpty(Scopes))
        sb.AppendFormat("<Scopes>{0}</Scopes>", Scopes);
      sb.Append("</Resolve>");
      return sb.ToString();
    }
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
        {
          this.EndpointReferenceType = EndpointReference.FromReader(reader);
        }
        if (reader.IsStartElement() && reader.LocalName == "Types")
          this.Types = reader.ReadElementContentAsString();
        if (reader.IsStartElement() && reader.LocalName == "Scopes")
          this.Scopes = reader.ReadElementContentAsString();
        if (!reader.IsStartElement() && reader.LocalName == "Resolve")
          return;
      }
    }

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
      //not used
      throw new NotImplementedException();
    }
    #endregion
  }
}
