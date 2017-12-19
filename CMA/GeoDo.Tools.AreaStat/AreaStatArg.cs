using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.Tools.AreaStat
{
    public class AreaStatArg
    {
        string _inputFileDir;
        string _outputFileName;
        float _resolution;

        public AreaStatArg(string argumentFileName)
        {
            if (string.IsNullOrEmpty(argumentFileName))
                return;
            XElement ele = XElement.Load(argumentFileName);
            if (ele != null)
            {
                _inputFileDir = ele.Element("InputDir").Value;
                _outputFileName = ele.Element("OutputFile").Value;
                string resolution = ele.Element("OutResolution").Value;
                if (string.IsNullOrEmpty(resolution))
                    _resolution = 0f;
                else
                    _resolution = float.Parse(ele.Element("OutResolution").Value);
            }
        }

        public string InputFileDir
        {
            get { return _inputFileDir; }
            set { _inputFileDir = value; }
        }

        public string OutputFileName
        {
            get { return _outputFileName; }
            set { _outputFileName = value; }
        }

        public float OutResolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }
    }
}
