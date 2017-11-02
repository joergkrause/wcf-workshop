using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.Net.Security;
using System.Xml;
using System.IO;

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
namespace Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.Protocol
{
  /// <summary>
  /// WindowsStreamSecurity
  /// </summary>
  class WindowsStreamSecurity
  {
    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static WindowsStreamSecurityBindingElement ReadXml(XmlReader reader)
    {
      WindowsStreamSecurityBindingElement tran = new WindowsStreamSecurityBindingElement();
      if (reader.LocalName != "WindowsStreamSecurityBindingElement")
        reader.ReadStartElement("WindowsStreamSecurityBindingElement");
      do
      {
        if (reader.IsEmptyElement && reader.LocalName == "WindowsStreamSecurityBindingElement")
          break;
        if (!reader.IsStartElement() && reader.LocalName == "WindowsStreamSecurityBindingElement")
          break;
        else
        {
          if (reader.IsStartElement() && reader.LocalName == "ProtectionLevel")
          {
            string content = reader.ReadContentAsString();
            switch (content)
            {
              case "EncryptAndSign": tran.ProtectionLevel = ProtectionLevel.EncryptAndSign; break;
              case "Sign": tran.ProtectionLevel = ProtectionLevel.Sign; break;
              default:
                tran.ProtectionLevel = ProtectionLevel.None; break;

            }
          }

        }
      }
      while (reader.Read());
      return tran;
    }
    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="enc">The enc.</param>
    /// <returns></returns>
    public static string WriteXml(WindowsStreamSecurityBindingElement enc)
    {
      StringBuilder sb = new StringBuilder();
      WindowsStreamSecurityBindingElement el = new WindowsStreamSecurityBindingElement();
      using (XmlTextWriter w = new XmlTextWriter(new StringWriter(sb)))
      {
        w.WriteStartElement("WindowsStreamSecurityBindingElement");
        if (enc.ProtectionLevel != el.ProtectionLevel)
        {
          string content = "None";
          if (enc.ProtectionLevel == ProtectionLevel.EncryptAndSign)
            content = "EncryptAndSign";
          else if (enc.ProtectionLevel == ProtectionLevel.Sign)
            content = "Sign";

          w.WriteElementString("ProtectionLevel", content);
        }
        w.WriteEndElement();
      }
      return sb.ToString();
    }
  }
}
