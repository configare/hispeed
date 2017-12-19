using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.CA
{
    public delegate void OnArgEditorApplyClick(object sender,RgbProcessorArg arg);
    public delegate void OnArgEditorOkClick(object sender,RgbProcessorArg arg);
    public delegate void OnArgEditorCancelClick(object sender,RgbProcessorArg arg);
    public delegate void OnArgEditorPreviewing(object sender,RgbProcessorArg arg);
}
