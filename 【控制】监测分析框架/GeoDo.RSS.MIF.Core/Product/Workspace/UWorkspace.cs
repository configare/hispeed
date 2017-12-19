using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public static class UWorkspace
    {
        public static string GetStrategyXmlName(WorkspaceDef wksdef)
        {
            string wkddir = MifEnvironment.GetWorkspaceDir();
            string identify = wksdef.Identify;
            string filedir = Path.Combine(wkddir, identify);
            string filename = Path.Combine(filedir, string.Format("StrategyFilter_{0}.xml", identify));
            return filename;
        }

        public static StrategyFilter StrategyFilter(WorkspaceDef wksdef)
        {
            string filename = GetStrategyXmlName(wksdef);

            StrategyFilter strategyFilter;
            if (!File.Exists(filename))
            {
                strategyFilter = new StrategyFilter();
                strategyFilter.Days = 1;
                strategyFilter.Sensors = new string[] {"AVHRR", "MERSI", "MODIS", "VISSR", "VIRR"};
                var str = UXmlConvert.GetString(strategyFilter);
                UIO.SaveFile(str, filename);
            }
            else
            {
                var str = UIO.ReadFile(filename);
                strategyFilter = UXmlConvert.GetObject<StrategyFilter>(str);
            }
            return strategyFilter;
        }
    }
}
