using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace CodeCell.Bricks.ModelFabric
{
    internal class BindingEnvironment:IBindingEnvironment,IEventHandler,IInternalSelection
    {
        private IModelEditor _modelEditor = null;
        private List<ActionElement> _actionElements = new List<ActionElement>();
        private List<ActionElementLink> _actionElementLinks = new List<ActionElementLink>();

        public BindingEnvironment(IModelEditor modelEditor)
        {
            _modelEditor = modelEditor;
        }

        public void LoadScriptFile(string scriptfilename)
        {
            string[] dirs = new string[] { AppDomain.CurrentDomain.BaseDirectory};
            ScriptLoader.LoadFrom(dirs,scriptfilename, out _actionElements, out _actionElementLinks);
            _modelEditor.Render();
        }

        public void Add(ActionElement ele)
        {
            ele.Name = GetUniqueName(ele);
            _actionElements.Add(ele);
            if (_actionElements.Count == 3)
            {
                _actionElementLinks.Add(new ActionElementLink(_actionElements[0], _actionElements[1]));
                _actionElementLinks.Add(new ActionElementLink(_actionElements[1], _actionElements[2]));
                _actionElementLinks.Add(new ActionElementLink(_actionElements[2], _actionElements[0]));
            }
        }

        private string GetUniqueName(ActionElement ele)
        {
            string exp = @"(?<NAME>\S*)\((?<ID>\d+)\)$";
            foreach (ActionElement e in _actionElements)
            {
                if (e.Name.ToUpper() == ele.Name.ToUpper())
                {
                    string name = ele.Name;
                    int idx = GetMaxIdx(ele.Name, exp);
                    return name + "("+idx.ToString()+")";
                }
            }
            return ele.Name;
        }

        private int GetMaxIdx(string mainName,string exp)
        {
            int idx = 0;
            foreach (ActionElement e in _actionElements)
            {
                Match m = Regex.Match(e.Name, exp);
                string name = m.Groups["NAME"].Value;
                if (string.IsNullOrEmpty(name))
                    continue;
                if (mainName.ToUpper() != name.ToUpper())
                    continue;
                string id = m.Groups["ID"].Value;
                if (string.IsNullOrEmpty(id))
                    continue;
                int i = int.Parse(id);
                if (i > idx)
                    idx = i;
            }
            return ++idx;
        }

        internal ActionElement[] ActionElements
        {
            get { return _actionElements.ToArray(); }
        }

        internal ActionElementLink[] ActionElementLinks
        {
            get { return _actionElementLinks.ToArray(); }
        }

        #region IBindingEnvironment 成员

        public BindingPair[] GetBindingPairs(IAction action)
        {
            if (_actionElements == null || _actionElements.Count == 0)
                return null;
            foreach (ActionElement ele in _actionElements)
            {
                if (ele.Equals(action))
                    return ele.BindingPairs;
            }
            return null;
        }

        public Dictionary<IAction, PropertyInfo[]> QueryCompatibleProperty(IAction action, BindingAttribute bindingAttribute, PropertyInfo propertyInfo)
        {
            if (_actionElements == null || _actionElements.Count == 0)
                return null;
            Dictionary<IAction, PropertyInfo[]> atts = new Dictionary<IAction, PropertyInfo[]>();
            //
            foreach (ActionElement ele in _actionElements)
            {
                IAction act = ele.Action;
                if (act.Equals(action))
                    continue;
                Dictionary<PropertyInfo, BindingAttribute> ps = ArgBindingHelper.GetBindingProperties(act.GetType());
                List<PropertyInfo> infos = new List<PropertyInfo>();
                foreach (PropertyInfo info in ps.Keys)
                {
                    infos.Clear();
                    BindingAttribute ba = ps[info];
                    if (!bindingAttribute.SemanticType.Equals(ba.SemanticType))
                        continue;
                    if (!propertyInfo.PropertyType.Equals(info.PropertyType))
                        continue;
                    if (ba.Direction == enumBindingDirection.Input || ba.Direction == enumBindingDirection.None)
                        continue;
                    infos.Add(info);
                }
                if(infos.Count>0)
                    atts.Add(act, infos.ToArray());
            }
            return atts;
        }

        public void UpdateBindingPair(IAction act, BindingPair[] bindingPairs)
        {
            if (_actionElements == null || _actionElements.Count == 0 || act == null || bindingPairs == null)
                return;
            foreach (ActionElement ele in _actionElements)
            {
                if (ele.Equals(act))
                {
                    ele.UpdateBindingParis(bindingPairs);
                    break;
                }
            }
            UpdateActionElementLinks();
        }

        private void UpdateActionElementLinks()
        {
        }

        #endregion

        #region IEventHandler 成员

        public void Handle(object sender, enumEventType eventType, object eventArg, EventHandleStatus status)
        {
            switch (eventType)
            {
                case enumEventType.MouseDoubleClick:
                    DoBinding(eventArg as MouseEventArgs,status);
                    break;
            }
        }

        private void DoBinding(MouseEventArgs mouseEventArgs, EventHandleStatus status)
        {
            if(_actionElements == null || _actionElements.Count==0)
                return ;
            Point pt = _modelEditor.ToViewCoord(mouseEventArgs.Location);
            foreach (ActionElement ele in _actionElements)
            {
                if (ele.IsHited(pt))
                {
                    DoBinding(ele.Action);
                    status.Handled = true;
                    break;
                }
            }
        }

        private void DoBinding(IAction action)
        {
            using (frmPropertyEditorDialog dlg = new frmPropertyEditorDialog())
            {
                IPropertyEditorDialog editor = dlg as IPropertyEditorDialog;
                dlg.ShowDialog(action, this as IBindingEnvironment);
            }
        }

        #endregion

        #region IInternalSelection 成员

        public void ClearSelection()
        {
            if (_actionElements == null || _actionElements.Count == 0)
                return;
            foreach(ActionElement ele in _actionElements)
                ele.IsSelected = false ;
        }

        public ActionElement[] Selection
        {
            get 
            {
                if (_actionElements == null || _actionElements.Count == 0)
                    return null;
                List<ActionElement> eles = new List<ActionElement>();
                foreach (ActionElement ele in _actionElements)
                    if (ele.IsSelected)
                        eles.Add(ele);
                return eles.Count > 0 ? eles.ToArray() : null;
             }
        }

        public ActionElement Select(Point point)
        {
            if (_actionElements == null || _actionElements.Count == 0)
                return null;
            if (Control.ModifierKeys != Keys.Shift)
                ClearSelection();
            foreach (ActionElement ele in _actionElements)
                if (ele.IsHited(point))
                {
                    ele.IsSelected = true;
                    return ele;
                }
            return null;
        }

        public ActionElement GetActionElementAt(Point point,out int anchorIndex) 
        {
            anchorIndex = -1;
            if (_actionElements == null || _actionElements.Count == 0)
                return null;
            foreach (ActionElement ele in _actionElements)
                if (ele.IsHitedAnchor(point, out anchorIndex))
                    if (anchorIndex != -1)
                        return ele;
            return null;
        }

        public ActionElement[] Select(RectangleF rectangle)
        {
            if (_actionElements == null || _actionElements.Count == 0)
                return null;
            if (Control.ModifierKeys != Keys.Shift)
                ClearSelection();
            List<ActionElement> eles = new List<ActionElement>();
            foreach (ActionElement ele in _actionElements)
            {
                if (rectangle.IntersectsWith(new RectangleF(ele.Location, ele.Size)))
                {
                    ele.IsSelected = true;
                    eles.Add(ele);
                }
            }
            return eles.Count > 0 ? eles.ToArray() : null;
        }

        #endregion
    }
}
