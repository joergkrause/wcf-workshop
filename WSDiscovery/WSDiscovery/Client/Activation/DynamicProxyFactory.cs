using System;

namespace Masieri.ServiceModel.WSDiscovery.Activation
{
    /// <summary>
    /// Factory for creating Dynamic proxy instances
    /// <code>
    /// TestClasses.SimpleClass testClass = new TestClasses.SimpleClass();
    /// TestClasses.ISimpleInterface testClassProxy = (TestClasses.ISimpleInterface) DynamicProxyFactory.Instance.CreateProxy(testClass, new InvocationDelegate(InvocationHandler));
    /// testClassProxy.Method1();
    /// </code>
    /// <see cref="IDynamicProxy"/>
    /// </summary>
    class DynamicProxyFactory
    {
        private static DynamicProxyFactory _factory = new DynamicProxyFactory();

        private DynamicProxyFactory()
        {
        }

        /// <summary>
        /// Get the instance of the factory (singleton)
        /// </summary>
        public static DynamicProxyFactory Instance
        {
            get { return _factory; }
        }

        /// <summary>
        /// Create a proxy for the target object
        /// </summary>
        /// <param name="target">The object to create a proxy for</param>
        /// <param name="_invocationHandler">The invocation handler for the proxy</param>
        /// <returns>The dynamic proxy instance</returns>
        public object CreateProxy(object target, InvocationDelegate invocationHandler)
        {
            return CreateProxy(target, invocationHandler, false, null);
        }

        /// <summary>
        /// Create a proxy for the target object
        /// </summary>
        /// <param name="target">The object to create a proxy for</param>
        /// <param name="_invocationHandler">The invocation handler for the proxy</param>
        /// <param name="_strict">Indicates if the cast support should be _strict. If _strict is true all casts are checked before being performed</param>
        /// <returns>The dynamic proxy instance</returns>
        public object CreateProxy(object target, InvocationDelegate invocationHandler, bool strict)
        {
            return CreateProxy(target, invocationHandler, strict, null);
        }

        /// <summary>
        /// Create a proxy for the target object
        /// </summary>
        /// <param name="target">The object to create a proxy for</param>
        /// <param name="_invocationHandler">The invocation handler for the proxy</param>
        /// <param name="_strict">Indicates if the cast support should be _strict. If _strict is true all casts are checked before being performed. The supportedType list will enabled support for more interfaces than the target object supports</param>
        /// <param name="_supportedTypes">List of types that are supported for casts. Is only checked if _strict is true.</param>
        /// <returns>The dynamic proxy instance</returns>
        public object CreateProxy(object target, InvocationDelegate invocationHandler, bool strict, Type[] supportedTypes)
        {
            return new DynamicProxyImpl(target, invocationHandler, strict, supportedTypes).GetTransparentProxy();
        }
    }
}
