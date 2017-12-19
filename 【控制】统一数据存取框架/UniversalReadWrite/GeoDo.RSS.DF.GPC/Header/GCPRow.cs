using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* 定义ISCCP D2数据格式
 * 文档：
 * http://isccp.giss.nasa.gov/docs/documents.html
 * http://isccp.giss.nasa.gov/docs/D-toc.html
 * （D2）:
 * http://isccp.giss.nasa.gov/products/products.html 中的D2
 * 数据展示：
 * http://isccp.giss.nasa.gov/cgi-bin/browsed2.cgi?variable=32&range=17&yymm=&button=View&data=0
 * 从1到130通道值代表含义：
 * http://isccp.giss.nasa.gov/pub/documents/d2.titles
 * ISCCP Equal-Area Map Grid：
 * http://isccp.giss.nasa.gov/docs/mapgridinfo.html
 */
namespace GeoDo.RSS.DF.GPC
{
    /// <summary>
    /// 整个文件是个二进制格式，其组织如下：
    /// (130 + 130 * 99)* 67 == 871000byte
    /// (每行行头+每个Cell值个数*Cell个数)*行数
    /// ----------------------------------------------------
    /// 总共有67行，每行是如下组织的：
    /// (130 + 130 * 99)=(130行头+130个值*99个Cell)
    /// </summary>
    public class GCPRow
    {
        private byte[] _profix = null;// new byte[130];
        private byte[][] _gridCell = null;// new byte[99][130];

        public GCPRow()
        { 
        }

        /* ISCCP的GPC格式数据的每行的头信息（prefix）,130bytes
         * byte 1:Record number in file (1 - 67)
         * 文件中的记录号，即行号，1-67
         * ...
         */
        public byte[] Profix
        {
            get { return _profix; }
            set { _profix = value; }
        }

        /* 1-7地图格点单元基本信息MAP GRID CELL IDENTIFICATION
         * MAP GRID CELL IDENTIFICATION,每个地图格点单元（D2MapGridCell）的头信息,130bytes
         * 具体含义请参照d2.title文件
         */
        public byte[][] GridCell
        {
            get { return _gridCell; }
            set { _gridCell = value; }
        }
    }
}
