using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    public static class ProjectionTransformFactory
    {
        public static IProjectionTransform GetDefault()
        {
            return new PrjTranSimpleEquidistantCyclindrical();
        }

        public static IProjectionTransform GetProjectionTransform(ISpatialReference srcSpatialRef, ISpatialReference dstSpatialRef)
        {
            if (dstSpatialRef == null && srcSpatialRef.ProjectionCoordSystem == null)//GLL
                return new PrjTranSimpleEquidistantCyclindrical();
            return new ProjectionTransform(srcSpatialRef, dstSpatialRef);
        }
    }
}
