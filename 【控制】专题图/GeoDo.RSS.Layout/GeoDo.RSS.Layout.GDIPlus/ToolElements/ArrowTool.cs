using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class ArrowTool : ControlTool
    {
        enum enumAction
        {
            SelectByRect,
            MovingElement,
            EditAnchor,
            None
        }
        private enumAction _action = enumAction.None;
        private PointF _beginPoint;
        private Point _beginPointScreen;
        private Point _crtPointScreen;
        private ISizableElement _editedElement;
        private ISelectedEditBox _editedBox;
        private int _anchorIndex = -1;
        private ILayoutHost _currentHost = null;
        private TextElement _txtEle = null;

        public ArrowTool()
        {
        }

        public override Cursor Cursor
        {
            get
            {
                return Cursors.Arrow;
            }
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null || _action != enumAction.SelectByRect)
                return;
            g.DrawRectangle(Pens.CornflowerBlue, Math.Min(_beginPointScreen.X, _crtPointScreen.X),
                            Math.Min(_beginPointScreen.Y, _crtPointScreen.Y),
                            Math.Abs(_beginPointScreen.X - _crtPointScreen.X),
                            Math.Abs(_beginPointScreen.Y - _crtPointScreen.Y));
        }

        public override void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
            ILayoutHost host = sender as ILayoutHost;
            switch (eventType)
            {
                case enumCanvasEventType.DoubleClick:
                    HandleDoubleClick(host, e);
                    break;
                case enumCanvasEventType.KeyDown:
                    HandleKey(host, e);
                    break;
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        _isMoving = false;
                        _beginPoint = new PointF(e.LayoutX, e.LayoutY);
                        //by chennan 20131107 修改图元无法点选选中
                        HitByPoint(sender as ILayoutHost, e.LayoutX, e.LayoutY);

                        _editedElement = GetEditedElementFromSelection(sender as ILayoutHost, e);
                        if (_editedElement == null)
                        {
                            HitByPoint(sender as ILayoutHost, e.LayoutX, e.LayoutY);
                            _editedElement = GetEditedElementFromSelection(sender as ILayoutHost, e);
                        }
                        if (_editedElement != null)
                        {
                            if (_anchorIndex != -1)
                                _action = enumAction.EditAnchor;
                            else
                                _action = enumAction.MovingElement;
                        }
                        else
                        {
                            _beginPointScreen.X = e.ScreenX;
                            _beginPointScreen.Y = e.ScreenY;
                            _action = enumAction.SelectByRect;
                        }
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_action == enumAction.SelectByRect)
                    {
                        _isMoving = false;
                        //   _isMultiSelect = false;
                        _crtPointScreen.X = e.ScreenX;
                        _crtPointScreen.Y = e.ScreenY;
                        host.Render();
                    }
                    else if (_action == enumAction.MovingElement)
                    {
                        if (_editedElement.IsLocked)
                            break;
                        _isMultiSelect = false;
                        MovingElement(host, e);
                    }
                    else if (_action == enumAction.EditAnchor)
                    {
                        if (_editedElement.IsLocked)
                            break;
                        _isMoving = false;
                        //  _isMultiSelect = false;
                        host.Container.Cursor = GetCursorByAnchor();
                        _editedBox.Apply(host.LayoutRuntime, _anchorIndex, _beginPoint, new PointF(e.LayoutX, e.LayoutY));
                        _beginPoint.X = e.LayoutX;
                        _beginPoint.Y = e.LayoutY;
                        host.Render();
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    if (_beginPoint.IsEmpty)
                        return;
                    if ((e.LayoutX - _beginPoint.X) < float.Epsilon && (e.LayoutY - _beginPoint.Y) < float.Epsilon)
                    {
                        bool isEmpty = HitByPoint(sender as ILayoutHost, e.LayoutX, e.LayoutY);
                        //by chennan 20131107 多选时未点选到正确位置不去除之前已选取的图元
                        if (isEmpty && ((Control.ModifierKeys & Keys.Shift) != Keys.Shift))
                            host.LayoutRuntime.ClearSelection();
                        else
                            ClearOtherSelection(host, e);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(Math.Min(_beginPoint.X, e.LayoutX),
                                                                            Math.Min(_beginPoint.Y, e.LayoutY),
                                                                            Math.Abs(_beginPoint.X - e.LayoutX),
                                                                            Math.Abs(_beginPoint.Y - e.LayoutY));
                        HitByRect(sender as ILayoutHost, rect);
                    }
                    host.Render();
                    _action = enumAction.None;
                    _anchorIndex = -1;
                    host.Container.Cursor = Cursor;
                    break;
            }
        }

        bool _isMoving = false;
        private void MovingElement(ILayoutHost host, CanvasEventArgs e)
        {
            host.Container.Cursor = Cursors.SizeAll;
            IElement[] selections = host.LayoutRuntime.Selection;
            if (selections == null)
                return;
            _isMoving = true;
            foreach (IElement ele in selections)
            {
                if (!ele.IsSelected)
                    continue;
                if (ele is SizableElementGroup)
                    (ele as SizableElementGroup).ApplyLocation(e.LayoutX - _beginPoint.X, e.LayoutY - _beginPoint.Y);
                else if (ele is SizableElement)
                {
                    IElement group = host.LayoutRuntime.Layout.FindParent(ele);
                    if (group != null && group is ISizableElementGroup)
                        (group as ISizableElementGroup).ApplyLocationByItemSelected(e.LayoutX - _beginPoint.X, e.LayoutY - _beginPoint.Y);
                    (ele as SizableElement).ApplyLocation(e.LayoutX - _beginPoint.X, e.LayoutY - _beginPoint.Y);
                }
            }
            _beginPoint.X = e.LayoutX;
            _beginPoint.Y = e.LayoutY;
            host.Render();
        }

        #region 双击文本元素出现TextBox
        int _originWid = 0;
        float _scale = 1;
        private void HandleDoubleClick(ILayoutHost host, CanvasEventArgs e)
        {
            if (host == null)
                return;
            ILayoutRuntime runtime = host.LayoutRuntime;
            if (runtime == null)
                return;
            IElement[] eles = runtime.Hit(e.LayoutX, e.LayoutY);
            if (eles == null || eles.Length == 0)
                return;
            _scale = runtime.Scale;
            foreach (IElement ele in eles)
            {
                if (ele is TextElement)
                {
                    if (ele.IsLocked)
                        continue;
                    _currentHost = host;
                    _txtEle = ele as TextElement;
                    _txtEle.Visible = false;
                    TextBox box = CreatTextbox(host);
                    return;
                }
                else if (ele is MultlineTextElement)
                {
                    if (ele.IsLocked)
                        continue;
                    _currentHost = host;
                    CreatMultlineTextbox(host, ele as MultlineTextElement);
                    return;
                }
            }
        }

        private TextBox CreatTextbox(ILayoutHost host)
        {
            TextBox box = new TextBox();
            box.Text = null;
            box.Visible = true;
            box.Capture = true;
            box.Font = new Font(_txtEle.Font.FontFamily, _txtEle.Font.Size * _scale); //_txtEle.Font;
            box.Text = _txtEle.Text;
            box.BorderStyle = BorderStyle.None;
            float x = _txtEle.Location.X;
            float y = _txtEle.Location.Y;
            host.LayoutRuntime.Layout2Screen(ref x, ref y);
            box.Location = new Point((int)x + LayoutControl.RULER_HEIGHT + 4, (int)y + LayoutControl.RULER_HEIGHT + 2);
            box.Width = (int)(_txtEle.Size.Width);
            _currentHost.Container.Controls.Add(box);
            box.BringToFront();
            box.Focus();  //必须在其它的属性设置完成之后设置焦点才有作用
            box.KeyDown += new KeyEventHandler(box_KeyDown);
            box.LostFocus += new EventHandler(box_LostFocus);
            box.TextChanged += new EventHandler(box_TextChanged); //设置文本框的宽随文本的宽度改变
            _originWid = box.Width;
            return box;
        }

        void box_TextChanged(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            System.Drawing.Graphics grp = System.Drawing.Graphics.FromHwnd(box.Handle);
            int width = (int)grp.MeasureString(box.Text, box.Font).Width;
            grp.Dispose();
            if (width > _originWid)
                box.Width = width;
        }

        void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetTextFromTextBox(sender as TextBox);
            }
        }

        void box_LostFocus(object sender, EventArgs e)
        {
            GetTextFromTextBox(sender as TextBox);
        }

        private void GetTextFromTextBox(TextBox box)
        {
            string text = box.Text;
            if (_currentHost == null)
                return;
            if (_txtEle == null)
                return;
            if (String.IsNullOrEmpty(text))
            {
                _currentHost.Container.Controls.Remove(box);
                _txtEle.Visible = true;
                _currentHost.Render();
                return;
            }
            _currentHost.Container.Controls.Remove(box);
            _txtEle.Text = text;
            _txtEle.Visible = true;
            _txtEle.ApplySize(box.Width - _txtEle.Size.Width * _scale, 0);
            _currentHost.Render();
        }

        #region 多行文本编辑
        private void CreatMultlineTextbox(ILayoutHost host, MultlineTextElement multlineTextElement)
        {
            multlineTextElement.Visible = false;
            TextBox txtBox = new TextBox();
            txtBox.Tag = multlineTextElement;
            txtBox.Multiline = true;
            txtBox.Capture = true;
            txtBox.Font = new Font(multlineTextElement.Font.FontFamily, multlineTextElement.Font.Size * _scale); //_txtEle.Font;
            txtBox.Text = multlineTextElement.Text;
            txtBox.BorderStyle = BorderStyle.FixedSingle;
            txtBox.ForeColor = multlineTextElement.Color;
            float x = multlineTextElement.Location.X;
            float y = multlineTextElement.Location.Y;
            host.LayoutRuntime.Layout2Screen(ref x, ref y);
            txtBox.Location = new Point((int)x + LayoutControl.RULER_HEIGHT + 4, (int)y + LayoutControl.RULER_HEIGHT + 2);
            txtBox.Width = (int)(multlineTextElement.Size.Width * _scale);
            txtBox.Height = (int)(multlineTextElement.Size.Height * _scale);
            _currentHost.Container.Controls.Add(txtBox);
            txtBox.BringToFront();
            txtBox.Focus();  //必须在其它的属性设置完成之后设置焦点才有作用
            //txtBox.KeyDown += new KeyEventHandler(txtBox_KeyDown);
            txtBox.TextChanged += new EventHandler(txtBox_TextChanged); //设置文本框的宽随文本的宽度改变
            txtBox.LostFocus += new EventHandler(txtBox_LostFocus);
            _originWid = txtBox.Width;
        }

        void txtBox_TextChanged(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            MultlineTextElement ele = box.Tag as MultlineTextElement;
            if (ele == null)
                return;
            //System.Drawing.Graphics grp = System.Drawing.Graphics.FromHwnd(box.Handle);
            //int width = (int)ele.Size.Width;//(int)grp.MeasureString(box.Text, box.Font).Width;
            //grp.Dispose();
            //if (width > _originWid)
            //    box.Width = width;
        }

        //void txtBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    RichTextBox box = sender as RichTextBox;
        //    if (e.Alt)
        //    {
        //        Keys keys = e.KeyData & Keys.KeyCode;
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            box.AppendText("\r\n");
        //            e.SuppressKeyPress = false;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            GetTextFromMultilineTextBox(sender as RichTextBox);
        //        }
        //    }
        //}

        void txtBox_LostFocus(object sender, EventArgs e)
        {
            GetTextFromMultilineTextBox(sender as TextBox);
        }

        private void GetTextFromMultilineTextBox(TextBox box)
        {
            string text = box.Text;
            if (_currentHost == null)
                return;
            MultlineTextElement ele = box.Tag as MultlineTextElement;
            if (ele == null)
                return;
            if (String.IsNullOrEmpty(text))
            {
                _currentHost.Container.Controls.Remove(box);
                _txtEle.Visible = true;
                _currentHost.Render();
                return;
            }
            _currentHost.Container.Controls.Remove(box);
            ele.Text = text;
            ele.Visible = true;
            //ele.ApplySize(box.Width - _txtEle.Size.Width * _scale, 0);
            _currentHost.Render();
            box.Tag = null;
            box.Clear();
            box.Visible = false;
            box = null;
        }
        #endregion
        #endregion

        private void HandleKey(ILayoutHost host, CanvasEventArgs eventArg)
        {
            PreviewKeyDownEventArgs e = eventArg.E as PreviewKeyDownEventArgs;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                TryAllArrowKeyMove(host, e);
            }
            else if (e.KeyCode == Keys.Delete)
                DeleteSelectedElements(host);
            else if (e.KeyCode == Keys.R)
                ResetOSize(host);
        }

        private void ResetOSize(ILayoutHost host)
        {
            IElement[] selection = host.LayoutRuntime.Selection;
            if (selection == null)
                return;
            var v = selection.Where<IElement>((ele) => { return ele is PictureElement; });
            if (v == null)
                return;
            foreach (PictureElement pic in v)
                pic.ResetOSize();
            host.Render();
        }

        private void DeleteSelectedElements(ILayoutHost host)
        {
            IElement[] selection = host.LayoutRuntime.Selection;
            if (selection == null)
                return;
            ILayout layout = host.LayoutRuntime.Layout;
            foreach (IElement ele in selection)
                DeleteElement(ele, layout);
            host.Render();
        }

        private void DeleteElement(IElement ele, ILayout layout)
        {
            if (ele.IsLocked)
                return;
            IElementGroup group = layout.FindParent(ele) as IElementGroup;
            if (group != null)
            {
                group.Elements.Remove(ele);
                ele.Dispose();
            }
        }

        private void TryAllArrowKeyMove(ILayoutHost host, PreviewKeyDownEventArgs e)
        {
            int step = 1;
            if (Control.ModifierKeys == Keys.Shift)
                step = 5;
            float x = step, y = 0;
            host.LayoutRuntime.Pixel2Layout(ref x, ref y);
            //
            IElement[] selection = host.LayoutRuntime.Selection;
            if (selection != null)
            {
                foreach (ISizableElement ele in selection)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                            ele.ApplyLocation(0, -step);
                            break;
                        case Keys.Down:
                            ele.ApplyLocation(0, step);
                            break;
                        case Keys.Right:
                            ele.ApplyLocation(step, 0);
                            break;
                        case Keys.Left:
                            ele.ApplyLocation(-step, 0);
                            break;
                    }
                }
            }
            host.Render();
        }

        private Cursor GetCursorByAnchor()
        {
            switch (_anchorIndex)
            {
                case 0://left-up
                case 4://right-down
                    return Cursors.SizeNWSE;
                case 1://up
                case 5://down
                    return Cursors.SizeNS;
                case 3://right
                case 7://left
                    return Cursors.SizeWE;
                case 2://right-up
                case 6://left-down
                    return Cursors.SizeNESW;
            }
            return Cursors.Default;
        }

        private ISizableElement GetEditedElementFromSelection(ILayoutHost host, CanvasEventArgs e)
        {
            IElement[] selection = host.LayoutRuntime.Selection;
            if (selection == null)
                return null;
            foreach (IElement ele in selection)
            {
                if (!ele.IsSelected)
                    continue;
                _editedBox = host.LayoutRuntime.SelectedEditBoxManager.Get(ele as ISizableElement);
                if (_editedBox != null)
                    _anchorIndex = _editedBox.IndexOfAnchor(e.ScreenX, e.ScreenY);
                PreviewKeyDownEventArgs pe = e.E as PreviewKeyDownEventArgs;
                if (pe != null && pe.KeyCode == Keys.ShiftKey && _anchorIndex != -1)
                    _isMultiSelect = true;
                if (_anchorIndex != -1 || ele.IsHited(e.LayoutX, e.LayoutY))
                {
                    ele.IsSelected = true;
                    return ele as ISizableElement;
                }
            }
            return null;
        }

        bool _isMultiSelect = false;
        private void ClearOtherSelection(ILayoutHost host, CanvasEventArgs e)
        {
            IElement[] selection = host.LayoutRuntime.Selection;
            if (selection == null)
                return;
            if (_editedElement == null)
                return;
            if (_isMoving)
                return;
            foreach (IElement ele in selection)
            {
                if (!ele.IsSelected)
                    continue;
                if (!ele.Equals(_editedElement))
                {
                    // by chennan 20131107 支持同时按下Shift进行多图元选择
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        continue;
                    else
                    {
                        PreviewKeyDownEventArgs pe = e.E as PreviewKeyDownEventArgs;
                        if (pe == null)
                            ele.IsSelected = false;
                        else if (pe.KeyCode != Keys.ShiftKey)
                            ele.IsSelected = false;
                    }
                    //PreviewKeyDownEventArgs pe = e.E as PreviewKeyDownEventArgs;
                    //if (pe == null)
                    //    ele.IsSelected = false;
                    //else if (pe.KeyCode != Keys.ShiftKey)
                    //    ele.IsSelected = false;
                }
            }
        }

        private void HitByRect(ILayoutHost host, RectangleF rect)
        {
            IElement[] eles = host.LayoutRuntime.Hit(rect);
            SetStatus((ele) => { ele.IsSelected = true; }, eles);
        }

        private bool HitByPoint(ILayoutHost host, float layoutX, float layoutY)
        {
            IElement[] eles = host.LayoutRuntime.Hit(layoutX, layoutY);
            SetStatus((ele) => { ele.IsSelected = true; }, eles);
            return eles == null || eles.Length == 0;
        }

        private void SetStatus(Action<IElement> setter, IElement[] eles)
        {
            if (eles == null)
                return;
            foreach (IElement ele in eles)
                setter(ele);
        }
    }
}
