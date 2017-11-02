using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel.Channels;
using Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.Transport;
using Masieri.ServiceModel.WSDiscovery.Helpers.BindingMementos.MessageEncoding;
using Masieri.ServiceModel.WSDiscovery.Helpers;

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
  /// EndpointReference class
  /// </summary>
    public class EndpointReference
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="EndpointReference"/> class.
      /// </summary>
        public EndpointReference()
        {

        }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address { get; set; }
        /// <summary>
        /// Gets or sets the reference parameters.
        /// </summary>
        /// <value>The reference parameters.</value>
        public object ReferenceParameters { get; set; }
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public object Metadata { get; set; }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<EndpointReference targetns=\"{0}\">", Constants.Addressing.Namespace10);
            sb.AppendFormat("<Address>{0}</Address>", this.Address);
            if (ReferenceParameters != null)
                sb.AppendFormat("<ReferenceParameters>{0}</ReferenceParameters>", this.ReferenceParameters.ToString());
            if (Metadata != null)
                sb.AppendFormat("<Metadata>{0}</Metadata>", this.Metadata.ToString());

            sb.Append("</EndpointReference>");
            return sb.ToString();
        }
        /// <summary>
        /// Froms the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static EndpointReference FromReader(XmlReader reader)
        {
            EndpointReference ep = new EndpointReference();
            do
            {

                if (reader.IsStartElement() && reader.LocalName == "Address")
                {
                    ep.Address = reader.ReadElementContentAsString();
                }
                if (reader.IsStartElement() && reader.LocalName == "ReferenceParameters")
                {
                    if (reader.Read() && reader.IsStartElement() && reader.LocalName == "Binding")
                    {
                        List<BindingElement> bindings = BindingMemento.RestoreMemento(reader);
                        CustomBinding binding = new CustomBinding(bindings.ToArray());
                        ep.ReferenceParameters = binding;
                    
                    }
                }
                if (reader.IsStartElement() && reader.LocalName == "Metadata")
                {
                    //TODO:ep.Metadata = reader.ReadElementContentAsString();
                }
                if (!reader.IsStartElement() && reader.LocalName == "EndpointReference")
                {
                    return ep;
                }
            } while (reader.Read());
            return null;
        }

       

    }
}
