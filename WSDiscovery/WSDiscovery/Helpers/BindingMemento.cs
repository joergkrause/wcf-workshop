using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using System.ServiceModel.Description;
using Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.MessageEncoding;
using Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.Transport;
using Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.Protocol;

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
namespace Masieri.ServiceModel.WSDiscovery.Helpers
{

  /// <summary>
  /// BindingMemento is the class that serialize/Deserialize the Binding of the service
  /// </summary>
  class BindingMemento
  {
    private static void Serialize(Type tipo, object instance, ref StringBuilder sb)
    {
      sb.AppendFormat("<{0}>", tipo.Name);
      foreach (PropertyInfo prop in tipo.GetProperties())
      {
        //solo quelle interessanti

        if (prop.PropertyType.IsClass)
        {
          if (prop.PropertyType != tipo)
            Serialize(prop.PropertyType, prop.GetValue(instance, null), ref sb);
        }
        else
        {
          sb.AppendFormat("<{0}>", prop.Name);
          sb.Append(prop.GetValue(instance, null).ToString());
          sb.AppendFormat("</{0}>", prop.Name);

        }

      }
      sb.AppendFormat("</{0}>", tipo.Name);
    }
    /// <summary>
    /// Restores the memento.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static List<BindingElement> RestoreMemento(XmlReader reader)
    {
      List<BindingElement> bindings = new List<BindingElement>();
      //TODO:Estendere con altri binding elements
      while (reader.Read())
      {
        if ((!reader.IsStartElement() || reader.IsEmptyElement) && reader.LocalName == "Binding")
          break;
        else
        {
          if (reader.IsStartElement() && reader.LocalName == "HttpTransportBindingElement")
          {
            bindings.Add(HttpTransport.ReadXml(reader));
          }
          else if (reader.IsStartElement() && reader.LocalName == "HttpsTransportBindingElement")
          {
            bindings.Add(HttpsTransport.ReadXml(reader));

          }
          else if (reader.IsStartElement() && reader.LocalName == "TcpTransportBindingElement")
          {
            bindings.Add(NetTcpTransport.ReadXml(reader));

          }
          else if (reader.IsStartElement() && reader.LocalName == "TextMessageEncodingBindingElement")
          {
            bindings.Add(TextMessageEncoding.ReadXml(reader));

          }
          else if (reader.IsStartElement() && reader.LocalName == "BinaryMessageEncodingBindingElement")
          {
            bindings.Add(BinaryMessageEncoding.ReadXml(reader));

          }
          else if (reader.IsStartElement() && reader.LocalName == "TransactionFlowBindingElement")
          {
            bindings.Add(TransactionFlow.ReadXml(reader));

          }
          else if (reader.IsStartElement() && reader.LocalName == "WindowsStreamSecurityBindingElement")
          {
            bindings.Add(WindowsStreamSecurity.ReadXml(reader));

          }

        }
      }
      return bindings;
    }
    /// <summary>
    /// Gets the memento.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <returns></returns>
    public static string GetMemento(Binding binding)
    {
      List<string> errors = new List<string>();
      BindingElementCollection elements = binding.CreateBindingElements();
      StringBuilder sb = new StringBuilder();
      sb.Append("<Binding>");
      foreach (BindingElement el in elements)
      {
        if (el is TransportBindingElement)
        {
          if (el is TcpTransportBindingElement)
          {
            TcpTransportBindingElement tran = el as TcpTransportBindingElement;
            sb.Append(NetTcpTransport.WriteXml(tran));

          }
          else if (el is HttpTransportBindingElement)
          {
            HttpTransportBindingElement tran = el as HttpTransportBindingElement;
            sb.Append(HttpTransport.WriteXml(tran));
          }
          else if (el is HttpsTransportBindingElement)
          {
            HttpsTransportBindingElement tran = el as HttpsTransportBindingElement;
            sb.Append(HttpsTransport.WriteXml(tran));
          }
          //else if (el is MsmqTransportBindingElement)
          //{
          //    MsmqTransportBindingElement tran = el as MsmqTransportBindingElement;
          //}
          //else if (el is MsmqIntegrationBindingElement)
          //{
          //    MsmqTransportBindingElement tran = el as MsmqIntegrationBindingElement;
          //}
          //else if (el is NamedPipeTransportBindingElement)
          //{
          //    NamedPipeTransportBindingElement tran = el as NamedPipeTransportBindingElement;
          //}
          //else if (el is PeerTransportBindingElement)
          //{
          //    PeerTransportBindingElement tran = el as PeerTransportBindingElement;
          //}

          else
          {
            errors.Add("Transport unknown: " + el.GetType().ToString());
          }
        }
        else if (el is MessageEncodingBindingElement)
        {
          if (el is TextMessageEncodingBindingElement)
          {
            sb.Append(TextMessageEncoding.WriteXml((TextMessageEncodingBindingElement)el));

          }
          else if (el is BinaryMessageEncodingBindingElement)
          {
            sb.Append(BinaryMessageEncoding.WriteXml((BinaryMessageEncodingBindingElement)el));
          }
          //else if (el is MtomMessageEncodingBindingElement)
          //{ }
          else
          {
            errors.Add("MessageEncodingBindingElement unknown: " + el.GetType().ToString());
          }
        }
        else if (el is TransactionFlowBindingElement)
        {
          sb.Append(TransactionFlow.WriteXml((TransactionFlowBindingElement)el));
        }
        else if (el is WindowsStreamSecurityBindingElement)
        {
          sb.Append(WindowsStreamSecurity.WriteXml((WindowsStreamSecurityBindingElement)el));
        }

        //else if (el is SecurityBindingElement)
        //{
        //    if (el is AsymmetricSecurityBindingElement)
        //    { }
        //    else if (el is SymmetricSecurityBindingElement)
        //    { }
        //    else if (el is TransportSecurityBindingElement)
        //    { }
        //    else
        //    {
        //        errors.Add("SecurityBindingElement unknown:" + el.GetType().ToString());
        //    }
        //}
        //else if (el is ReliableSessionBindingElement)
        //{ 

        //}
        //else if (el is OneWayBindingElement)
        //{

        //}
        else
        {
          errors.Add("BindingElement unknown: " + el.GetType().ToString());
        }

      }
      sb.Append("</Binding>");
      return sb.ToString();
    }

  }
}
