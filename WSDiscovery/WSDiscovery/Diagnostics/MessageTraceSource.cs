using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Xml.XPath;
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
namespace Masieri.ServiceModel.WSDiscovery.Diagnostics
{
  /// <summary>
  /// Trace Source Class for Messages
  /// </summary>
  internal class MessageTraceSource : TraceSource
  {
    System.ServiceModel.Configuration.DiagnosticSection _section = null;
    public MessageTraceSource()
      : base("System.ServiceModel.MessageLogging")
    {
      object obj = null;
      obj = ConfigurationManager.GetSection("system.serviceModel/diagnostics");
      _section = obj as System.ServiceModel.Configuration.DiagnosticSection;
      if (_section == null)
        _section = new System.ServiceModel.Configuration.DiagnosticSection();
    }
    /// <summary>
    /// Writes the message at trasport level.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="source">The source.</param>
    public void WriteMessageAtTrasportLevel(string message, string source)
    {
      if (_section.MessageLogging.LogMessagesAtTransportLevel)
      {
        XPathDocument doc;
        using (XmlTextReader reader = new XmlTextReader(new StringReader(FormatMessage(message, source, _section.MessageLogging.LogEntireMessage))))
        {
          doc = new XPathDocument(reader);
          XPathNavigator nav = doc.CreateNavigator();
          base.TraceData(TraceEventType.Information, 0, nav);
        }
      }
    }

    /// <summary>
    /// Writes the malformed message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="source">The source.</param>
    public void WriteMalformedMessage(string message, string source)
    {
      if (_section.MessageLogging.LogMalformedMessages)
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<![CDATA[{0}]]>", message);
        XPathDocument doc;
        using (XmlTextReader reader = new XmlTextReader(new StringReader(FormatMessage(sb.ToString(), source, _section.MessageLogging.LogEntireMessage))))
        {
          doc = new XPathDocument(reader);
          XPathNavigator nav = doc.CreateNavigator();
          base.TraceData(TraceEventType.Error, 0, nav);
        }

      }
    }
    /// <summary>
    /// Formats the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="logEntireMessage">if set to <c>true</c> [log entire message].</param>
    /// <returns></returns>
    internal string FormatMessage(string message, string messageType, bool logEntireMessage)
    {
      string timeStamp = DateTime.Now.ToString("yyyy-dd-MMThh:mm:ss.fffffffzzz");
      return @"<MessageLogTraceRecord Time=""" + timeStamp +
        @""" Source=""WSDiscoveryMessage"" Type=""" + messageType + @""" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">" + ((logEntireMessage) ? message:"...") + @"
</MessageLogTraceRecord>";


    }
  }
}
