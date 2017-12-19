using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class HdrFile_Parser
    {
        [Test]
        public void ParseFromFile()
        {
            string infile = "d:\\LDF_2012_2_23.hdr";
            ParseFromFile(infile);
        }

        [Test]
        public void ParseFromFileWithPrjInfo()
        {
            string infile = "d:\\FY3A_VIRRX_GBAL_L1_Albers_20110322_0525_1000M_MS_LW_GBAL2.hdr";
            ParseFromFile(infile);
        }

        public void ParseFromFile(string infile)
        {
            string outfile = Path.Combine(Path.GetDirectoryName(infile), Path.GetFileNameWithoutExtension(infile) + "_COPY.hdr");
            HdrFile hdr = HdrFile.LoadFrom(infile);
            Assert.NotNull(hdr);
            hdr.SaveTo(outfile);
            Console.Write(File.ReadAllText(outfile));
            //FileAssert.AreEqual(infile, outfile);
        }
    }
}
