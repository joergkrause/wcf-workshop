using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
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
namespace Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.MessageEncoding
{
  /// <summary>
  /// BinaryMessageEncoding
  /// </summary>
  class BinaryMessageEncoding
  {
    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static BinaryMessageEncodingBindingElement ReadXml(XmlReader reader)
    {
      BinaryMessageEncodingBindingElement enc = new BinaryMessageEncodingBindingElement();
      if (reader.LocalName != "BinaryMessageEncodingBindingElement")
        reader.ReadStartElement("BinaryMessageEncodingBindingElement");
      do
      {
        if (!reader.IsStartElement() && reader.LocalName == "BinaryMessageEncodingBindingElement")
          break;
        else
        {
          if (reader.IsStartElement() && reader.LocalName == "MaxReadPoolSize")
          {
            enc.MaxReadPoolSize = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MaxWritePoolSize")
          {
            enc.MaxWritePoolSize = reader.ReadContentAsInt();
          }
          else if (reader.IsStartElement() && reader.LocalName == "MessageVersion")
          {
            string messageVersion = reader.ReadElementContentAsString();
            if (messageVersion.IndexOf("None") >= 0)
            {
              if (messageVersion.IndexOf("Soap11") >= 0)
              {
                enc.MessageVersion = MessageVersion.Soap11;
              }
              else
                enc.MessageVersion = MessageVersion.None;
            }
            else if (messageVersion.IndexOf("Soap11") >= 0)
            {
              if (messageVersion.IndexOf("Addressing") == -1)
                enc.MessageVersion = MessageVersion.Soap11;
              else if (messageVersion.IndexOf("Addressing10") >= 0)
                enc.MessageVersion = MessageVersion.Soap11WSAddressing10;
              else
                enc.MessageVersion = MessageVersion.Soap11WSAddressingAugust2004;
            }
            else if (messageVersion.IndexOf("Soap12") >= 0)
            {
              if (messageVersion.IndexOf("Addressing") == -1)
                enc.MessageVersion = MessageVersion.Soap12;
              else if (messageVersion.IndexOf("Addressing10") >= 0)
                enc.MessageVersion = MessageVersion.Soap12WSAddressing10;
              else
                enc.MessageVersion = MessageVersion.Soap12WSAddressingAugust2004;

            }
            else
              enc.MessageVersion = MessageVersion.Default;
          }
          else if (reader.IsStartElement() && reader.LocalName == "ReaderQuotas")
          {

            while (true)
            {
              if ((!reader.IsStartElement() || reader.IsEmptyElement) && reader.LocalName == "ReaderQuotas")
                break;
              else if (reader.IsStartElement() && reader.LocalName == "MaxArrayLength")
              {
                enc.ReaderQuotas.MaxArrayLength = reader.ReadElementContentAsInt();
              }
              else if (reader.IsStartElement() && reader.LocalName == "MaxBytesPerRead")
              {
                enc.ReaderQuotas.MaxBytesPerRead = reader.ReadElementContentAsInt();

              }
              else if (reader.IsStartElement() && reader.LocalName == "MaxDepth")
              {
                enc.ReaderQuotas.MaxDepth = reader.ReadElementContentAsInt();

              }
              else if (reader.IsStartElement() && reader.LocalName == "MaxNameTableCharCount")
              {
                enc.ReaderQuotas.MaxNameTableCharCount = reader.ReadElementContentAsInt();

              }
              else if (reader.IsStartElement() && reader.LocalName == "MaxStringContentLength")
              {
                enc.ReaderQuotas.MaxStringContentLength = reader.ReadElementContentAsInt();

              }
              else
                reader.Read();

            }

          }

        }
      }
      while (reader.Read());
      return enc;
    }
    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="enc">The enc.</param>
    /// <returns></returns>
    public static string WriteXml(BinaryMessageEncodingBindingElement enc)
    {
      StringBuilder sb = new StringBuilder();
      BinaryMessageEncodingBindingElement el = new BinaryMessageEncodingBindingElement();
      using (XmlTextWriter w = new XmlTextWriter(new StringWriter(sb)))
      {
        //Comprimere
        w.WriteStartElement("BinaryMessageEncodingBindingElement");
        if (enc.MaxReadPoolSize != el.MaxReadPoolSize)
          w.WriteElementString("MaxReadPoolSize", enc.MaxReadPoolSize.ToString());
        if (enc.MaxWritePoolSize != el.MaxWritePoolSize)
          w.WriteElementString("MaxWritePoolSize", enc.MaxWritePoolSize.ToString());
        if (enc.MessageVersion != el.MessageVersion)
          w.WriteElementString("MessageVersion", enc.MessageVersion.ToString());
        //Quotas
        w.WriteStartElement("ReaderQuotas");
        if (enc.ReaderQuotas.MaxArrayLength != el.ReaderQuotas.MaxArrayLength)
          w.WriteElementString("MaxArrayLength", enc.ReaderQuotas.MaxArrayLength.ToString());
        if (enc.ReaderQuotas.MaxBytesPerRead != el.ReaderQuotas.MaxBytesPerRead)
          w.WriteElementString("MaxBytesPerRead", enc.ReaderQuotas.MaxBytesPerRead.ToString());
        if (enc.ReaderQuotas.MaxDepth != el.ReaderQuotas.MaxDepth)
          w.WriteElementString("MaxDepth", enc.ReaderQuotas.MaxDepth.ToString());
        if (enc.ReaderQuotas.MaxNameTableCharCount != el.ReaderQuotas.MaxNameTableCharCount)
          w.WriteElementString("MaxNameTableCharCount", enc.ReaderQuotas.MaxNameTableCharCount.ToString());
        if (enc.ReaderQuotas.MaxStringContentLength != el.ReaderQuotas.MaxStringContentLength)
          w.WriteElementString("MaxStringContentLength", enc.ReaderQuotas.MaxStringContentLength.ToString());
        w.WriteEndElement();
        w.WriteEndElement();
      }
      return sb.ToString();
    }
  }
}
