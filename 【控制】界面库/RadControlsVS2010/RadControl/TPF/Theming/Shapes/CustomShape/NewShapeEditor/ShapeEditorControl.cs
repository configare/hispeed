using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Telerik.WinControls
{
    [ToolboxItemAttribute(false)]
    public partial class RadShapeEditorControl : UserControl
    {
        #region Consts and Enums
        const float snapDistConst = 10f;

        [FlagsAttribute, CLSCompliant(false)]
        public enum SnapTypes : uint
        {
            SnapToGrid = 0x00000001,
            SnapToCtrl = 0x00000002,
            SnapToCurves = 0x00000004,
            SnapToExtensions = 0x00000008
        };

        #endregion

        #region Private Data

        private Rectangle dimension;

        private ShapeLinesCollection shape;
        private SnapToGrid snapToGrid;
        private ShapeEditorZoom zoom;

        private PointF snappedPoint;
        private ShapePoint snappedCtrlPoint = null;
        private IShapeCurve snappedCurve = null;

        private uint snapStates;        // set of flags defining active snappings

        private bool snappingOccured = false;
        private bool drawsGuideLines = true;
        private bool isDragging = false;
        private bool isScrolling = false;
        private PointF scrollStartPos;
        private PointF areaStartPos;

        #region Control events

        public event SnapChangedEventHandler SnapChanged;
        public event ZoomChangedEventHandler ZoomChanged;

        #endregion

        #region Precached pens, brushes, etc.

        private Pen gridDrawPen;

        private Pen PenSnappedPoint;
        private Pen PenSnappedCtrlPoint;
        private Pen PenSnappedCtrlPointLocked;
        private Pen PenSnappedPointLine;
        private Pen PenSnappedLine;
        private Pen PenShape;
        private Brush BrushControlPoints;

        private Pen PenControlLines;
        private Pen PenLinesExtensions;

        private Brush BrushControlLinesPts;

        private Brush DimensionRectFill;
        private Brush DimensionHelperRects;
        private Pen DimensionRectLines;

        #endregion

        private Timer scrollTimer; // scrolls the view while mouse is out of bounds
        //private Timer selectTimer; // waits before start moving the selected point

        private object SelectedProperty
        {
            get
            {
                if (propertyGrid == null) return null;

                return propertyGrid.SelectedObject;
            }
            set
            {
                if (propertyGrid == null) return;

                propertyGrid.SelectedObject = value;
            }
        }

        #endregion

        #region Accessors

        public ShapeLinesCollection Shape
        {
            get { return shape; }
            set
            {
                if (value == null) return;
                shape = value;
            }
        }

        public Rectangle Dimension
        {
            get { return dimension; }
            set { dimension = value;  }
        }

        public PropertyGrid propertyGrid = null;

        public float GridSize
        {
            get { return snapToGrid.FieldWidth; }
            set
            {
                if ((value < 10f) || (value > 500f)) return;

                snapToGrid.FieldWidth = value;
                Refresh();
            }
        }

        #endregion

        #region Temporary debug variables and methods

        internal bool debugMode = false;

        internal string scrolls = null;
        internal string mousePos = null;

        private  void StopOnDebug()
        {
            if (debugMode)
            {
                debugMode = false;
            }
        }

        private void PrintDebugInformation(PaintEventArgs e)
        {
            if (scrolls != null)
                e.Graphics.DrawString(scrolls,
                    new Font("Verdana", 12, FontStyle.Bold, GraphicsUnit.Pixel, 0),
                    new SolidBrush(Color.Green), new PointF(0, 0));

            if (mousePos != null)
                e.Graphics.DrawString(mousePos,
                    new Font("Verdana", 12, FontStyle.Bold, GraphicsUnit.Pixel, 0),
                    new SolidBrush(Color.Green), new PointF(0, 16));

        }

        #endregion

        #region Constructors

        public RadShapeEditorControl()
        {
            InitializeComponent();

            InitPensAndBrushes();

            InitZoomAndAutoScroll();

            InitSnapToGrid();

            InitShape();

            Cursor = Cursors.Cross;

            SetSnapping(
                SnapTypes.SnapToCtrl |
                SnapTypes.SnapToCurves |
                SnapTypes.SnapToExtensions |
                SnapTypes.SnapToGrid
                );

            base.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ContainerControl |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, 
                true
                );

            InitScrollTimer();
        }

        public void Reset()
        {
            snappingOccured = false;
            drawsGuideLines = true;
            isDragging = false;
            isScrolling = false;
            snappedPoint = new PointF(0,0);
            snappedCtrlPoint = null;
            snappedCurve = null;
            zoom.ZoomFactor = 1f;
            snapToGrid.SnapRelative = 0.2f;

            SetSnapping(
                SnapTypes.SnapToCtrl |
                SnapTypes.SnapToCurves |
                SnapTypes.SnapToExtensions |
                SnapTypes.SnapToGrid
                );

            scrollStartPos = new PointF(0, 0);
            areaStartPos = new PointF(0, 0);
        }


        #endregion

        #region Snapping accessors 

        public bool GridSnap
        {
            get { return IsSnappingActive(SnapTypes.SnapToGrid); }
            set 
            {
                if (value == IsSnappingActive(SnapTypes.SnapToGrid)) return;

                if (value)
                    SetSnapping(SnapTypes.SnapToGrid); 
                else
                    ClrSnapping(SnapTypes.SnapToGrid);

                OnSnapChanged(new SnapChangedEventArgs(SnapTypes.SnapToGrid));

                Refresh();
            }
        }

        public bool CtrlPointsSnap
        {
            get { return IsSnappingActive(SnapTypes.SnapToCtrl); }
            set 
            {
                if (value == IsSnappingActive(SnapTypes.SnapToCtrl)) return;

                if (value)
                    SetSnapping(SnapTypes.SnapToCtrl); 
                else
                    ClrSnapping(SnapTypes.SnapToCtrl);

                OnSnapChanged(new SnapChangedEventArgs(SnapTypes.SnapToCtrl));

                Refresh();
            }
        }

        public bool CurvesSnap
        {
            get { return IsSnappingActive(SnapTypes.SnapToCurves); }
            set 
            {
                if (value == IsSnappingActive(SnapTypes.SnapToCurves)) return;

                if (value)
                    SetSnapping(SnapTypes.SnapToCurves); 
                else
                    ClrSnapping(SnapTypes.SnapToCurves);

                OnSnapChanged(new SnapChangedEventArgs(SnapTypes.SnapToCurves));

                Refresh();
            }
        }

        public bool ExtensionsSnap
        {
            get { return IsSnappingActive(SnapTypes.SnapToExtensions); }
            set
            {
                if (value == IsSnappingActive(SnapTypes.SnapToExtensions)) return;

                if (value)
                    SetSnapping(SnapTypes.SnapToExtensions);
                else
                    ClrSnapping(SnapTypes.SnapToExtensions);

                OnSnapChanged(new SnapChangedEventArgs(SnapTypes.SnapToExtensions));

                Refresh();
            }
        }

        #endregion

        #region Public Methods

        public float Zoom(PointF center, float factor)
        {
            if (factor == zoom.ZoomFactor) return factor;

            float oldZoomFactor = zoom.ZoomFactor;

            PointF centerPoint = zoom.VisibleToPt(center);

            if (!zoom.Zoom(factor)) return zoom.ZoomFactor;

            snapToGrid.SnapRelative = 0.2f / zoom.ZoomFactor;

            zoom.ZoomAtPoint(centerPoint, center);

            if (oldZoomFactor != zoom.ZoomFactor)
                OnZoomChanged(new ZoomChangedEventArgs(zoom.ZoomFactor));

            Refresh();

            return zoom.ZoomFactor;
        }

        public float ZoomCenter(float factor)
        {
            return Zoom(new PointF(zoom.VisibleArea.Width/2, zoom.VisibleArea.Height/2), factor);
        }

        #endregion

        #region Drawing and Drawing Helper methods

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawDimensionRectAndGrid(e);

            DrawExtensions(e);

            DrawShape(e);

            DrawSnappedPoint(e);

            //PrintDebugInformation(e);
        }

        private void DrawGrid(PaintEventArgs e)
        {
            float step = snapToGrid.FieldWidth * zoom.ZoomFactor;

            Point fromV = new Point();
            Point toV = new Point();
            Point fromH = new Point();
            Point toH = new Point();

            fromH.X = 0;
            toH.X = ClientRectangle.Width;

            fromV.Y = 0;
            toV.Y = ClientRectangle.Height;

            Size offset = new Size(-(zoom.Location.X % (int)Math.Round(step)), -(zoom.Location.Y % (int)Math.Round(step)));

            int numLines = Math.Max(ClientRectangle.Width, ClientRectangle.Height);
            for (float i = 0; i < numLines + step; i += step)
            {
                toH.Y = fromH.Y = (int)i + offset.Height;
                toV.X = fromV.X = (int)i + offset.Width;
                e.Graphics.DrawLine(gridDrawPen, fromH, toH);
                e.Graphics.DrawLine(gridDrawPen, fromV, toV);
            }
        }

        private void DrawSnappedPoint(PaintEventArgs e)
        {
            if ((!snappingOccured) || (isScrolling)) return;

            PointF visiblePt = zoom.PtToVisible(snappedPoint);

            Rectangle r = new Rectangle();
            r.Location = new Point((int)Math.Round(visiblePt.X), (int)Math.Round(visiblePt.Y));

            e.Graphics.DrawLine(PenSnappedPointLine, r.Location, PointToClient(MousePosition));

            r.Inflate(2, 2);

            //StopOnDebug();

            e.Graphics.DrawEllipse(
                snappedCtrlPoint == null ? PenSnappedPoint :
                snappedCtrlPoint.IsLocked ? PenSnappedCtrlPointLocked : PenSnappedCtrlPoint,
                r);
        }

        private void DrawDimensionRectAndGrid(PaintEventArgs e)
        {
            RectangleF r;
            r = zoom.RectToVisible(dimension);

            PointF mid = new PointF(r.X + r.Width / 2, r.Y + r.Height /2);
            
            //draw the black rectangle denoting the clipping rectangle of the control area

            e.Graphics.FillRectangle(DimensionRectFill, r.X, r.Y, r.Width, r.Height);

            DrawGrid(e);
            
            e.Graphics.DrawRectangle(DimensionRectLines, r.X, r.Y, r.Width, r.Height);

            e.Graphics.FillRectangle(DimensionHelperRects, mid.X - 4, r.Y - 4, 8, 4);
            e.Graphics.FillRectangle(DimensionHelperRects, mid.X - 4, r.Bottom, 8, 4);
            e.Graphics.FillRectangle(DimensionHelperRects, r.X - 4, mid.Y - 4, 4, 8);
            e.Graphics.FillRectangle(DimensionHelperRects, r.Right, mid.Y - 4, 4, 8);

        }

        private void DrawShape(PaintEventArgs e)
        {
            for (int i = 0; i < shape.Lines.Count; i++)
            {
                ShapePoint[] curvePoints = shape.Lines[i].Points;

                Pen currentPen  = shape.Lines[i] == snappedCurve ? PenSnappedLine : PenShape;

                for (int j = 0; j < curvePoints.Length - 1; j++)
                {
                    e.Graphics.DrawLine(currentPen, zoom.PtToVisible(curvePoints[j].Location), zoom.PtToVisible(curvePoints[j + 1].Location));
                }
                if (drawsGuideLines)
                {
                    DrawEndPoint(e, shape.Lines[i].LastPoint);
                    DrawEndPoint(e, shape.Lines[i].LastPoint);
                }
                if (shape.Lines[i] is ShapeBezier)
                    DrawControlPoints(e, shape.Lines[i] as ShapeBezier);
            }
        }

        private void DrawEndPoint(PaintEventArgs e, ShapePoint pt)
        {
            RectangleF r = new RectangleF();
            r.Location = zoom.PtToVisible(pt.Location);
            r.Inflate(2, 2);
            e.Graphics.FillEllipse(BrushControlPoints, r);
        }

        private void DrawControlPoints(PaintEventArgs e, ShapeBezier b)
        {
            //if (!drawGuideLines) return;
            RectangleF rect = new RectangleF();
            rect.Location = zoom.PtToVisible(b.ControlPoints[1].Location);
            
            rect.Inflate(2, 2);
            
            e.Graphics.DrawLine(PenControlLines, 
                zoom.PtToVisible(b.ControlPoints[0].Location), 
                zoom.PtToVisible(b.ControlPoints[1].Location)
                );

            e.Graphics.FillEllipse(BrushControlLinesPts, rect);

            rect.Width = rect.Height = 0;
            rect.Location = zoom.PtToVisible(b.ControlPoints[2].Location);
            rect.Inflate(2, 2);

            e.Graphics.DrawLine(PenControlLines, 
                zoom.PtToVisible(b.ControlPoints[3].Location),
                zoom.PtToVisible(b.ControlPoints[2].Location)
                );

            e.Graphics.FillEllipse(BrushControlLinesPts, rect);
        }

        private void DrawExtensions(PaintEventArgs e)
        {
            if (!IsSnappingActive(SnapTypes.SnapToExtensions) || (!drawsGuideLines)) return;

            PointF from = new PointF();
            PointF to = new PointF();

            for (int i = 0; i < shape.Lines.Count; i++)
            {
                for (int j = 0; j < shape.Lines[i].Extensions.Length; j += 2)
                {
                    from = zoom.PtToVisible(shape.Lines[i].Extensions[j].Location);
                    to = zoom.PtToVisible(shape.Lines[i].Extensions[j + 1].Location);

                    ExtendLineToBorders(ref from, ref to);
                    e.Graphics.DrawLine(PenLinesExtensions, from, to);
                }
            }
        }

        private void ExtendLineToBorders(ref PointF from, ref PointF to)
        {
            float m;

            PointF d = new PointF(to.X - from.X, to.Y - from.Y);
            if (d.X == 0)
            {
                from.Y = 0;
                to.Y = ClientRectangle.Height;
                return;
            }

            if (d.Y == 0)
            {
                from.X = 0;
                to.X = ClientRectangle.Width;
                return;
            }

            m = d.X / d.Y;

            if (Math.Abs(m) <= 1)
            {
                from.X -= m * from.Y;
                from.Y = 0;
                to.X += m * (ClientRectangle.Height - to.Y);
                to.Y = ClientRectangle.Height;
            }
            else
            {
                from.Y -= from.X / m;
                from.X = 0;
                to.Y += (ClientRectangle.Width - to.X) / m;
                to.X = ClientRectangle.Width;
            }

        }

        #endregion

        #region Event handlers

        #region Mouse events

        protected void OnMouseMove(object sender, MouseEventArgs e)
        {
            ActivateShapeEditor();
            
            snappingOccured = false;

            StopOnDebug();

            if (isScrolling)
            {
                Point pos = new Point(
                    (int)Math.Round(scrollStartPos.X - e.Location.X + areaStartPos.X),
                    (int)Math.Round(scrollStartPos.Y - e.Location.Y + areaStartPos.Y)
                    );

                zoom.Location = pos;
                Refresh();
                //PointF.Subtract(scrollStartPos, areaStartPos);
                //return;
            }

            if (isDragging)
            {
                if (!ClientRectangle.Contains(e.Location) && (!scrollTimer.Enabled))
                    scrollTimer.Enabled = true;
                    //scrollTimer.Start();
                
                snappingOccured = DoSnap(zoom.VisibleToPt(e.Location), snapDistConst);
                snappedCtrlPoint.Location = snappedPoint;
            }
            else
            {
                snappingOccured = false;
                snappedCtrlPoint = null;
                snappedCurve = null;

                StopOnDebug();

                if (snappingOccured = DoSnap(zoom.VisibleToPt(e.Location), snapDistConst))
                    snappedCtrlPoint = shape.SnappedCtrlPoint;
            }

            Refresh();
        }

        private void ActivateShapeEditor()
        {
            if (!this.CanFocus) return;

            if (Focused) return;

            Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float newZoom = zoom.ZoomFactor * (Math.Sign(e.Delta) > 0 ? 2.0f : 0.5f);
            Zoom(e.Location, newZoom);
            
            Refresh();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            snappingOccured = false;

            Refresh();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (snappedCtrlPoint != null)
            {
                if (ShapePoint.DistSquared(zoom.VisibleToPt(e.Location), snappedCtrlPoint.Location) > snapDistConst * snapDistConst)
                    snappedCtrlPoint = null;

                SelectedProperty = snappedCtrlPoint;
            }

            if (snappedCurve != null)
            {
                SelectedProperty = snappedCurve;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (snappedCtrlPoint != null)
                {
                    snappedCtrlPoint.IsModified = true;
                    isDragging = true;
                }
            }

            if ( (e.Button == MouseButtons.Middle) || 
                ((e.Button == MouseButtons.Right) && (Control.ModifierKeys == Keys.Shift)) )
            {
                Cursor.Current = Cursors.Hand;
                isScrolling = true;
                scrollStartPos = e.Location;
                areaStartPos = zoom.Location;
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                if (ShowSnappedPointMenu(e)) return;

                if (ShowSnappedLineMenu(e)) return;

                contextMenuGeneral.Show(PointToScreen(e.Location));

            }

        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            isScrolling = false;
            scrollTimer.Enabled = false;
            propertyGrid.Refresh();

            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                if (snappedCtrlPoint != null) snappedCtrlPoint.IsModified = false;
            }

            if (e.Button == MouseButtons.Right)
            {
                // do nothing for now
            }
        }

        private bool ShowSnappedPointMenu(MouseEventArgs e)
        {
            if (snappedCtrlPoint == null) return false;

            pointMenuItemDelete.Enabled = !snappedCtrlPoint.IsLocked;

            pointMenuItemLocked.Checked = snappedCtrlPoint.IsLocked;
            contextMenuPoint.Show(PointToScreen(e.Location));

            return true;
        }

        private bool ShowSnappedLineMenu(MouseEventArgs e)
        {
            if (snappedCurve == null) return false;

            contextMenuCurve.Show(PointToScreen(e.Location));

            return true;
        }

        #endregion

        #region Keyboard events

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            debugMode = e.Control;
            
            switch (e.KeyCode)
            {
                case Keys.G:
                    GridSnap = !GridSnap;
                    break;

                case Keys.E:
                    ExtensionsSnap = !ExtensionsSnap;
                    break;

                case Keys.C:
                    CtrlPointsSnap = !CtrlPointsSnap;
                    break;

                case Keys.L:
                    CurvesSnap = !CurvesSnap;
                    break;
                default:
                    break;
            };
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            debugMode = e.Control;
        }

        #endregion

        #region Scrolling, etc.

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            scrolls = string.Format("{0}, {1}", HorizontalScroll.Value, VerticalScroll.Value);

            StopOnDebug();

           /* if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                zoom.ScrollHorizontal(e.NewValue);
            else
                zoom.ScrollVertical(e.NewValue);*/

            Refresh();
        }

        #endregion

        #region Custom events

        public virtual void OnSnapChanged(SnapChangedEventArgs e)
        {
            if (this.SnapChanged == null) return;

            this.SnapChanged(this, e);
        }

        public virtual void OnZoomChanged(ZoomChangedEventArgs e)
        {
            if (this.ZoomChanged == null) return;

            this.ZoomChanged(this, e);
        }

        #endregion

        #endregion

        #region Initialization

        private void InitShape()
        {
            float[] pts = new float[] { 64f, 64f, 192f, 64f, 576f, 320f, 64f, 320f };
            ShapePoint[] sp = new ShapePoint[4];

            shape = new ShapeLinesCollection();

            for (int i = 0; i < 4; i++)
                sp[i] = new ShapePoint(pts[i * 2], pts[i * 2 + 1]);

            shape.Add(new ShapeLine(sp[0], sp[1]));
            shape.Add(new ShapeLine(sp[1], sp[2]));
            shape.Add(new ShapeLine(sp[2], sp[3]));
            shape.Add(new ShapeLine(sp[3], sp[0]));

            dimension = new Rectangle(64, 64, 512, 256);
        }

        private void InitZoomAndAutoScroll()
        {
            zoom = new ShapeEditorZoom(16f, 16f);

            zoom.WorkingArea = new RectangleF(0, 0, 600, 340);
            zoom.VisibleArea = ClientRectangle;
        }

        private void InitSnapToGrid()
        {
            snapToGrid = new SnapToGrid();
            snapToGrid.FieldWidth = 32;
            snapToGrid.SnapType = SnapToGrid.SnapTypes.Relative;
            snapToGrid.SnapRelative = 0.2f;
        }

        private void InitPensAndBrushes()
        {
            // Grid Pen
            gridDrawPen = new Pen(Color.FromArgb(232, 232, 232));
            gridDrawPen.DashStyle = DashStyle.Dash;

            // Snapped Control Point
            PenSnappedPoint = new Pen(Color.Red);
            PenSnappedCtrlPoint = new Pen(Color.Green);
            PenSnappedCtrlPointLocked = new Pen(Color.Gray);
            PenSnappedPointLine = new Pen(Color.Gray);

            // snapped line color
            PenSnappedLine = new Pen(Color.BlueViolet);

            // Shape Pens
            PenShape = new Pen(Color.Black);
            BrushControlPoints = new SolidBrush(Color.Black);
            // Control Lines
            PenControlLines = new Pen(Color.DarkRed);
            BrushControlLinesPts = new SolidBrush(Color.Red);

            // Lines' Extensions
            PenLinesExtensions = new Pen(Color.Cyan, 1f);
            PenLinesExtensions.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            DimensionRectFill = new SolidBrush(Color.FromArgb(192, 192, 192));
            DimensionHelperRects = new SolidBrush(Color.Black);
            DimensionRectLines = new Pen(Color.Black, 1f);
            DimensionRectLines.DashStyle = DashStyle.Solid;

        }

        private void InitScrollTimer()
        {
            scrollTimer = new Timer();
            scrollTimer.Interval = 20;
            scrollTimer.Tick += new EventHandler(OnScrollTimer);
        }

        #endregion

        #region Flags operations

        private void SetSnapping(SnapTypes flags)
        {
            snapStates |= (uint)flags;
        }

        private void ClrSnapping(SnapTypes flags)
        {
            snapStates &= ~(uint)flags;
        }

        private bool IsSnappingActive(SnapTypes flags)
        {
            return (snapStates & (uint)flags) != 0;
        }

        private void ToggleSnapping(SnapTypes flags)
        {
            snapStates ^= (uint)flags;
        }

        #endregion

        #region Menus' Actions

        #region Act on Points

        private void Point_Lock(object sender, EventArgs e)
        {
            if (snappedCtrlPoint == null) return;

            snappedCtrlPoint.IsLocked = !snappedCtrlPoint.IsLocked;
        }

        private void Point_Insert(object sender, EventArgs e)
        {
            shape.InsertPoint(snappedCurve, snappedPoint);
        }

        private void Point_Delete(object sender, EventArgs e)
        {
            shape.DeletePoint(snappedCtrlPoint);
            snappedCtrlPoint = null;
        }
        
        #endregion

        #region Act on Curves/Lines

        private void Curve_Convert(object sender, EventArgs e)
        {
            if (snappedCurve == null) return;

            shape.ConvertCurve(snappedCurve);
        }

        #endregion

        #region Zoom and Fit-to-

        private void fitToScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FitShapeToScreen();
        }

        private void fitBoundsToScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FitBoundsToScreen();
        }

        private void actualPixelSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomCenter(1.0f);
        }

        private void extendBoundsToFitShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtendBoundsToFitShape();
        }

        #endregion

        private void OnResize(object sender, EventArgs e)
        {
            zoom.VisibleAreaSize = ClientRectangle.Size;
        }

        #endregion

        #region Exposed Methods

        public void UpdateShape()
        {
            shape.UpdateShape();

            Refresh();
        }

        public void ExtendBoundsToFitShape()
        {
            RectangleF rect = shape.GetBoundingRect();
            dimension = new Rectangle(
                (int)Math.Round(rect.X),
                (int)Math.Round(rect.Y),
                (int)Math.Round(rect.Width),
                (int)Math.Round(rect.Height)
                );

            Refresh();
        }

        public void FitBoundsToScreen()
        {
            FitToScreen(dimension);
        }

        public void FitShapeToScreen()
        {
            FitToScreen(shape.GetBoundingRect());
        }

        #endregion

        private void OnScrollTimer(Object source, EventArgs e)
        {
            Point mouse = PointToClient(MousePosition);
            Point pt = zoom.Location;

            int step = scrollTimer.Interval / 4;

            if (mouse.X < ClientRectangle.Left) pt.X -= step * (1 + (ClientRectangle.Left - mouse.X) / 10);
            if (mouse.Y < ClientRectangle.Top) pt.Y -= step * (1 + (ClientRectangle.Top - mouse.Y) / 10);
            if (mouse.X > ClientRectangle.Right) pt.X += step * (1 + (mouse.X - ClientRectangle.Right) / 10);
            if (mouse.Y > ClientRectangle.Bottom) pt.Y += step * (1 + (mouse.Y - ClientRectangle.Bottom) / 10);

            zoom.Location = pt;

            DoSnap(zoom.VisibleToPt(mouse), snapDistConst);
            if (snappedCtrlPoint != null)
                snappedCtrlPoint.Location = snappedPoint;
            Refresh();
        }

        private bool DoSnap(PointF location, float snapDist)
        {
            bool res = false;
            PointF curPos = new PointF();
            int snapType = 0;

            snapDist /= zoom.ZoomFactor;

            snappedCurve = null;

            // Snap to control points if possible
            if (IsSnappingActive(SnapTypes.SnapToCtrl) && shape.SnapToCtrlPoints(location, snapDist))
            {
                snappedPoint = shape.SnappedCtrlPoint.Location;
                return true;
            }

            // try snap to grid
            if (IsSnappingActive(SnapTypes.SnapToGrid))
                snapType = snapToGrid.SnapPtToGrid(location);

            // Try snap to grid
            if (snapType > 0)
            {
                curPos = snapToGrid.SnappedPoint;
                res = true;
            }
            else
            {
                curPos = location;
                res = false;
            }


            // try snapping intersection between grid and shape curves/lines
            if ((snapType != 0) && shape.SnapToGrid(location, curPos, snapType, snapDist))
            {
                snappedCurve = shape.SnappedCurve;
                snappedPoint = shape.SnappedPoint;
                return true;
            }

            // try snapping to lines / curves' segments
            if (IsSnappingActive(SnapTypes.SnapToCurves) && shape.SnapToSegments(curPos, snapDist))
            {
                snappedCurve = shape.SnappedCurve;
                snappedPoint = shape.SnappedPoint;

                return true;
            }

            // try snapping to line extensions (end point tangents for curves)
            if (IsSnappingActive(SnapTypes.SnapToExtensions) && shape.SnapToExtensions(curPos, snapDist))
            {
                snappedPoint = shape.SnappedPoint;
                return true;
            }

            // no successfull snapping to lines/curves occured, use grid point snapping result
            snappedPoint = curPos;

            return res;
        }

        private void FitToScreen(RectangleF rect)
        {
            zoom.FitToScreen(rect);

            snapToGrid.SnapRelative = 0.2f / zoom.ZoomFactor;

            OnZoomChanged(new ZoomChangedEventArgs(zoom.ZoomFactor));

            Refresh();
        }

        private void ShapeEditorControl_Load(object sender, EventArgs e)
        {
            Reset();
            FitToScreen(dimension);
        }

    }

    #region Custom EventArgs classes

    public class SnapChangedEventArgs : EventArgs
    {
        public readonly RadShapeEditorControl.SnapTypes param;
        public SnapChangedEventArgs(RadShapeEditorControl.SnapTypes data)
        {
            this.param = data;
        }
    }

    public class SelectedChangedEventArgs : EventArgs
    {
        public readonly object snappedObject;

        public SelectedChangedEventArgs(object obj)
        {
            snappedObject = obj;
        }
    }

    public class ZoomChangedEventArgs : EventArgs
    {
        public readonly float zoomCoef;
        public ZoomChangedEventArgs(float data)
        {
            this.zoomCoef = data;
        }
    }

    #endregion

    #region Delegates for custom events

    public delegate void SnapChangedEventHandler(object sender, SnapChangedEventArgs args);

    public delegate void SelectedChangedEventHandler(object sender, SelectedChangedEventArgs args);

    public delegate void ZoomChangedEventHandler(object sender, ZoomChangedEventArgs args);

    #endregion

}