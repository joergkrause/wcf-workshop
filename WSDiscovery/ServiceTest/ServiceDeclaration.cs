using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ServiceTest
{
  [ServiceContract()]
  public interface IServiceSample
  {
    [OperationContract()]
    string GetString(string parToReturn);
  }
  [ServiceContract()]
  public interface IServiceSample2
  {
    [OperationContract()]
    int GetNumber(int parToReturn);
  }
  /// <summary>
  /// Service Implementation
  /// </summary>
  public class ServiceInstance : IServiceSample, IServiceSample2
  {

    #region IServiceSample Members

    public string GetString(string parToReturn)
    {
      return parToReturn;
    }

    #endregion

    #region IServiceSample2 Members

    public int GetNumber(int parToReturn)
    {
      return parToReturn;
    }

    #endregion
  }
}
