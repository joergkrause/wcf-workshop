using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masieri.ServiceModel.WSDiscovery.Messages;

namespace Masieri.ServiceModel.WSDiscovery.Proxy
{
  class ProxyMemento
  {
    public EndpointReference EndpointReferenceValue { get; set; }
    public string Type { get; set; }
    public string ScopeMatchBy { get; set; }
    public string Scopes { get; set; }
    public string XAddr { get; set; }
    public string MetadataVersion { get; set; }
    public ProxyMemento()
    {
        
    }
    public ProxyMemento(string type, Hello hello)
    {
      this.EndpointReferenceValue = new EndpointReference();
      this.EndpointReferenceValue.Address = hello.EndpointReferenceValue.Address;
      this.EndpointReferenceValue.Metadata = hello.EndpointReferenceValue.Metadata;
      this.EndpointReferenceValue.ReferenceParameters = hello.EndpointReferenceValue.ReferenceParameters;
      this.Type = type;
      this.MetadataVersion = hello.MetadataVersion;
      this.ScopeMatchBy = hello.ScopeMatchBy;
      this.XAddr = hello.XAddr;
      this.Scopes = hello.Scopes;
    }
  }
}
