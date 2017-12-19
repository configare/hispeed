using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST
{
    class Program
    {
        static void Main(string[] args)
        {
            LstAutoProduct lst = new LstAutoProduct();
            lst.Make(args);
        }
    }
}
