using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class VectorAOITemplateFactory
    {
        public static string[] VectorTemplateNames = null;

        static VectorAOITemplateFactory()
        {
            VectorTemplateNames = new string[] { "太湖", "太湖无水草", "巢湖", "滇池", "鄱阳湖", "洞庭湖", 
                "海陆模版", "贝尔湖", "呼伦湖", "海陆模版_反", "丹江口水库", "官厅水库", "密云水库","郊区农田","洞庭分区","西藏常用湖泊",
                "纳木错", "色林错", "羊卓雍错", "普莫雍错", "当惹雍错", "扎日南木错", "佩枯错", "塔若错", "玛旁雍错", "拉昂错"};
        }

        public static VectorAOITemplate GetAOITemplate(string name)
        {
            switch (name)
            {
                case "太湖":
                    return new VectorAOITemplate("太湖水体边界.shp", true);
                case "太湖无水草":
                    return new VectorAOITemplate("太湖无水草边界.shp", true);
                case "巢湖":
                    return new VectorAOITemplate("巢湖水体边界.shp", true);
                case "滇池":
                    return new VectorAOITemplate("滇池水体边界.shp", true);
                case "鄱阳湖":
                    return new VectorAOITemplate("鄱阳湖水体边界.shp", true);
                case "洞庭湖":
                    return new VectorAOITemplate("洞庭湖水体边界.shp", true);
                case "海陆模版":
                    return new VectorAOITemplate("海陆模版.shp", true);
                case "海陆模版_反":
                    VectorAOITemplate t = new VectorAOITemplate("海陆模版.shp", true);
                    t.IsReverse = true;
                    return t;
                case "贝尔湖":
                    return new VectorAOITemplate("中国湖泊.shp",
                        (fet) => { return fet.GetFieldValue("name") == "贝尔湖"; }
                        );
                case "呼伦湖":
                    return new VectorAOITemplate("中国湖泊.shp",
                        (fet) => { return fet.GetFieldValue("name") == "呼伦湖"; }
                        );
                case "省级行政区":
                    return new VectorAOITemplate("省级行政区域_面.shp");
                case "土地利用类型":
                    return new VectorAOITemplate("土地利用类型_合并.shp");
                case "丹江口水库":
                    return new VectorAOITemplate("丹江口水库_P.shp");
                case "官厅水库":
                    return new VectorAOITemplate("官厅水库_P.shp");
                case "密云水库":
                    return new VectorAOITemplate("密云水库_P.shp");
                case "郊区农田":
                    return new VectorAOITemplate("郊区农田_P.shp");
                case "洞庭分区":
                    return new VectorAOITemplate("洞庭分区.shp");
                case "纳木错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "纳木错"; }
                        );
                case "色林错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "色林错"; }
                        );
                case "羊卓雍错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "羊卓雍错"; }
                        );
                case "普莫雍错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "普莫雍错"; }
                        );
                case "当惹雍错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "当惹雍错"; }
                        );
                case "扎日南木错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "扎日南木错"; }
                        );
                case "佩枯错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "佩枯错"; }
                        );
                case "塔若错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "塔若错"; }
                        );
                case "玛旁雍错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "玛旁雍错"; }
                        );
                case "拉昂错":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp",
                        (fet) => { return fet.GetFieldValue("name") == "拉昂错"; }
                        );
                case "西藏常用湖泊":
                    return new VectorAOITemplate("西藏常用湖泊区域.shp");
                default:
                    return null;
            }
        }
    }
}
