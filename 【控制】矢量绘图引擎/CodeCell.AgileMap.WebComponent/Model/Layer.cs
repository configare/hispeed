using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CodeCell.AgileMap.WebComponent
{
    public abstract class Layer:AgileMapEntity
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = null;
        private bool _visible = true;
        protected Map _map = null;

        public Layer()
            : base()
        { 
        }

        public Layer(string id)
            : base()
        {
            _id = id;
        }

        public Map Map
        {
            get { return _map; }
            internal set { _map = value; }
        }

        public string Id 
        {
            get { return _id; }
        }

        public virtual string Name
        {
            get { return _name; }
            set 
            {
                _name = value;
                NotifyPropertyChange("Name");
            }
        }

        public virtual bool Visible
        {
            get { return _visible; }
            set 
            {
                _visible = value;
                NotifyPropertyChange("Visible");
            }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
