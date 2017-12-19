using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoNDVI
{
    public class Program
    {
        static void Main(string[] args)
        {
            Producter producter = new Producter();
            producter.Make(args[0], args[1], args[2]);
        }
    }
}
