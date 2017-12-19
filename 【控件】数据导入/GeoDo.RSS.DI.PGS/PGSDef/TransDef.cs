using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DI.PGS
{
    public class TransDef
    {
        private ProductDef[] _products;

        public TransDef(ProductDef[] products)
        {
            _products = products;
        }

        public ProductDef GetProductBySmartProductIdentify(string smartIdentify)
        {
            if (_products == null)
                return null;
            foreach (ProductDef item in _products)
            {
                if (item.SmartIdentify.ToUpper() == smartIdentify.ToUpper())
                    return item;
            }
            return null;
        }
    }
}
