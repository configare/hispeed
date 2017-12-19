using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents a custom made ellipse path which will be used to specify the path travelled by carousel items when animated
	/// </summary>
    public class CarouselEllipsePath : CarouselParameterPath
    {
        //private bool upToDate = false;
        private Point3D center = new Point3D();
        private Point3D u = new Point3D();
        private Point3D v = new Point3D();

        private double initialAngle = 0;
        private double finalAngle = 360;

	    [Description("Gets or sets the center of the ellipse of the path")]
        [NotifyParentProperty(true)]
        public Point3D Center
        {
            get { return this.center; }
            set 
            { 
                if (this.center != value)
                {
                    this.center = value;
                    this.OnPropertyChanged("Center");
                }

            }
        }

		[Description("Gets or sets the first focus of the ellipse of the path")]
        [NotifyParentProperty(true)]
        public Point3D U
        {
            get { return this.u; }

            set
            {
                if (this.u != value)
                {
                    this.u = value;
                    this.OnPropertyChanged("U");
                }
            }
        }

		[Description("Gets or sets the second focus of the ellipse of the path")]
        [NotifyParentProperty(true)]
        public Point3D V
        {
            get { return this.v; }

            set
            {
                if (this.v != value)
                {
                    this.v = value;
                    this.OnPropertyChanged("V");
                }
            }
        }

        /// <summary>
        /// Gets or sets the angle where itms new items will first appear in the carousel view.
        /// </summary>
		[Description("Gets or sets the angle where itms new items will first appear in the carousel view.")]
        [NotifyParentProperty(true)]
        public double InitialAngle
        {
            get { return this.initialAngle; }

            set
            {
                if (this.initialAngle != value)
                {
                    this.initialAngle = value;
                    this.OnPropertyChanged("InitialAngle");
                }
            }
        }

		[Description("Gets or sets the final angle of the ellipse of the path")]
        [NotifyParentProperty(true)]
        public double FinalAngle
        {
            get { return this.finalAngle; }

            set
            {
                if (this.finalAngle != value)
                {
                    this.finalAngle = value;
                    this.OnPropertyChanged("FinalAngle");
                }
            }
        }

        //public double CurrentItemAngle
        //{
        //    get { return this.currentItemAngle; }

        //    set
        //    {
        //        if (this.currentItemAngle != value)
        //        {
        //            this.currentItemAngle = value;
        //            this.OnPropertyChanged("CurrentItemAngle");
        //        }
        //    }
        //}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets initial path point in the path")]        
        public override object InitialPathPoint
        {
            get { return Evaluate3D(Center, U, V, ToRadians(this.initialAngle)); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets final path point in the path")]
        public override object FinalPathPoint
        {
            get { return Evaluate3D(Center, U, V, ToRadians(this.finalAngle)); }
        }

	    private static Point3D Evaluate3D(Point3D C, Point3D u, Point3D v, double angle)
        {
            Point3D U = new Point3D(u), V = new Point3D(v);
            double a = U.Length();
            double b = V.Length();

            U.Normalize();
            V.Normalize();

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            return new Point3D(
                C.X + (a * cos * U.X ) + ( b * sin * V.X ),
                C.Y + (a * cos * U.Y ) + ( b * sin * V.Y ),
                C.Z + (a * cos * U.Z ) + ( b * sin * V.Z )
                );
        }

	    public override object EvaluateByParameter(VisualElement element, CarouselPathAnimationData data, double value)
        {
            double angle = ( 2 * value * Math.PI ) + ToRadians(this.initialAngle);

            return Evaluate3D(this.center, u, v, angle);
        }

        private static double ToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Center":
                case "U":
                case "V":
                case "RelativePath":
                case "OpacityChangeCondition":

                    UpdateZindexSource();
                    break;
            }

            base.OnPropertyChanged(e);
        }

        private void UpdateZindexSource()
        {
            double m = 
                Math.Abs(Math.Max(Math.Max(Center.Z, U.Z), V.Z)) +
                Math.Abs(Math.Min(Math.Min(Center.Z, U.Z), V.Z));

            base.zIndexScale = 100000000 / m;
            base.zIndexFromPath = Center.Z != U.Z || Center.Z != V.Z;
        }
    }
}
