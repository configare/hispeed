using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace GeoDo.FileProject
{
    //[Export(typeof(IFileProjector)),ExportMetadata("VERSION","1")]
    //public class SingleFileProjector:FileProjector
    //{
    //    public SingleFileProjector()
    //        : base()
    //    {
    //        _name = "SINGLEFILE";
    //        _fullname = "单文件投影";
    //    }

    //    public override bool IsSupport(string fileName)
    //    {
    //        return false;
    //    }

    //    public override FilePrjSettings CreateDefaultPrjSettings()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void ComputeDstEnvelope(RSS.Core.DF.IRasterDataProvider srcRaster, Project.ISpatialReference dstSpatialRef, out RasterProject.PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
