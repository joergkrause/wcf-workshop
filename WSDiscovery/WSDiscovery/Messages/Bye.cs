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
  /// Bye class messege
  /// </summary>
  class Bye
  {
    /// <summary>
    /// Gets or sets the endpoint reference value.
    /// </summary>
    /// <value>The endpoint reference value.</value>
    public EndpointReference EndpointReferenceValue { get; set; }

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


        if (!reader.IsStartElement() && reader.LocalName == "Bye")
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
      //Not used
      throw new NotImplementedException();
    }

    #endregion
  }
}
