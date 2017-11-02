using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Masieri.ServiceModel.WSDiscovery.Activation
{
    /// <summary>
    /// The implementation for a dynamic proxy. Should not be instantiated directly, but rather through the
    /// DynamicProxyFactory
    /// </summary>
    class DynamicProxyImpl : RealProxy, IDynamicProxy, IRemotingTypeInfo
    {
        private Guid _instanceID = Guid.NewGuid();
        /// <summary>
        /// The object we are the proxy for
        /// </summary>
        private object _proxyTarget;
        /// <summary>
        /// Should we be _strict regarding interface support?
        /// </summary>
        private bool _strict;
        /// <summary>
        /// A list of the types we support. Is only used when _strict is true. The proxy targets type(s) are automatically supported
        /// </summary>
        private Type[] _supportedTypes;
        /// <summary>
        /// The delegate for handling the invocation part of the method call process
        /// </summary>
        private InvocationDelegate _invocationHandler;

        /// <summary>
        /// Creates a new proxy instance, with _proxyTarget as the proxied object
        /// </summary>
        /// <param name="_proxyTarget">The object to proxy</param>
        /// <param name="_invocationHandler">The invocation handler</param>
        /// <param name="_strict">Should type support (for casts) be _strict or loose</param>
        /// <param name="_supportedTypes">A List of supported types. Only used if _strict is true. May be null</param>
        protected internal DynamicProxyImpl(object proxyTarget, InvocationDelegate invocationHandler, bool strict, Type[] supportedTypes)
            : base(typeof(IDynamicProxy))
        {
            this._proxyTarget = proxyTarget;
            this._invocationHandler = invocationHandler;
            this._strict = strict;
            this._supportedTypes = supportedTypes;
        }

        public Guid InstanceID
        { get { return _instanceID; } }
        /// <summary>
        /// CreateObjRef() isn't supported.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Nothing</returns>
        /// <exception cref="NotSupportedException">CreateObjRef() for DynamicProxy isn't supported</exception>
        public override ObjRef CreateObjRef(Type type)
        {
            throw new NotSupportedException("ObjRef for DynamicProxy isn't supported");
        }

        /// <summary>
        /// Checks whether the proxy representing the specified object type can be cast to the type represented by the IRemotingTypeInfo interface
        /// </summary>
        /// <param name="toType">The Type we wish to cast to</param>
        /// <param name="obj">The object we wish to cast</param>
        /// <returns>True if the _strict property is false, otherwise the list of _supportedTypes is checked.<br>
        /// The proxy targets type(s) are automatically supported</br></returns>
        public bool CanCastTo(Type toType, object obj)
        {
            // Assume we can (which is the default unless _strict is true)
            bool canCast = true;

            if (_strict)
            {
                // First check if the _proxyTarget supports the cast
                if (toType.IsAssignableFrom(_proxyTarget.GetType()))
                {
                    canCast = true;
                }
                else if (_supportedTypes != null)
                {
                    canCast = false;
                    // Check if the list of supported interfaces supports the cast
                    foreach (Type type in _supportedTypes)
                    {
                        if (toType == type)
                        {
                            canCast = true;
                            break;
                        }
                    }
                }
                else
                {
                    canCast = false;
                }
            }

            return canCast;
        }

        /// <summary>
        /// TypeName isn't supported since DynamicProxy doesn't support CreateObjRef()
        /// </summary>
        /// <exception cref="NotSupportedException">TypeName for Dynamic Proxy isn't supported</exception>
        public string TypeName
        {
            get { throw new NotSupportedException("TypeName for DynamicProxy isn't supported"); }
            set { throw new NotSupportedException("TypeName for DynamicProxy isn't supported"); }
        }

        /// <summary>
        /// The reflective method for invoking methods. See documentation for RealProxy.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override IMessage Invoke(IMessage message)
        {
            // Convert to a MethodCallMessage
            IMethodCallMessage methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)message);

            // Extract the method being called
            MethodBase method = methodMessage.MethodBase;

            // Perform the call
            object returnValue = null;
            OnInvokingHandler();
            if (method.DeclaringType == typeof(IDynamicProxy))
            {
                // Handle IDynamicProxy interface calls on this instance instead of on the proxy target instance
                returnValue = method.Invoke(this, methodMessage.Args);
            }
            else
            {
                // Delegate to the invocation handler
                returnValue = _invocationHandler(_proxyTarget, method, methodMessage.Args);
            }
            OnInvokedHandler();
            // Create the return message (ReturnMessage)
            ReturnMessage returnMessage = new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
            return returnMessage;
        }

        /// <summary>
        /// Returns the target object for the proxy
        /// </summary>
        public object ProxyTarget
        {
            get { return _proxyTarget; }
            set { _proxyTarget = value; }
        }

        /// <summary>
        /// The delegate which handles the invocation task in the dynamic proxy
        /// </summary>
        public InvocationDelegate InvocationHandler
        {
            get { return _invocationHandler; }
            set { _invocationHandler = value; }
        }
        /// <summary>
        /// Type support strictness. Used for cast strictness
        /// </summary>
        public bool Strict
        {
            get { return _strict; }
            set { _strict = value; }
        }

        /// <summary>
        /// List of supported types for cast strictness support. Is only checked if Strict is true
        /// </summary>
        public Type[] SupportedTypes
        {
            get { return _supportedTypes; }
            set { _supportedTypes = value; }
        }


        #region IDynamicProxy Members

        public event EventHandler InvokingHanlder;
        public event EventHandler InvokedHanlder;

        public void OnInvokingHandler()
        {
            if (InvokingHanlder != null)
                InvokingHanlder(this, new EventArgs());
        }

        public void OnInvokedHandler()
        {
            if (InvokedHanlder != null)
                InvokedHanlder(this, new EventArgs());
        }

        #endregion
    }
}
