/* 封装对HDF4数据读取
 * 通用光栅图像（GR API）
 * 科学数据集（SD API）
 * 虚拟数据Vdata（VS API）
 * 注解Annotation
 * 虚拟组合Vgroup（V API）
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public interface ICloudSatDataProvider : IDisposable
    {

        object ReadVData();

        

        double[] ReadDataSet(string dsName);
    }
}
