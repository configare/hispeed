using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public abstract class Command:ICommand
    {
        protected int _id = 0;
        protected string _name = null;
        protected string _text = null;
        protected string _toolTip = null;
        protected ISmartSession _smartSession = null;
   
        public Command()
        { 
        }

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToolTip
        {
            get { return _toolTip; }
        }

        public System.Drawing.Bitmap Bitmap
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsChanged
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Enable
        {
            get { throw new NotImplementedException(); }
        }

        public bool Visible
        {
            get { throw new NotImplementedException(); }
        }

        public int OpertionContextIds
        {
            get { throw new NotImplementedException(); }
        }

        public void Apply(ISmartSession session)
        {
            _smartSession = session;
        }

        public virtual void Execute()
        {
        }

        public virtual void Execute(string argument)
        {

        }

        public virtual void Execute(string argument,params string[] args)
        {

        }
    }
}
