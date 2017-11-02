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
namespace Masieri.ServiceModel.WSDiscovery.Service
{
  /// <summary>
  /// ScopeList class
  /// </summary>
  public class ScopeList : System.Collections.IEnumerable
  {
    List<string> _internalList;
    /// <summary>
    /// Occurs when [scope list modified].
    /// </summary>
    public event EventHandler ScopeListModified;
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeList"/> class.
    /// </summary>
    public ScopeList()
    {
      _internalList = new List<string>();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeList"/> class.
    /// </summary>
    /// <param name="scopes">The scopes.</param>
    public ScopeList(string[] scopes)
    {
      _internalList = new List<string>(scopes);
    }
    /// <summary>
    /// Adds the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    public void Add(string scope)
    {
      _internalList.Add(scope);
      OnScopeModified();
    }
    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="scopes">The scopes.</param>
    public void AddRange(string[] scopes)
    {
      _internalList.AddRange(scopes);
      OnScopeModified();
    }
    /// <summary>
    /// Removes the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    public void Remove(string scope)
    {
      _internalList.Remove(scope);
      OnScopeModified();
    }
    /// <summary>
    /// Removes all.
    /// </summary>
    /// <param name="match">The match.</param>
    public void RemoveAll(Predicate<string> match)
    {
      _internalList.RemoveAll(match);
      OnScopeModified();
    }

    /// <summary>
    /// Called when [scope modified].
    /// </summary>
    private void OnScopeModified()
    {
      if (ScopeListModified != null)
        ScopeListModified(this, new EventArgs());
    }
    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count { get { return _internalList.Count; } }
    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
      _internalList.Clear();
    }
    /// <summary>
    /// Toes the array.
    /// </summary>
    /// <returns></returns>
    public string[] ToArray()
    {
      return _internalList.ToArray();
    }
    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    System.Collections.IEnumerator GetEnumerator()
    {
      return _internalList.GetEnumerator();
    }
    #endregion
    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _internalList.GetEnumerator();
    }
    #endregion

    
  }
}
