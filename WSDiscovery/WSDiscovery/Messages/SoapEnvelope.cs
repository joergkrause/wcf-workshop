using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.Net;
using System.Net.Sockets;

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
  /// SoapEnvelope class
  /// </summary>
  public class SoapEnvelope
  {
    List<SoapHeader> _headers = new List<SoapHeader>();
    MessageVersion _version = MessageVersion.Soap12WSAddressing10;
    /// <summary>
    /// Initializes a new instance of the <see cref="SoapEnvelope"/> class.
    /// </summary>
    /// <param name="version">The version.</param>
    public SoapEnvelope(MessageVersion version)
    {
      _version = version;
    }
    /// <summary>
    /// Gets the headers.
    /// </summary>
    /// <value>The headers.</value>
    public List<SoapHeader> Headers
    {
      get { return _headers; }
    }
    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="header">The header.</param>
    public void AddHeader(SoapHeader header)
    {
      //prm gia esiste lo rimuovo
      foreach (SoapHeader h in _headers)
      {
        if (h.Name == header.Name)
        {
          _headers.Remove(h);
        }
      }
      _headers.Add(header);
    }
    /// <summary>
    /// Gets or sets the content of the body.
    /// </summary>
    /// <value>The content of the body.</value>
    public object BodyContent { get; set; }
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      string soapNamespace = "";
      string addressingNamespace = "";

      if (_version == MessageVersion.Soap11)
      {
        soapNamespace = Constants.Soap.Namespace11;
        addressingNamespace = "";
      }
      else if (_version == MessageVersion.Soap11WSAddressing10)
      {
        soapNamespace = Constants.Soap.Namespace11;
        addressingNamespace = Constants.Addressing.Namespace10;
      }
      else if (_version == MessageVersion.Soap11WSAddressingAugust2004)
      {
        soapNamespace = Constants.Soap.Namespace11;
        addressingNamespace = Constants.Addressing.NamespaceAugust2004;
      }
      else if (_version == MessageVersion.Soap12)
      {
        soapNamespace = Constants.Soap.Namespace12;
        addressingNamespace = "";
      }
      else if (_version == MessageVersion.Soap12WSAddressing10)
      {
        soapNamespace = Constants.Soap.Namespace12;
        addressingNamespace = Constants.Addressing.Namespace10;
      }
      else if (_version == MessageVersion.Soap12WSAddressingAugust2004)
      {
        soapNamespace = Constants.Soap.Namespace12;
        addressingNamespace = Constants.Addressing.NamespaceAugust2004;
      }

      StringBuilder sb = new StringBuilder();
      sb.Append("<s:Envelope");
      sb.AppendFormat(" xmlns:a=\"{0}\"", addressingNamespace);
      sb.AppendFormat(" xmlns:d=\"{0}\"", Constants.NamespaceUri);
      sb.AppendFormat(" xmlns:s=\"{0}\"", soapNamespace);
      sb.Append(">");
      if (_headers.Count > 0)
      {
        sb.Append("<s:Header>");
        foreach (SoapHeader header in _headers)
        {
          sb.Append(header.ToString());
        }
        sb.Append("</s:Header>");
      }
      sb.Append("<s:Body>");
      //qui il contenuto del body
      if (BodyContent != null)
        sb.Append(BodyContent.ToString());
      sb.Append("</s:Body>");
      sb.Append("</s:Envelope>");
      return sb.ToString();
    }

  }
}
