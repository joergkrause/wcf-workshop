using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
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
  /// TransactionFlow
  /// </summary>
  class TransactionFlow
  {
    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static TransactionFlowBindingElement ReadXml(XmlReader reader)
    {
      TransactionFlowBindingElement tran = new TransactionFlowBindingElement();
      if (reader.LocalName != "TransactionFlowBindingElement")
        reader.ReadStartElement("TransactionFlowBindingElement");
      do
      {
        if (reader.IsEmptyElement && reader.LocalName == "TransactionFlowBindingElement")
          break;
        if (!reader.IsStartElement() && reader.LocalName == "TransactionFlowBindingElement")
          break;
        else
        {
          if (reader.IsStartElement() && reader.LocalName == "TransactionProtocol")
          {
            string content = reader.ReadContentAsString();
            switch (content)
            {
              case "OleTransactions": tran.TransactionProtocol = TransactionProtocol.OleTransactions; break;
              case "WSAtomicTransaction11": tran.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11; break;
              case "WSAtomicTransactionOctober2004": tran.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004; break;
              default:
                tran.TransactionProtocol = TransactionProtocol.Default; break;

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
    public static string WriteXml(TransactionFlowBindingElement enc)
    {
      StringBuilder sb = new StringBuilder();
      TransactionFlowBindingElement el = new TransactionFlowBindingElement();
      using (XmlTextWriter w = new XmlTextWriter(new StringWriter(sb)))
      {
        w.WriteStartElement("TransactionFlowBindingElement");
        if (enc.TransactionProtocol != el.TransactionProtocol)
        {
          string content = "Default";
          if (enc.TransactionProtocol == TransactionProtocol.OleTransactions)
            content = "OleTransactions";
          else if (enc.TransactionProtocol == TransactionProtocol.WSAtomicTransaction11)
            content = "WSAtomicTransaction11";
          else if (enc.TransactionProtocol == TransactionProtocol.WSAtomicTransactionOctober2004)
            content = "WSAtomicTransactionOctober2004";
          w.WriteElementString("TransactionProtocol", content);
        }
        w.WriteEndElement();
      }
      return sb.ToString();
    }
  }
}
