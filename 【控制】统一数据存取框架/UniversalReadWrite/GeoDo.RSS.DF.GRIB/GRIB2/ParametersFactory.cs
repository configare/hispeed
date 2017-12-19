using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GRIB
{
    public static class ParametersFactory
    {
        public static List<Discipline> DisciplineSet;
        private static string parameterTableFile=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ParameterTable2GRIB2.xml");

        public static Parameter GetParameter(int disciplineNo, int categoryNo, int parameterNo)
        {
            if (DisciplineSet == null || DisciplineSet.Count < 1)
            {
                InitDisciplineSet();
            }
            foreach (Discipline item in DisciplineSet)
            {
                if (item.Number == disciplineNo)
                {
                    Category cat = item.GetCategory(categoryNo);
                    if (cat != null)
                    {
                        return cat.GetParameter(parameterNo);
                    }
                }
            }
            return null;
        }

        private static void InitDisciplineSet()
        {
            if(!File.Exists(parameterTableFile))
                return ;
            DisciplineSet = new List<Discipline>();
            XDocument doc = XDocument.Load(parameterTableFile);
            XElement root = doc.Root;
            //disciplines
            var eleNames = root.Elements("Discipline");
            foreach (XElement eleName in eleNames)
            {
                Discipline dis = new Discipline();
                dis.Name=eleName.Attribute("name").Value;
                dis.Number=int.Parse(eleName.Attribute("number").Value);
                var categorys=eleName.Elements("Category");
                foreach(XElement cat in categorys)
                {
                    Category category=new Category();
                    category.Name=cat.Attribute("name").Value;
                    category.Number=int.Parse(cat.Attribute("number").Value);
                    var parameters=cat.Elements("Parameter");
                    if(parameters!=null)
                    {
                        foreach(XElement para in parameters)
                        {
                            Parameter parameter=new Parameter();
                            parameter.Name=para.Attribute("name").Value;
                            parameter.ParameterNo=int.Parse(para.Attribute("number").Value);
                            parameter.Unit=para.Attribute("unit").Value;
                            parameter.Abbrev=para.Attribute("abbrev").Value;
                            category.ParameterList.Add(parameter);
                        }
                    }
                    dis.CategoryList.Add(category);
                }
                DisciplineSet.Add(dis);
            }
        }

    }
}
