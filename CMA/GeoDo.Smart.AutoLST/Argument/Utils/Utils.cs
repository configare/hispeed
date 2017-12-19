using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
  class Utils
  {
    public static bool StringStartsWith(string stringToSearch, string[] searchValues)
    {
      bool result = false;

      foreach (string value in searchValues)
      {
        result = result || stringToSearch.StartsWith(value);
        if (result) break;
      }

      return result;
    }
  }
}
