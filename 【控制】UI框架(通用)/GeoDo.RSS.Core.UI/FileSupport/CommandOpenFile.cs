using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.Core.UI
{
    [Export(typeof(GeoDo.RSS.Core.UI.ICommand))]
    public class CommandOpenFile : Command
    {
        public CommandOpenFile()
            : base()
        {
            _id = 2000;
            _name = "OpenFile";
            _text = "打开文件";
            _toolTip = "打开文件";
        }

        public override void Execute()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                foreach (string filter in SupportedFileFilters.AllFileFilters)
                    if (string.IsNullOrEmpty(dlg.Filter))
                        dlg.Filter += filter;
                    else
                        dlg.Filter += ("|" + filter);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.Multiselect)
                    {
                        foreach (string fname in dlg.FileNames)
                        {
                            Execute(fname);
                            string ext = Path.GetExtension(fname);
                            if (!string.IsNullOrWhiteSpace(ext) && ext.ToLower() == ".hdf")//限制一次只能打开一个hdf
                            {
                                break;
                            }
                        }
                    }
                    else
                        Execute(dlg.FileName);
                }
            }
        }

        public override void Execute(string argument)
        {
            //argument = argument.ToUpper();
            OpenFileFactory.Open(argument);
        }

        public override void Execute(string argument, params string[] args)
        {
            //argument = argument.ToUpper();
            OpenFileFactory.Open(argument, args);
        }
    }
}