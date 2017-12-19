/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GeoDo.RSS.CA
{
    /// <summary>
    /// Curve control specialization for RGB curves
    /// </summary>
    public sealed class CurveControlRgb
        : CurveControl
    {
        public CurveControlRgb()
            : base(3, 256)
        {
            this.mask = new bool[3] { true, false, false };
            visualColors = new ColorBgra[] {
                                               //ColorBgra.Black,
                                               ColorBgra.Red,
                                               ColorBgra.Green,
                                               ColorBgra.Blue
                                           };
            channelNames = new string[]{
                //"RGB",
                "Red",
                "Green",
                "Blue"
            };
            ResetControlPoints();
        }

        public override ColorTransferMode ColorTransferMode
        {
            get
            {
                return ColorTransferMode.Rgb;
            }
        }
    }
}
