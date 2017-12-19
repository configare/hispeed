using Telerik.WinControls.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class CarouselBezierPath : CarouselParameterPath
    {
        private ReaderWriterLock pointsLock = new ReaderWriterLock();
        private ReaderWriterLock paramsLock = new ReaderWriterLock();
        private double[][] parameters = new double[3][] { new double[3] { 0, 0, 0 }, new double[3] { 0, 0, 0 }, new double[3] { 0, 0, 0 } };
        private Point3D[] points = new Point3D[4] { Point3D.Empty, Point3D.Empty, Point3D.Empty, Point3D.Empty };
        private string[] propertyNames = new string[] { "FirstPoint", "CtrlPoint1", "CtrlPoint2", "LastPoint" };

        private bool closedPath = false;

        public CarouselBezierPath()
            : base()
        {
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point3D[] ControlPoints
        {
            get { return this.points; }
        }

        [NotifyParentProperty(true)]
        public Point3D FirstPoint
        {
            get { return this.points[0]; }

            set { SetPoint(0, value); }
        }

        private bool ShouldSerializeFirstPoint()
        {
            return this.FirstPoint != Point3D.Empty;
        }

        private void ResetFirstPoint()
        {
            this.FirstPoint = Point3D.Empty;
        }

        private bool ShouldSerializeCtrlPoint1()
        {
            return this.CtrlPoint1 != Point3D.Empty;
        }

        private void ResetCtrlPoint1()
        {
            this.CtrlPoint1 = Point3D.Empty;
        }

        private bool ShouldSerializeCtrlPoint2()
        {
            return this.CtrlPoint2 != Point3D.Empty;
        }

        private void ResetCtrlPoint2()
        {
            this.CtrlPoint2 = Point3D.Empty;
        }

        private bool ShouldSerializeLastPoint()
        {
            return this.LastPoint != Point3D.Empty;
        }

        private void ResetLastPoint()
        {
            this.LastPoint = Point3D.Empty;
        }

        [NotifyParentProperty(true)]
        public Point3D CtrlPoint1
        {
            get { return this.points[1]; }

            set { SetPoint(1, value); }
        }

        [NotifyParentProperty(true)]
        public Point3D CtrlPoint2
        {
            get { return this.points[2]; }

            set { SetPoint(2, value); }
        }

        [NotifyParentProperty(true)]
        public Point3D LastPoint
        {
            get { return this.points[3]; }

            set { SetPoint(3, value); }
        }

        #region Bezier specific methods

        private static double EvaluateCoordinate(double[] tPow, double point, double[] para)
        {
            for (int i = 0; i < 3; i++)
                point += para[i] * tPow[i];

            return point;
        }

        private void EvaluateParameters()
        {
            this.pointsLock.AcquireReaderLock(20);

            if (this.isModified)
            {
                paramsLock.AcquireWriterLock(20);
                this.isModified = false;

                this.parameters[0][2] = 3 * (this.points[1].X - this.points[0].X);
                this.parameters[0][1] = ( 3 * (this.points[2].X - this.points[1].X) ) - this.parameters[0][2];
                this.parameters[0][0] = this.points[3].X - this.points[0].X - this.parameters[0][2] - this.parameters[0][1];

                this.parameters[1][2] = 3 * (this.points[1].Y - this.points[0].Y);
                this.parameters[1][1] = ( 3 * (this.points[2].Y - this.points[1].Y) ) - this.parameters[1][2];
                this.parameters[1][0] = this.points[3].Y - this.points[0].Y - this.parameters[1][2] - this.parameters[1][1];

                this.parameters[2][2] = 3 * (this.points[1].Z - this.points[0].Z);
                this.parameters[2][1] = ( 3 * (this.points[2].Z - this.points[1].Z) ) - this.parameters[2][2];
                this.parameters[2][0] = this.points[3].Z - this.points[0].Z - this.parameters[2][2] - this.parameters[2][1];

                paramsLock.ReleaseWriterLock();
            }

            this.pointsLock.ReleaseReaderLock();
        }

        private void SetPoint(int pt, Point3D value)
        {
            bool notify = false;
            this.pointsLock.AcquireWriterLock(10);

            if (this.points[pt] != value)
            {
                this.points[pt] = value;
                this.isModified = true;
                notify = true;
            }

            this.pointsLock.ReleaseWriterLock();

            if (notify)
            {
                this.OnPropertyChanged(propertyNames[pt]);
            }
        }

        #endregion

        #region IPathCalculator Members

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override object InitialPathPoint
        {
            get { return this.points[0]; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override object FinalPathPoint
        {
            get { return this.points[3]; }
        }

        
        public override object EvaluateByParameter(VisualElement element, CarouselPathAnimationData data, double value)
        {
            double[] tPow = new double[3] { 0, 0, value };

            tPow[1] = tPow[2] * tPow[2];
            tPow[0] = tPow[1] * tPow[2];

            EvaluateParameters();

            paramsLock.AcquireReaderLock(20);

            Point3D result = new Point3D(
                EvaluateCoordinate(tPow, this.points[0].X, this.parameters[0]),
                EvaluateCoordinate(tPow, this.points[0].Y, this.parameters[1]),
                EvaluateCoordinate(tPow, this.points[0].Z, this.parameters[2])
                );

            paramsLock.ReleaseReaderLock();

            return result;
        }

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FirstPoint":
                case "CtrlPoint1":
                case "CtrlPoint2":
                case "LastPoint":
                case "RelativePath":
                case "ZScale":
                case "OpacityChangeCondition":
                    UpdateZindexSource();
                    break;
            }

            base.OnPropertyChanged(e);
        }

        private void UpdateZindexSource()
        {
            double min = this.points[0].Z;
            double max = min;

            bool fromHere = false;

            for ( int i = 1; i < 3; i++ )
            {
                fromHere |= this.points[0].Z != this.points[i].Z;
                min = Math.Min(min, this.points[i].Z);
                max = Math.Max(max, this.points[i].Z);
            }

            this.closedPath = this.FirstPoint.Equals(this.LastPoint);

            base.zIndexScale = 10000000 / (Math.Abs(min) + Math.Abs(max));
            base.zIndexFromPath = fromHere;
        }

        protected override bool IsClosedPath
        {
            get
            {
                return this.closedPath;
            }
        }
    }
}