using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Diagnostics;

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
namespace Masieri.ServiceModel.WSDiscovery
{
  /// <summary>
  /// Log4net like class to log
  /// </summary>
  class DiscoveryLogger
  {
    private static readonly ServiceModelTraceSource _tracesource = new ServiceModelTraceSource();
    private static readonly System.Diagnostics.TraceSource _discoveryTraceSource = new System.Diagnostics.TraceSource("Masieri.ServiceModel.WSDiscovery");
    static DiscoveryLogger()
    {


    }
    /// <summary>
    /// Debugs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Debug(object message, Exception exception)
    {
      _tracesource.TraceData(System.Diagnostics.TraceEventType.Verbose, 0, message, exception);
      _discoveryTraceSource.TraceData(System.Diagnostics.TraceEventType.Verbose, 0, message, exception);
    }
    /// <summary>
    /// Debugs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Debug(object message)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, Convert.ToString(message));
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, Convert.ToString(message));
    }
    /// <summary>
    /// Debugs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="para">The para.</param>
    public static void Debug(string message, params object[] para)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, message, para);
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, message, para);
    }
    /// <summary>
    /// Errors the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Error(object message, Exception exception)
    {
      _tracesource.TraceData(System.Diagnostics.TraceEventType.Error, 0, message, exception);
      _discoveryTraceSource.TraceData(System.Diagnostics.TraceEventType.Error, 0, message, exception);
    }
    /// <summary>
    /// Errors the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Error(object message)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Error, 0, Convert.ToString(message));
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Error, 0, Convert.ToString(message));
    }
    /// <summary>
    /// Fatals the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Fatal(object message, Exception exception)
    {
      _tracesource.TraceData(System.Diagnostics.TraceEventType.Critical, 0, message, exception);
      _discoveryTraceSource.TraceData(System.Diagnostics.TraceEventType.Critical, 0, message, exception);
    }
    /// <summary>
    /// Fatals the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Fatal(object message)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Critical, 0, Convert.ToString(message));
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Critical, 0, Convert.ToString(message));
    }
    /// <summary>
    /// Infoes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Info(object message, Exception exception)
    {
      _tracesource.TraceData(System.Diagnostics.TraceEventType.Information, 0, message, exception);
      _discoveryTraceSource.TraceData(System.Diagnostics.TraceEventType.Information, 0, message, exception);
    }
    /// <summary>
    /// Infoes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Info(object message)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, Convert.ToString(message));
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, Convert.ToString(message));
    }
    /// <summary>
    /// Infoes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="para">The para.</param>
    public static void Info(string message, params object[] para)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, message, para);
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, message, para);
    }
    /// <summary>
    /// Warns the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void Warn(object message, Exception exception)
    {
      _tracesource.TraceData(System.Diagnostics.TraceEventType.Warning, 0, message, exception);
      _discoveryTraceSource.TraceData(System.Diagnostics.TraceEventType.Warning, 0, message, exception);
    }
    /// <summary>
    /// Warns the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Warn(object message)
    {
      _tracesource.TraceEvent(System.Diagnostics.TraceEventType.Warning, 0, Convert.ToString(message));
      _discoveryTraceSource.TraceEvent(System.Diagnostics.TraceEventType.Warning, 0, Convert.ToString(message));
    }
  }
}
