using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Service;

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
  /// 
  /// </summary>
  class ResolveMatches
  {
    /// <summary>
    /// Gets or sets the resolve match value.
    /// </summary>
    /// <value>The resolve match value.</value>
    public ResolveMatch ResolveMatchValue { get; set; }

    /// <summary>
    /// ResolveMatch class
    /// </summary>
    public class ResolveMatch
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
      sb.AppendFormat("<ResolveMatches targetns=\"{0}\">", Constants.NamespaceUri);
      sb.Append("<ResolveMatch>");
      sb.Append(this.ResolveMatchValue.EndpointReferenceValue.ToString());
      if (ResolveMatchValue.Types != null)
        sb.AppendFormat("<Types>{0}</Types>", ResolveMatchValue.Types);
      if (ResolveMatchValue.Scopes != null)
        sb.AppendFormat("<Scopes>{0}</Scopes>", ResolveMatchValue.Scopes);
      sb.AppendFormat("<XAddrs>{0}</XAddrs>", ResolveMatchValue.XAddrs);
      sb.AppendFormat("<MetadataVersion>{0}</MetadataVersion>", ServiceContext.Current.MetadataVersion);
      sb.Append("</ResolveMatch>");
      sb.Append("</ResolveMatches>");
      return base.ToString();
    }

    /// <summary>
    /// Froms the reader.
    /// </summary>
    /// <param name="reader">The XML dictionary reader.</param>
    /// <returns></returns>
    public static ResolveMatches FromReader(System.Xml.XmlDictionaryReader reader)
    {
      ResolveMatches rms = new ResolveMatches();
      rms.ResolveMatchValue = new ResolveMatch();
      ResolveMatch rm = rms.ResolveMatchValue;
      do
      {
        if (reader.IsStartElement() && reader.LocalName == "ResolveMatches")
          while (reader.Read())
          {
            if (reader.IsStartElement() && reader.LocalName == "ResolveMatch")
            {
              while (reader.Read())
              {
                if (reader.IsStartElement() && reader.LocalName == "EndpointReference")
                {
                  rm.EndpointReferenceValue = EndpointReference.FromReader(reader);
                }
                if (reader.IsStartElement() && reader.LocalName == "Types")
                  rm.Types = reader.ReadElementContentAsString();
                if (reader.IsStartElement() && reader.LocalName == "XAddrs")
                  rm.XAddrs = reader.ReadElementContentAsString();
                if (reader.IsStartElement() && reader.LocalName == "Scopes")
                {
                  if (reader.HasAttributes)
                    rm.ScopeMatchBy = reader.GetAttribute("MatchBy");
                  rm.Scopes = reader.ReadElementContentAsString();
                }
                if (reader.IsStartElement() && reader.LocalName == "MetadataVersion")
                  rm.MetadataVersion = reader.ReadElementContentAsString();
                if (!reader.IsStartElement() && reader.LocalName == "ResolveMatch")
                {
                  return rms;
                }

              }
            }
          }
      } while (reader.Read());
      return null;
    }
  }
}
