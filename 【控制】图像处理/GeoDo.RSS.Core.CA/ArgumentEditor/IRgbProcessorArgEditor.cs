using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.CA
{
    public interface IRgbProcessorArgEditor
    {
        bool IsSupport(Type type);
        OnArgEditorApplyClick OnApplyClicked { get; set; }
        OnArgEditorCancelClick OnCancelClicked { get; set; }
        OnArgEditorOkClick OnOkClicked { get; set; }
        OnArgEditorPreviewing OnPreviewing { get; set; }
        IRgbProcessor Processor { get; set; }
        void Init(IRgbArgEditorEnvironmentSupport env,IRgbProcessor processor);
        void Show(RgbProcessorArg arg);
        DialogResult ShowDialog(RgbProcessorArg arg);
    }
}
