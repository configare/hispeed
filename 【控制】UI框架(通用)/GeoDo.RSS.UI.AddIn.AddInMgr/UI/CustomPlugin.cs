using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeoDo.RSS.UI.AddIn.AddInMgr
{
    public class CustomPlugin : INotifyPropertyChanged,IEditableObject
    {
        private string _index;
        private string _title = null;
        private string _command = null;
        private string _parameter = null;
        private string _initDir = null;

        public CustomPlugin(string title)
            :this(title,null,null,null)
        {
        }

        public CustomPlugin( string title, string command, string parameter, string initDir)
        {
            _title = title;
            _command = command;
            _parameter = parameter;
            _initDir = initDir;
        }

        public CustomPlugin(string index, string title, string command, string parameter, string initDir)
            :this(title,command,parameter,initDir)
        {
            _index = index;

        }
        //索引号
        public string Index
        {
            get
            {
                return this._index;
            }
            set
            {
                this._index = value;
            }

        }
        //标题
        public string Title
        {
            get 
            { 
                return this._title; 
            }
            set 
            {  
                this._title = value; 
            }
        }
        //命令
        public string Command
        {
            get
            {
                return this._command;
            }
            set
            {
                this._command = value;
            }
        }
        //参数
        public string Parameter
        {
            get
            {
                return this._parameter;
            }
            set
            {
                this._parameter = value;
            }
        }
        //初始目录
        public string InitDir
        {
            get
            {
                return this._initDir;
            }
            set
            {
                this._initDir = value;
            }

        }

        public override string ToString()
        {
            return this.Title;
        }

        public void BeginEdit()
        {
          
        }

        public void CancelEdit()
        {
          
        }

        public void EndEdit()
        {
         
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
