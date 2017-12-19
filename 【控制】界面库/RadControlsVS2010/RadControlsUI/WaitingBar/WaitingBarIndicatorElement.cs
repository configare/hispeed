using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
    public class WaitingBarIndicatorElement: LightVisualElement
    {

        #region Initialization

        static WaitingBarIndicatorElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new WaitingBarIndicatorStateManager(), typeof(WaitingBarIndicatorElement));
        }

        public WaitingBarSeparatorElement SeparatorElement
        {
            get { return this.separator; }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawFill = true;
            this.StretchVertically = true;
            this.StretchHorizontally = false;
            this.ZIndex = -1;
            this.BackColor = Color.Green;
            this.ClipDrawing = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            separator = new WaitingBarSeparatorElement();
            separator.Class = "WaitingIndicatorSeparator";
            this.Children.Add(separator);
        }

        #endregion

        #region Fields
        
        WaitingBarSeparatorElement separator;

        #endregion

        #region RadProperties
        
        public static RadProperty IsVerticalProperty = RadProperty.Register("IsVertical", typeof(bool),
         typeof(WaitingBarIndicatorElement), new RadElementPropertyMetadata(
            false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty OffsetProperty = RadProperty.Register("Offset", typeof(int), typeof(WaitingBarIndicatorElement),
            new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Properties
        
        public virtual int Offset
        {
            get 
            { 
                return (int)GetValue(OffsetProperty); 
            }
            set 
            { 
                SetValue(OffsetProperty, value); 
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            RadWaitingBarElement waitingBarElement = FindAncestor<RadWaitingBarElement>();

            SizeF desiredSize = base.MeasureOverride(availableSize);

            if ( waitingBarElement != null)
            {
                if (StretchVertically && !float.IsInfinity(availableSize.Height))
                {
                    desiredSize.Height = availableSize.Height;
                }
                else
                {
                    if (this.Image != null && GetValueSource(RadWaitingBarElement.WaitingIndicatorSizeProperty) != Telerik.WinControls.ValueSource.Local)
                    {
                        desiredSize.Height = Image.Height;
                    }
                    else
                    {
                        desiredSize.Height = waitingBarElement.WaitingIndicatorSize.Height;
                    }
                }

                if (StretchHorizontally && !float.IsInfinity(availableSize.Width))
                {
                    desiredSize.Width = availableSize.Width;
                }
                else
                {
                    if (this.Image != null && GetValueSource(RadWaitingBarElement.WaitingIndicatorSizeProperty) != Telerik.WinControls.ValueSource.Local)
                    {
                        desiredSize.Height = Image.Height;
                    }
                    else
                    {
                        desiredSize.Width = waitingBarElement.WaitingIndicatorSize.Width;
                    }
                }
            }

            return desiredSize;
        }

        #endregion

    }
}
