using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;

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
  /// Constants
  /// </summary>
    sealed class Constants
    {
      /// <summary>
      /// Constants for SOAP
      /// </summary>
        public sealed class Soap
        {
          /// <summary>
          /// Max Header Size
          /// </summary>
            public const int MaxHeaderSize = 4096;
            public const int MaxMessageSize = 10240;
            public const string Namespace11 = "http://schemas.xmlsoap.org/soap/envelope/";
            public const string Namespace12 = "http://www.w3.org/2003/05/soap-envelope";
            
        }
        /// <summary>
        /// Constants for Addressing
        /// </summary>
        public sealed class Addressing
        {
            public const string NamespaceAugust2004 = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
            public const string Namespace10 = "http://www.w3.org/2005/08/addressing";
            public const string AnonymousDestination = "http://www.w3.org/2005/08/addressing/anonymous";
            public const string NoneDestination = "http://www.w3.org/2005/08/addressing/none";

        }
        public const string ScopesSeparator = "\r\n";
        public const string TypesSeparator = " ";
        #if DEBUG
        public const int DiscoveryTimeout = 10000;
        #else
                public const int DiscoveryTimeout = 5000;
        #endif

        public static MessageVersion SOAPMessageVersion = MessageVersion.Soap11WSAddressing10;
        public const string NamespacePrefix = "d";
        public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2005/04/discovery";
        public const string HelloAction = "http://schemas.xmlsoap.org/ws/2005/04/discovery/Hello";
        public const string ByeAction = "http://schemas.xmlsoap.org/ws/2005/04/discovery/Bye";
        public const string ProbeAction = "http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe";
        public const string ProbeMatchAction = "http://schemas.xmlsoap.org/ws/2005/04/discovery/ProbeMatches";
        public const string ResolveAction = "http://schemas.xmlsoap.org/ws/2005/04/discovery/Resolve";
        public const string ResolveMatchAction = "http://schemas.xmlsoap.org/ws/2005/04/discovery/ResolveMatches";

        public const string HandshakeConfigurationName = "IDiscoveryHandshake";
        public const string DiscoveryTargetServiceResponsesConfigurationName = "IDiscoveryTargetServiceResponses";
        public const string DiscoveryTargetServiceConfigurationName = "IDiscoveryTargetService";
        public const string RoleAnonymous = "http://schemas.xmlsoap.org/ws/2004/08/role/anonymous";
        public const string TargetService = "urn:schemas-xmlsoap-org:ws:2005:04:discovery";
        public const string ScopesMatchBy = "http://schemas.xmlsoap.org/ws/2005/04/discovery/rfc2396";
        //TODO: Sistemare sulla hello e sul riconoscimento del proxy
        public const string DiscoveryProxyType = NamespacePrefix + ":DiscoveryProxy";
        public const string DiscoveryTargetServiceType = NamespacePrefix + ":TargetService";
        public const int MulticastPort = 3702;
        public const string MulticastAddress = "239.255.255.250";
        public const string MulticastAddressIPV6 = "FF02::C";

        public const int AppMaxDelay = 500;
        public const int MatchTimeout = AppMaxDelay + 100;
        public const double MessageReceiveTimeout = 5000;
        /// <summary>
        /// Tempo massimo per il proxy per rispondere, dopo di che sarà considerato morto
        /// </summary>
        public const int ProxyTimeout = 5000;
        public static UriHostNameType IPVersion = UriHostNameType.IPv4;
        public static EndpointAddress DiscoveryEndpoint = new EndpointAddress(String.Format("soap.udp://{0}:{1}/", (IPVersion == UriHostNameType.IPv4) ? MulticastAddress : MulticastAddressIPV6, MulticastPort));
        public const string DiscoveryBindingSectionPath = "system.serviceModel/bindings/discoveryBinding";
        public const string DiscoveryExceptionMessage = "Discovery process tried to recover the fault state of the channel but all the discovered available channels have been used";

        public const string ClientSectionName = "clientConfig";
        public const string DiscoveryExtendedSectionName = "extendedConfig";
        public const string DiscoveryProxySectionName = "proxyConfig";
        /// <summary>
        /// Tre secondi e mezzo come massimo timeout per ricevere almeno un serviceEndpoint
        /// </summary>
        public static int MaxDiscoveryWaitingTimeout = 3500;
    }
}
