using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    class TemplateCode
    {
        public const string FeatureComputeFuncTemplate = @"
using System;
using System.Collections.Generic;

namespace GeoDo.RSS.MIF.Core
{
    public class FuncBuilder : IFeatureComputeFuncProvider<%DataType%,%Feature%>
    {
        /*
        public Func<int, %DataType%[], %Feature%> GetBoolFunc()
        {
            return (idx, values) => { return values[0] / values[1]; };
        }
		*/

%Func%		
      
	   public float NDVI(%DataType% b1,%DataType% b2)
	   {
	        return (b1 + b2) == 0? 0f : (b1 - b2 ) / (float)(b1 + b2);
	   }
    }
}";

        public const string FuncTemplate = @"
using System;
using System.Collections.Generic;

namespace GeoDo.RSS.MIF.Core
{
    public class FuncBuilder : IExtractFuncProvider<%DataType%>
    {
        /*
        public Func<int, %DataType%[], bool> GetBoolFunc()
        {
            return (idx, values) => { return (values[0] > 300) && (values[1] > 230); };
        }
		*/

%Func%		

       public float NDVI(%DataType% b1,%DataType% b2)
	   {
	        return (b1 + b2) == 0? 0f : (b1 - b2 ) / (float)(b1 + b2);
	   }
    }
}";

        public const string FeatureSimpleComputeFuncTemplate = @"
using System;
using System.Collections.Generic;

namespace GeoDo.RSS.MIF.Core
{
    public class FuncBuilder : IFeatureSimpleComputeFuncProvider<%DataType%,%Feature%>
    {

%Func%		
    }
}";
    }
}
