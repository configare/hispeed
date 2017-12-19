using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.Smart.AutoLST
{
    public class LstArgs
    {
        public LstArgs(string[] args)
        {
            LoadFromArgs(args);
        }

        private void LoadFromArgs(string[] args)
        {
            string schema = "ldf*,outfile*,ndvi*";
            Args.Args parser = new Args.Args(schema, args);
            LdfFile = parser.GetString("ldf");
            NDVIFile = parser.GetString("ndvi");
            OutFile = parser.GetString("outfile");
        }

        public string LdfFile;
        public string NDVIFile;
        public string OutFile;
    }
}
