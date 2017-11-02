using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

/*****************************************************************************
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
//*****************************************************************************/
namespace Masieri.ServiceModel.WSDiscovery.Behaviors
{
  /// <summary>
  /// ScopesCollection Class
  /// </summary>
    public class ScopesCollection : ConfigurationElementCollection
    {
      /// <summary>
      /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
      /// </summary>
      /// <value></value>
      /// <returns>
      /// The <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> of this collection.
      /// </returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Masieri.ServiceModel.WSDiscovery.Behaviors.ScopeElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ScopeElement this[int index]
        {
            get { return (ScopeElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(ScopeElement element)
        {
            BaseAdd(element);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ScopeElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ScopeElement)element).Url;
        }

        /// <summary>
        /// Removes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Remove(ScopeElement element)
        {
            BaseRemove(element.Url);
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
    }
}
