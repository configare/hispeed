using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Calculates animation rectangle values.
    /// </summary>
    public class AnimationValueRectangleCalculator : AnimationValueCalculator
    {
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
        {
			Rectangle currRect = Rectangle.Empty;
			if (currValue != null)
			{
				currRect = (Rectangle)currValue;
			}
			if (endValue != null)
			{
				int startLeft = currRect.Left;
				int startTop = currRect.Top;
				int startWidth = currRect.Width;
				int startHeight = currRect.Height;
				int endLeft;
				int endTop;
				int endWidth;
				int endHeight;

				if (startValue.GetType() == typeof(Rectangle))
				{
					Rectangle startRectangle = (Rectangle)startValue;
					startLeft = startRectangle.Left;
					startTop = startRectangle.Top;
					startWidth = startRectangle.Width;
					startHeight = startRectangle.Height;
				}
				else if (startValue.GetType() == typeof(Point))
				{
					Point startPoint = (Point)startValue;
					startLeft = startPoint.X;
					startTop = startPoint.Y;
				}
				else if (startValue.GetType() == typeof(Size))
				{
				}

				endLeft = startLeft;
				endTop = startTop;
				endWidth = startWidth;
				endHeight = startHeight;

				if (endValue.GetType() == typeof(Rectangle))
				{
					Rectangle endRectangle = (Rectangle)endValue;
					endLeft = endRectangle.Left;
					endTop = endRectangle.Top;
					endWidth = endRectangle.Width;
					endHeight = endRectangle.Height;
				}
				else if (endValue.GetType() == typeof(Point))
				{
					Point endPoint = (Point)endValue;
					endLeft = endPoint.X;
					endTop = endPoint.Y;
				}
				else if (endValue.GetType() == typeof(Size))
				{
					Size endSize = (Size)endValue;
					endWidth = endSize.Width;
					endHeight = endSize.Height;
				}

				int newLeft, newTop, newWidth, newHeight;
 
				newLeft = calc.CalculateCurrentValue(startLeft, endLeft, currFrameNum, totalFrameNum);
				newTop = calc.CalculateCurrentValue(startTop, endTop, currFrameNum, totalFrameNum);
				newWidth = calc.CalculateCurrentValue(startWidth, endWidth, currFrameNum, totalFrameNum);
				newHeight = calc.CalculateCurrentValue(startHeight, endHeight, currFrameNum, totalFrameNum);

				return new Rectangle(newLeft, newTop, newWidth, newHeight);
			}

			if (step == null)
				return currValue;

			if (step.GetType() == typeof(Rectangle))
			{
				Rectangle rect = (Rectangle)step;

				return new Rectangle(
				currRect.X + rect.X,
				currRect.Y + rect.Y,
				currRect.Width + rect.Width,
				currRect.Height + rect.Height);
			}
			else if (step.GetType() == typeof(Point))
			{
				Point pt = (Point)step;				

				return new Rectangle(
				currRect.X + pt.X,
				currRect.Y + pt.Y,
				currRect.Width,
				currRect.Height);
			}
			else if (step.GetType() == typeof(Size))
			{
				Size size = (Size)step;				

				return new Rectangle(
					currRect.X,
					currRect.Y,
					currRect.Width + size.Width,
					currRect.Height + size.Height);
			}

			return currValue;
        }
      
        public override object CalculateInversedStep(object step)
        {
            Rectangle rect = (Rectangle)step;
            
            return new Rectangle(
                -rect.X,
                -rect.Y,
                -rect.Width,
                -rect.Height);
        }

        public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
        {
			Rectangle rectStart = Rectangle.Empty;
			Rectangle rectEnd = Rectangle.Empty;

			if (animationEndValue.GetType() == typeof(Rectangle))
			{
				rectEnd = (Rectangle)animationEndValue;
			}
			else if (animationEndValue.GetType() == typeof(Point))
			{
				rectEnd = new Rectangle((Point)animationEndValue, Size.Empty);
			}
			else if (animationEndValue.GetType() == typeof(Size))
			{
				rectEnd = new Rectangle(Point.Empty, (Size)animationEndValue);
			}

			if (animationStartValue.GetType() == typeof(Rectangle))
			{
				rectStart = (Rectangle)animationStartValue;
			}
			else if (animationStartValue.GetType() == typeof(Point))
			{
				rectStart = new Rectangle((Point)animationStartValue, Size.Empty);				
			}
			else if (animationStartValue.GetType() == typeof(Size))
			{
				rectStart = new Rectangle(Point.Empty, (Size)animationStartValue);				
			}

            return new Rectangle(
				base.CalculateIntStep(rectStart.X, rectEnd.X, numFrames),
				base.CalculateIntStep(rectStart.Y, rectEnd.Y, numFrames),
				base.CalculateIntStep(rectStart.Width, rectEnd.Width, numFrames),
				base.CalculateIntStep(rectStart.Height, rectEnd.Height, numFrames));
        }
    }
    /// <summary>
    /// Represents a value size animation calculator.
    /// </summary>
    public class AnimationValueSizeCalculator : AnimationValueCalculator
	{
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
		{
			Size size = (Size)step;
			Size currSize = (Size)currValue;

			return new Size(
				currSize.Width + size.Width,
				currSize.Height + size.Height);
		}

		public override object CalculateInversedStep(object step)
		{
			Size size = (Size)step;

			return new Size(
				-size.Width,
				-size.Height);
		}
   
		public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
		{
			Size sizeStart = (Size)animationStartValue;
			Size sizeEnd = (Size)animationEndValue;

			return new Size(
				base.CalculateIntStep(sizeStart.Width, sizeEnd.Width, numFrames),
				base.CalculateIntStep(sizeStart.Height, sizeEnd.Height, numFrames));
		}
	}
    /// <summary>
    /// Represents a value point animation calculator.
    /// </summary>
	public class AnimationValuePointCalculator : AnimationValueCalculator
	{
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
		{
			Point size = (Point)step;
			Point currSize = (Point)currValue;

			return new Point(
				currSize.X + size.X,
				currSize.Y + size.Y);
		}
    
		public override object CalculateInversedStep(object step)
		{
			Point size = (Point)step;

			return new Size(
				-size.X,
				-size.Y);
		}
 
		public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
		{
			Point sizeStart = (Point)animationStartValue;
			Point sizeEnd = (Point)animationEndValue;

			return new Point(
				base.CalculateIntStep(sizeStart.X, sizeEnd.X, numFrames),
				base.CalculateIntStep(sizeStart.Y, sizeEnd.Y, numFrames));
		}
	}
    /// Represents a value point animation calculator using floating point values.
	public class AnimationValuePointFCalculator : AnimationValueCalculator
	{
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
		{
			PointF size = (PointF)step;
			PointF currSize = (PointF)currValue;

			return new PointF(
				currSize.X + size.X,
				currSize.Y + size.Y);
		}

		public override object CalculateInversedStep(object step)
		{
			PointF size = (PointF)step;

			return new PointF(
				-size.X,
				-size.Y);
		}
  
		public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
		{
			PointF sizeStart = (PointF)animationStartValue;
			PointF sizeEnd = (PointF)animationEndValue;

			return new PointF(
				base.CalculateFloatStep(sizeStart.X, sizeEnd.X, numFrames),
				base.CalculateFloatStep(sizeStart.Y, sizeEnd.Y, numFrames));
		}
	}
    /// <summary>
    ///Represents a value size animation calculator using floating point values.
    /// </summary>
    public class AnimationValueSizeFCalculator : AnimationValueCalculator
	{
        public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
		{
            SizeF size = (SizeF)startValue;			

            float resWidth = calc.CalculateCurrentValue(size.Width, ((SizeF)endValue).Width, currFrameNum, totalFrameNum);
            float resHeight = calc.CalculateCurrentValue(size.Height, ((SizeF)endValue).Height, currFrameNum, totalFrameNum);

			return new SizeF(
				resWidth,
                resHeight);
		}
       
		public override object CalculateInversedStep(object step)
		{
			SizeF size = (SizeF)step;

			return new SizeF(
				-size.Width,
				-size.Height);
		}

		public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
		{
			SizeF sizeStart = (SizeF)animationStartValue;
			SizeF sizeEnd = (SizeF)animationEndValue;

			return new SizeF(
				base.CalculateFloatStep(sizeStart.Width, sizeEnd.Width, numFrames),
				base.CalculateFloatStep(sizeStart.Height, sizeEnd.Height, numFrames));
		}
	}
}
