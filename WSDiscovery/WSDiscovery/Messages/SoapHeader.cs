using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
  /// SoapHeader class
  /// </summary>
    public class SoapHeader
    {
        Dictionary<string, string> _attributes = new Dictionary<string, string>();
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Adds the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddAttribute(string name, string value)
        {
            if (_attributes.ContainsKey(name))
                _attributes.Add(name, value);
        }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}", Name);
            switch (Name.ToLower())
            {
                //special cases
                case "a:to":
                case "a:messageid":
                case "a:action":
                case "a:relatesto":
                case "a:replyto": 
                    sb.Append(" s:mustUnderstand=\"1\"");
                    break;
                
                
                default:
                    foreach (string key in _attributes.Keys)
                    {
                        sb.AppendFormat(" {0}=\"{1}\"", key, _attributes[key]);
                    }
                    break;
            }
            sb.Append(">");
            sb.Append(Value);
            sb.AppendFormat("</{0}>", Name);
            return sb.ToString();
        }
    }
}
