using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    enum enumLDataType
    {
        L_Unknown = 0,
        /*! Eight bit unsigned integer */
        L_Byte = 1,
        /*! Sixteen bit unsigned integer */
        L_UInt16 = 2,
        /*! Sixteen bit signed integer */
        L_Int16 = 3,
        /*! Thirty two bit unsigned integer */
        L_UInt32 = 4,
        /*! Thirty two bit signed integer */
        L_Int32 = 5,
        /*! Thirty two bit floating point */
        L_Float32 = 6,
        /*! Sixty four bit floating point */
        L_Float64 = 7,
        /*! Complex Int16 */
        L_CInt16 = 8,
        /*! Complex Int32 */
        L_CInt32 = 9,
        /*! Complex Float32 */
        L_CFloat32 = 10,
        /*! Complex Float64 */
        L_CFloat64 = 11,
        L_TypeCount = 12,         /* maximum type # + 1 */
        L_RGB = 13,                /* 全色图像 */
        L_ARGB = 14                /* 带有透明度的全色图像 */
    }
}
