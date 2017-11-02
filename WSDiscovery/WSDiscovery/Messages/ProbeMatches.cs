using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Service;
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
  /// ProbeMatches
  /// </summary>
  class ProbeMatches
  {
    /// <summary>
    /// Gets or sets the probe match value.
    /// </summary>
    /// <value>The probe match value.</value>
    public ProbeMatch ProbeMatchValue { get; set; }

    /// <summary>
    /// ProbeMatch
    /// </summary>
    public class ProbeMatch
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
    }
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("<ProbeMatches targetns=\"{0}\">", Constants.NamespaceUri);
      if (ProbeMatchValue != null)
      {
        sb.Append("<ProbeMatch>");
        sb.Append(this.ProbeMatchValue.EndpointReferenceValue.ToString());
        if (ProbeMatchValue.Types != null)
          sb.AppendFormat("<Types>{0}</Types>", ProbeMatchValue.Types);
        if (ProbeMatchValue.XAddrs != null)
          sb.AppendFormat("<XAddrs>{0}</XAddrs>", ProbeMatchValue.XAddrs);
        if (ProbeMatchValue.Scopes != null)
        {
          if (string.IsNullOrEmpty(ProbeMatchValue.ScopeMatchBy))
            sb.AppendFormat("<Scopes>{0}</Scopes>", ProbeMatchValue.Scopes);
          else
            sb.AppendFormat("<Scopes MatchBy=\"{1}\">{0}</Scopes>", ProbeMatchValue.Scopes, ProbeMatchValue.ScopeMatchBy);
        }
        sb.AppendFormat("<MetadataVersion>{0}</MetadataVersion>", ProbeMatchValue.MetadataVersion);
        sb.Append("</ProbeMatch>");
      }
      sb.Append("</ProbeMatches>");
      return sb.ToString();
    }
    /// <summary>
    /// ProbeMatches Froms the XmlDictionaryReader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static ProbeMatches FromReader(XmlDictionaryReader reader)
    {
      ProbeMatches pms = new ProbeMatches();
      pms.ProbeMatchValue = new ProbeMatch();
      ProbeMatch pm = pms.ProbeMatchValue;
      do
      {
        if (reader.IsStartElement() && reader.LocalName == "ProbeMatches")
          while (reader.Read())
          {
            if (reader.IsStartElement() && reader.LocalName == "ProbeMatch")
            {
              while (reader.Read())
              {
                if (reader.IsStartElement() && reader.LocalName == "EndpointReference")
                {
                  pm.EndpointReferenceValue = EndpointReference.FromReader(reader);
                }
                if (reader.IsStartElement() && reader.LocalName == "Types")
                  pm.Types = reader.ReadElementContentAsString();
                if (reader.IsStartElement() && reader.LocalName == "XAddrs")
                  pm.XAddrs = reader.ReadElementContentAsString();
                if (reader.IsStartElement() && reader.LocalName == "Scopes")
                {
                  if (reader.HasAttributes)
                    pm.ScopeMatchBy = reader.GetAttribute("MatchBy");
                  pm.Scopes = reader.ReadElementContentAsString();
                }
                if (reader.IsStartElement() && reader.LocalName == "MetadataVersion")
                  pm.MetadataVersion = reader.ReadElementContentAsString();
                if (!reader.IsStartElement() && reader.LocalName == "ProbeMatch")
                {
                  return pms;
                }

              }
            }
          }
      } while (reader.Read());
      return null;
    }
  }
}
