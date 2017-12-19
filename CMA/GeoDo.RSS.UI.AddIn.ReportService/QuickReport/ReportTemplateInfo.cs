using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    public class ReportTemplateInfo
    {
        public decimal ReportTemplateID;
        public string ReportTemplateName;
        public string ReportSubProType;
        public DateTime ReportDateTime = DateTime.MinValue;

        public ReportTemplateInfo()
        { }

        public ReportTemplateInfo(string reportSubProType, string reportModelName, decimal reportTemplateID)
        {
            ReportTemplateID = reportTemplateID;
            ReportTemplateName = reportModelName;
            ReportSubProType = reportSubProType;
        }
    }
}
