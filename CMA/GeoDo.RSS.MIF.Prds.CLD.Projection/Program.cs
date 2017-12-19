using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;

namespace GeoDo.RSS.MIF.Prds.CLD.Projection
{
    class Program
    {
        static void Main(string[] args)
        {
            string prjItemName =args[0];
            double minX = double.Parse(args[1]);
            double maxX = double.Parse(args[2]);
            double minY = double.Parse(args[3]);
            double maxY = double.Parse(args[4]);

            PrjEnvelopeItem prjItem=new PrjEnvelopeItem(prjItemName,new GeoDo.RasterProject.PrjEnvelope(minX,maxX,minY,maxY));
            string dataSet= args[5];
            string locationFile= args[6];            
            string dataFile= args[7];
            string outfname = args[8];
            float outResolution= float.Parse(args[9]);
            MOD06DataProject.Project(prjItem, dataSet, locationFile, dataFile, outfname, outResolution);
        }
    }
}
