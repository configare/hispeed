using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    /// <summary>
    /// LST产品分块数据,属性值修正
    /// </summary>
    public class LSTBlock10Modify:IBlockDefinition
    {
        private List<BlockItem> _items = null;
        private int span = 10;
        private int _xStart = 00; //W30
        private float _lonOffset = 150.00f;
        private float _lonResolution = 1000.00f;

        public LSTBlock10Modify()
        { }

        #region IBlockDefinition 成员

        public string Name
        {
            get { return "NDVI固定10度分幅"; }
        }

        public BlockItem[] GetAllBlockItems()
        {
            if (_items == null)
                ComputeBlockItems();
            return _items != null && _items.Count > 0 ? _items.ToArray() : null;
        }

        public BlockItem[] GetChinaRegionBlockItems()
        {
            throw new NotImplementedException();
        }

        public BlockItem GetBlockItem(double longitude, double latitude)
        {
            throw new NotImplementedException();
        }

        public BlockItem GetBlockItem(string name)
        {
            if (_items == null)
                ComputeBlockItems();
            foreach (BlockItem it in _items)
                if (it.Name.ToUpper() == name.ToUpper())
                    return it;
            return null;
        }

        #endregion

        private void ComputeBlockItems()
        {
            string[] LonNOs = GetLongitudeNOs();
            string[] LatNOs = GetLatitudeNOs();
            _items = new List<BlockItem>();
            for (int i = 0; i < LatNOs.Length; i++)
            {
                float LeftY = GetLat(LatNOs[i]);
                for (int j = 0; j < LonNOs.Length; j++)
                {
                    float LeftX = GetLon(LonNOs[j]);
                    BlockItem it = new BlockItem(LatNOs[i] + LonNOs[j], LeftX, LeftY, (float)((span/0.01) * _lonResolution));
                    it.BlockIdentity = "D10";
                    //Console.WriteLine("NO:" + it.Name + ",X = " + it.MinX.ToString() + ", Y = " + it.MaxY.ToString());
                    _items.Add(it);
                    it.BlockTypes = masBlockTypes.D10;
                }
            }
        }

        private int GetX(string strNo)
        {
            int v = 0;
            if (int.TryParse(strNo[0].ToString(), out v))
            {
                return v * span + _xStart; //加上起始坐标数
            }
            else
            {
                char c = strNo[0];
                int cNO = c - 'A';
                if (c >= 'L')
                {
                    cNO = c - 'L' - 18;     
                    cNO *= (-span);
                    return cNO;
                }
                else
                {
                    cNO *= span;
                    return 100 + (int)cNO + _xStart;
                }
            }
        }

        private float GetLon(string strNo)
        {
            int v = 0;
            if (int.TryParse(strNo[0].ToString(), out v))
            {
                return (float)(((v * span + _xStart) - _lonOffset) * _lonResolution * (span / 0.1)); //加上起始坐标数
            }
            else
            {
                char c = strNo[0];
                int cNO = c - 'A';
                if(c>='L')
                {
                    cNO = c - 'L' + 1;
                    return (float)((180 - cNO * span) * _lonResolution * (span / 0.1));
                }
                else if (c >= 'I')
                {
                    cNO = 'I'- c -1;
                    return (float)((cNO * (span) - _lonOffset) * _lonResolution * (span / 0.1));
                }
                else if (c >= 'F')
                {
                    cNO = c - 'F';
                    return (float)(cNO * (span) * _lonResolution * (span / 0.1));
                }
                else
                {
                    cNO *= span;
                    return (float)(((100 + (int)cNO + _xStart) - _lonOffset) * _lonResolution * (span / 0.1));
                }
            }
        }

        private float GetLat(string strNo)
        {
            int v = 0;
            if (int.TryParse(strNo[0].ToString(), out v))
            {
                if (v == 9)
                    return 0;
                else
                    return (v + 1) * span * 100000; //处理为左上角
            }
            else
            {
                char c = strNo[0];
                int cNO = c - 'A' + 1;
                cNO *= (-span);
                return (int)cNO * 100000;
            }
        }

        private int GetY(string strNo)
        {
            int v = 0;
            if (int.TryParse(strNo[0].ToString(), out v))
            {
                if (v == 9)
                    return 0;
                else
                    return (v + 1) * span; //处理为左上角
            }
            else
            {
                char c = strNo[0];
                int cNO = c - 'A' + 1;
                cNO *= (-span);
                return (int)cNO;
            }
        }

        private string[] GetLatitudeNOs()
        {
            List<string> LatNOs = new List<string>();
            string[] Nos = null;
            //North, +  Latitude,     00~09,..., 80_89
            Nos = ComputeBlockNOs(0, 90, '0');
            LatNOs.AddRange(Nos);
            //South,  -  Latitude,     90~99,..., H0~H9
            Nos = ComputeBlockNOs(10, 90, 'A');
            LatNOs.Add("90");
            LatNOs.AddRange(Nos);
            return LatNOs.ToArray();
        }

        private string[] GetLongitudeNOs()
        {
            List<string> LonNos = new List<string>();
            string[] Nos = null;
            //East,    +  Longitude,  00~09,..., H0~H9
            Nos = ComputeBlockNOs(0, 100, '0');
            LonNos.AddRange(Nos);
            Nos = ComputeBlockNOs(100, 180, 'A');
            LonNos.AddRange(Nos);
            //West,   -   Longitude,  I0~I9, ..., Z0~Z9
            Nos = ComputeBlockNOs(0, 180, 'I');
            LonNos.AddRange(Nos);
            return LonNos.ToArray();
        }

        private string[] ComputeBlockNOs(int beginValue, int endValue, char beginChar)
        {
            int n = (endValue - beginValue) / span;
            string strNO = null;
            List<string> NOs = new List<string>();
            for (int i = 0; i < n; i++)
            {
                strNO = beginChar.ToString().PadRight(2, '0');
                NOs.Add(strNO);
                beginChar++;
            }
            return NOs.ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
