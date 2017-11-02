using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Activation;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
namespace Masieri.ServiceModel.WSDiscovery
{
  /// <summary>
  /// DiscoveryOperationContextScope class: use it to add Custom Headers to the message
  /// </summary>
  /// <typeparam name="TChannel">The type of the channel.</typeparam>
  public class DiscoveryOperationContextScope<TChannel> : IDisposable where TChannel : class
  {
    /// <summary>
    /// Type enum to know if the channel is created or not
    /// </summary>
    public enum ChannelTypeEnum
    {
      /// <summary>
      /// Real Channel
      /// </summary>
      Real,
      /// <summary>
      /// Proxy Channel (No OperationContextScope Exists)
      /// </summary>
      Proxy
    }
    /// <summary>
    /// OutgoingMessageHeaders: Message Header List Class To add to the <see cref="DiscoveryOperationContextScope&lt;TChannel&gt;"/> class.
    /// </summary>
    public class OutgoingMessageHeaders : List<MessageHeader>
    {
      DiscoveryOperationContextScope<TChannel> _scope;
      /// <summary>
      /// Initializes a new instance of the <see cref="DiscoveryOperationContextScope&lt;TChannel&gt;.OutgoingMessageHeaders"/> class.
      /// </summary>
      /// <param name="scope">The scope.</param>
      internal OutgoingMessageHeaders(DiscoveryOperationContextScope<TChannel> scope)
      {
        _scope = scope;
      }
      /// <summary>
      /// Adds the specified header.
      /// </summary>
      /// <param name="header">The header.</param>
      public new void Add(MessageHeader header)
      {
        if (_scope.ChannelType == ChannelTypeEnum.Proxy)
          base.Add(header);
        else
          OperationContext.Current.OutgoingMessageHeaders.Add(header);
      }
      Uri _to;
      /// <summary>
      /// Gets or sets to.
      /// </summary>
      /// <value>To.</value>
      public Uri To
      {
        get { return _to; }
        set
        {
          _to = value;
          if (_scope.ChannelType == ChannelTypeEnum.Real)
            OperationContext.Current.OutgoingMessageHeaders.To = value;
        }
      }
      string _action;
      /// <summary>
      /// Gets or sets the action.
      /// </summary>
      /// <value>The action.</value>
      public string Action
      {
        get { return _action; }
        set
        {
          _action = value;
          if (_scope.ChannelType == ChannelTypeEnum.Real)
            OperationContext.Current.OutgoingMessageHeaders.Action = value;
        }
      }
      UniqueId _messageId;
      /// <summary>
      /// Gets or sets the message id.
      /// </summary>
      /// <value>The message id.</value>
      public UniqueId MessageId
      {
        get { return _messageId; }
        set
        {
          _messageId = value;
          if (_scope.ChannelType == ChannelTypeEnum.Real)
            OperationContext.Current.OutgoingMessageHeaders.MessageId = value;
        }
      }
      UniqueId _relatesTo;
      /// <summary>
      /// Gets or sets the relates to.
      /// </summary>
      /// <value>The relates to.</value>
      public UniqueId RelatesTo
      {
        get { return _relatesTo; }
        set
        {
          _relatesTo = value;
          if (_scope.ChannelType == ChannelTypeEnum.Real)
            OperationContext.Current.OutgoingMessageHeaders.RelatesTo = value;
        }
      }

    }
    bool _disposed = false;
    IDynamicProxy _proxyChannel;
    DiscoveryClient<TChannel> _client;
    OperationContextScope _realScope = null;
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveryOperationContextScope&lt;TChannel&gt;"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    public DiscoveryOperationContextScope(DiscoveryClient<TChannel> client)
    {
      _outgoingMessageHeaders = new OutgoingMessageHeaders(this);
      IDynamicProxy proxyChannel = client.Channel as IDynamicProxy;
      _client = client;
      if (proxyChannel != null)
      {
        //The channel isn't created
        client.Invoking += new EventHandler(proxyChannel_InvokingHanlder);
        _proxyChannel = proxyChannel;

      }
      else
        _realScope = new OperationContextScope((IContextChannel)client.Channel);
    }
    OutgoingMessageHeaders _outgoingMessageHeaders;

    /// <summary>
    /// Gets the type of the channel.
    /// </summary>
    /// <value>The type of the channel.</value>
    public ChannelTypeEnum ChannelType
    {
      get { return _proxyChannel == null ? ChannelTypeEnum.Real : ChannelTypeEnum.Proxy; }
    }
    /// <summary>
    /// Gets the outgoing headers.
    /// </summary>
    /// <value>The outgoing headers.</value>
    public OutgoingMessageHeaders OutgoingHeaders
    {
      get { return _outgoingMessageHeaders; }
    }
    void proxyChannel_InvokingHanlder(object sender, EventArgs e)
    {
      if (this.ChannelType == DiscoveryOperationContextScope<TChannel>.ChannelTypeEnum.Proxy)
      {
        //Now I have to create the channel and the real OperationContextScope
        if (_realScope == null)
        {
          _realScope = new OperationContextScope((IContextChannel)_client.Channel);
          //I add all the storaged headers
          foreach (MessageHeader header in _outgoingMessageHeaders)
          {
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

          }
          if (_outgoingMessageHeaders.Action != null)
            OperationContext.Current.OutgoingMessageHeaders.Action = _outgoingMessageHeaders.Action;
          if (_outgoingMessageHeaders.MessageId != null)
            OperationContext.Current.OutgoingMessageHeaders.MessageId = _outgoingMessageHeaders.MessageId;
          if (_outgoingMessageHeaders.To != null)
            OperationContext.Current.OutgoingMessageHeaders.To = _outgoingMessageHeaders.To;
          if (_outgoingMessageHeaders.RelatesTo != null)
            OperationContext.Current.OutgoingMessageHeaders.RelatesTo = _outgoingMessageHeaders.RelatesTo;
        }
      }

    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (!_disposed)
      {
        _client = null;
        _proxyChannel = null;
        if (_realScope != null)
          _realScope.Dispose();
        _disposed = true;
      }
    }

    #endregion
  }
}
