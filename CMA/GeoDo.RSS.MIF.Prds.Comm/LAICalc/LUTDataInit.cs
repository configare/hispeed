using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class LUTDataInit
    {
        public struct Cover{
            public double [][]Ka1a2co;//(*Ka1a2co)[10];
            public double [][]SKa1a2co;//(*SKa1a2co)[10];
            public double [][]Cco;//(*Cco)[2];
            public double [][]SCco;//(*SCco)[2];
            public double [][]LeSr;//(*LeSr)[10];
            public double [][]LeRsr;//(*LeRsr)[10];
            public int []Lines;
            public int []SLines;
        } ;
        public  Cover[] CoverT = new Cover[18];


        public LUTDataInit()
        {
            initparameters();
        }

        private void initparameters()
        {
            CoverT[0].Ka1a2co=LUTData.coniKa1a2co;
	        CoverT[0].Cco=LUTData.coniCco;
	        CoverT[0].LeSr=LUTData.coniLeSr;
	        CoverT[0].Lines=LUTData.coniLines;
	        CoverT[0].SKa1a2co=LUTData.SconiKa1a2co;
	        CoverT[0].SCco=LUTData.SconiCco;
	        CoverT[0].LeRsr=LUTData.coniLeRsr;
	        CoverT[0].SLines=LUTData.SconiLines;	

	        CoverT[1].Ka1a2co=LUTData.tropKa1a2co;
	        CoverT[1].Cco=LUTData.tropCco;
	        CoverT[1].LeSr=LUTData.tropLeSr;
	        CoverT[1].Lines=LUTData.tropLines;
	        CoverT[1].SKa1a2co=LUTData.StropKa1a2co;
	        CoverT[1].SCco=LUTData.StropCco;
	        CoverT[1].LeRsr=LUTData.tropLeRsr;
	        CoverT[1].SLines=LUTData.StropLines;

	        CoverT[2].Ka1a2co=LUTData.coniKa1a2co;
	        CoverT[2].Cco=LUTData.coniCco;
	        CoverT[2].LeSr=LUTData.coniLeSr;
	        CoverT[2].Lines=LUTData.coniLines;
	        CoverT[2].SKa1a2co=LUTData.SconiKa1a2co;
	        CoverT[2].SCco=LUTData.SconiCco;
	        CoverT[2].LeRsr=LUTData.coniLeRsr;
	        CoverT[2].SLines=LUTData.SconiLines;
	
	        CoverT[3].Ka1a2co=LUTData.decidKa1a2co;
	        CoverT[3].Cco=LUTData.decidCco;
	        CoverT[3].LeSr=LUTData.decidLeSr;
	        CoverT[3].Lines=LUTData.decidLines;
	        CoverT[3].SKa1a2co=LUTData.SdecidKa1a2co;
	        CoverT[3].SCco=LUTData.SdecidCco;
	        CoverT[3].LeRsr=LUTData.decidLeRsr;
	        CoverT[3].SLines=LUTData.SdecidLines;

	        CoverT[4].Ka1a2co=LUTData.condecKa1a2co;
	        CoverT[4].Cco=LUTData.condecCco;
	        CoverT[4].LeSr=LUTData.condecLeSr;
	        CoverT[4].Lines=LUTData.condecLines;
	        CoverT[4].SKa1a2co=LUTData.ScondecKa1a2co;
	        CoverT[4].SCco=LUTData.ScondecCco;
	        CoverT[4].LeRsr=LUTData.condecLeRsr;
	        CoverT[4].SLines=LUTData.ScondecLines;
	
	        CoverT[5].Ka1a2co=LUTData.shrubKa1a2co;
	        CoverT[5].Cco=LUTData.shrubCco;
	        CoverT[5].LeSr=LUTData.shrubLeSr;
	        CoverT[5].Lines=LUTData.shrubLines;
	        //CoverT[5].SKa1a2co=LUTData.SshrubKa1a2co;
	        //CoverT[5].SCco=LUTData.SshrubCco;
	        //CoverT[5].LeRsr=LUTData.shrubLeRsr;
	        //CoverT[5].SLines=LUTData.SshrubLines;

	        CoverT[6].Ka1a2co=LUTData.shrubKa1a2co;
	        CoverT[6].Cco=LUTData.shrubCco;
	        CoverT[6].LeSr=LUTData.shrubLeSr;
	        CoverT[6].Lines=LUTData.shrubLines;
	        //CoverT[6].SKa1a2co=LUTData.SshrubKa1a2co;
	        //CoverT[6].SCco=LUTData.SshrubCco;
	        //CoverT[6].LeRsr=LUTData.shrubLeRsr;
	        //CoverT[6].SLines=LUTData.SshrubLines;

	        CoverT[7].Ka1a2co=LUTData.shrubKa1a2co;
	        CoverT[7].Cco=LUTData.shrubCco;
	        CoverT[7].LeSr=LUTData.shrubLeSr;
	        CoverT[7].Lines=LUTData.shrubLines;
	        //CoverT[7].SKa1a2co=LUTData.SshrubKa1a2co;
	        //CoverT[7].SCco=LUTData.SshrubCco;
	        //CoverT[7].LeRsr=LUTData.shrubLeRsr;
	        //CoverT[7].SLines=LUTData.SshrubLines;

	        CoverT[8].Ka1a2co=LUTData.cropKa1a2co;
	        CoverT[8].Cco=LUTData.cropCco;
	        CoverT[8].LeSr=LUTData.cropLeSr;
	        CoverT[8].Lines=LUTData.cropLines;
	        //CoverT[9].SKa1a2co=LUTData.ScropKa1a2co;
	        //CoverT[9].SCco=LUTData.ScropCco;
	        //CoverT[9].LeRsr=LUTData.cropLeRsr;
	        //CoverT[9].SLines=LUTData.ScropLines;
            
	        CoverT[9].Ka1a2co=LUTData.cropKa1a2co;
	        CoverT[9].Cco=LUTData.cropCco;
	        CoverT[9].LeSr=LUTData.cropLeSr;
	        CoverT[9].Lines=LUTData.cropLines;
	        //CoverT[9].SKa1a2co=LUTData.ScropKa1a2co;
	        //CoverT[9].SCco=LUTData.ScropCco;
	        //CoverT[9].LeRsr=LUTData.cropLeRsr;
	        //CoverT[9].SLines=LUTData.ScropLines;

	        CoverT[10].Ka1a2co=LUTData.cropKa1a2co;
	        CoverT[10].Cco=LUTData.cropCco;
	        CoverT[10].LeSr=LUTData.cropLeSr;
	        CoverT[10].Lines=LUTData.cropLines;
	        //CoverT[10].SKa1a2co=LUTData.ScropKa1a2co;
	        //CoverT[10].SCco=LUTData.ScropCco;
	        //CoverT[10].LeRsr=LUTData.cropLeRsr;
	        //CoverT[10].SLines=LUTData.ScropLines;

	        CoverT[11].Ka1a2co=LUTData.cropKa1a2co;
	        CoverT[11].Cco=LUTData.cropCco;
	        CoverT[11].LeSr=LUTData.cropLeSr;
	        CoverT[11].Lines=LUTData.cropLines;
	        //CoverT[11].SKa1a2co=LUTData.ScropKa1a2co;
	        //CoverT[11].SCco=LUTData.ScropCco;
	        //CoverT[11].LeRsr=LUTData.cropLeRsr;
	        //CoverT[11].SLines=LUTData.ScropLines;

	        CoverT[12].Ka1a2co=LUTData.cropKa1a2co;
	        CoverT[12].Cco=LUTData.cropCco;
	        CoverT[12].LeSr=LUTData.cropLeSr;
	        CoverT[12].Lines=LUTData.cropLines;
	        //CoverT[12].SKa1a2co=LUTData.ScropKa1a2co;
	        //CoverT[12].SCco=LUTData.ScropCco;
	        //CoverT[12].LeRsr=LUTData.cropLeRsr;
	        //CoverT[12].SLines=LUTData.ScropLines;

	        CoverT[13].Ka1a2co=LUTData.cropKa1a2co;
	        CoverT[13].Cco=LUTData.cropCco;
	        CoverT[13].LeSr=LUTData.cropLeSr;
	        CoverT[13].Lines=LUTData.cropLines;
	        //CoverT[13].SKa1a2co=LUTData.ScropKa1a2co;
	        //CoverT[13].SCco=LUTData.ScropCco;
	        //CoverT[13].LeRsr=LUTData.cropLeRsr;
	        //CoverT[13].SLines=LUTData.ScropLines;

	        CoverT[14].Ka1a2co=LUTData.cropKa1a2co;  //add by Lin for test 
	        CoverT[14].Cco=LUTData.cropCco;
	        CoverT[14].LeSr=LUTData.cropLeSr;
	        CoverT[14].Lines=LUTData.cropLines;

	        CoverT[17].Ka1a2co=LUTData.tdecKa1a2co;
	        CoverT[17].Cco=LUTData.tdecCco;
	        CoverT[17].LeSr=LUTData.tdecLeSr;
	        CoverT[17].Lines=LUTData.tdecLines;
	        CoverT[17].SKa1a2co=LUTData.StdecKa1a2co;
	        CoverT[17].SCco=LUTData.StdecCco;
	        CoverT[17].LeRsr=LUTData.tdecLeRsr;
	        CoverT[17].SLines=LUTData.StdecLines;
        }
    }
}
