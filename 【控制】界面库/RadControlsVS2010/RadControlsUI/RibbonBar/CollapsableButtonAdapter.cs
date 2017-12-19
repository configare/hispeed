using System.Drawing.Design;
using Telerik.WinControls.Design;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{

    public class CollapsableButtonAdapter : CollapsibleElement
    {
        public RadButtonElement button;

        public CollapsableButtonAdapter(RadButtonElement button)
        {
            this.button = button;
        }

        #region ICollapsibleElement Members

        public override bool ExpandElementToStep(int nextStep)
        {
            bool result = false;

            if (nextStep == (int)ChunkVisibilityState.SmallImages)
            {
                result = ChangeVisibility(false, true);
            }
            else if (nextStep == (int)ChunkVisibilityState.NoText)
            {
                result = ChangeVisibility(true, true);
            }
            else if (nextStep == (int)ChunkVisibilityState.Expanded)
            {
                result = ChangeVisibility(true, false);
                result |= ChangeVisibility(true, true);
            }

            this.CollapseStep = nextStep;
            return result;
        }

        public override bool CollapseElementToStep(int nextStep)
        {
            bool result = false;

            if (nextStep == (int)ChunkVisibilityState.SmallImages)
            {
                result = ChangeVisibility(false, true);
            }
            else if (nextStep == (int)ChunkVisibilityState.NoText)
            {
                result = ChangeVisibility(false, false);
            }

            this.CollapseStep = nextStep;
            return result;
        }

        #endregion

        #region Collapsable helpers
        private bool ChangeVisibility(bool display, bool changeImages)
        {
            bool visibilityChanged = false;

            visibilityChanged = ChangeItemVisibility(display, changeImages);

            return visibilityChanged;
        }

        private bool ChangeItemVisibility(bool display, bool changeImages)
        {
            bool visibilityChanged = false;
            if (button == null)
                return visibilityChanged;

            if (!changeImages && (button.DisplayStyle != DisplayStyle.ImageAndText))
                return visibilityChanged;

            if (button.Children.Count < 2)
                return visibilityChanged;

            ImageAndTextLayoutPanel imageAndTextPanel = button.Children[1] as ImageAndTextLayoutPanel;

            if (imageAndTextPanel == null)
                return visibilityChanged;

            if (imageAndTextPanel.Children.Count < 2)
                return visibilityChanged;

            if (!changeImages && ChangeTextVisibility(imageAndTextPanel, display))
                visibilityChanged = true;

            if (changeImages && ChangeImages(display))
                visibilityChanged = true;

            return visibilityChanged;
        }

        private bool ChangeTextVisibility(ImageAndTextLayoutPanel imageAndTextPanel, bool display)
        {
            if (imageAndTextPanel.DisplayStyle != DisplayStyle.ImageAndText)
                return false;

            if ((button.Image == null) && (button.ImageIndex == -1) && (button.ImageKey == string.Empty))
                return false;

            RadElement textElement = imageAndTextPanel.Children[1];

            if (display && (textElement.Visibility == ElementVisibility.Collapsed))
            {
                textElement.Visibility = ElementVisibility.Visible;
                textElement.InvalidateMeasure();
                return true;
            }
            if (!display && (textElement.Visibility == ElementVisibility.Visible))
            {
                textElement.Visibility = ElementVisibility.Collapsed;
                textElement.InvalidateMeasure();
                return true;
            }
            return false;
        }

        private bool HasNoButtonSmallImages()
        {
            return (button.SmallImage == null) &&
                (button.SmallImageIndex == -1) &&
                (button.SmallImageKey == string.Empty);
        }

        private bool ChangeImages(bool display)
        {
            if (this.HasNoButtonSmallImages())
            {
                return false;
            }

            if (!display)
            {
                button.UseSmallImageList = true;
                this.PreserveOldImage(button);

                if (button.SmallImage != null)
                    button.Image = button.SmallImage;

                if (button.SmallImageIndex != -1)
                    button.ImageIndex = button.SmallImageIndex;

                if (button.SmallImageKey != string.Empty)
                    button.ImageKey = button.SmallImageKey;
            }
            else
            {
                button.UseSmallImageList = false;
                RestoreOldImage(button);
            }
            button.InvalidateMeasure();
            button.ImagePrimitive.InvalidateMeasure();
            button.UpdateLayout();
            return true;
        }

        private void PreserveOldImage(RadButtonElement button)
        {
            if (button.Image != null && button.SmallImage != button.Image)
                button.SetValue(RadButtonElement.LargeImageProperty, button.Image);

            if (button.ImageIndex != -1)
                button.SetValue(RadButtonElement.LargeImageIndexProperty, button.ImageIndex);

            if (button.ImageKey != string.Empty)
                button.SetValue(RadButtonElement.LargeImageKeyProperty, button.ImageKey);
        }

        private void RestoreOldImage(RadButtonElement button)
        {
            if (button.LargeImage != null)
            {
                button.Image = button.LargeImage;
                button.ImagePrimitive.InvalidateMeasure();
                button.SetValue(RadButtonElement.LargeImageProperty, null);
            }

            if (button.LargeImageIndex != -1)
            {
                button.ImageIndex = button.LargeImageIndex;
                button.ImagePrimitive.InvalidateMeasure();
                button.SetValue(RadButtonElement.LargeImageIndexProperty, -1);
            }

            if (button.LargeImageKey != string.Empty)
            {
                button.ImageKey = button.LargeImageKey;
                button.ImagePrimitive.InvalidateMeasure();
                button.SetValue(RadButtonElement.LargeImageKeyProperty, string.Empty);
            }
        }
        #endregion


        override public bool CanExpandToStep(int nextStep)
        {
            if (nextStep == this.CollapseStep && nextStep != this.CollapseMaxSteps)
            {
                return false;
            }

            if (nextStep == 1)
            {
                return true;
            }

            if (nextStep == (int)ChunkVisibilityState.SmallImages)
            {
                return !(((button.SmallImage == null) &&
                (button.SmallImageIndex == -1) &&
                (button.SmallImageKey == string.Empty)));
            }

            if (nextStep == (int)ChunkVisibilityState.NoText)
            {
                if (button.Children.Count < 2)
                    return false;

                ImageAndTextLayoutPanel imageAndTextPanel = button.Children[1] as ImageAndTextLayoutPanel;

                if (imageAndTextPanel == null)
                    return false;

                if (imageAndTextPanel.Children.Count < 2)
                    return false;

                if (button.DisplayStyle != DisplayStyle.ImageAndText)
                {
                    return false;
                }

                if ((imageAndTextPanel.TextImageRelation != TextImageRelation.ImageBeforeText) &&
                    (imageAndTextPanel.TextImageRelation != TextImageRelation.TextBeforeImage))
                    return false;
                return true;
            }
            return false;
        }

        override public bool CanCollapseToStep(int nextStep)
        {
            if (nextStep < this.CollapseStep)
            {
                return false;
            }

            if (nextStep == (int)ChunkVisibilityState.Expanded)
            {
                return true;
            }

            if (nextStep == (int)ChunkVisibilityState.SmallImages)
            {
                bool result = !((button.SmallImage == null) &&
                (button.SmallImageIndex == -1) &&
                (button.SmallImageKey == string.Empty));
                
                return result;
            }

            if (nextStep == (int)ChunkVisibilityState.NoText)
            {
                if (button.Children.Count < 2)
                    return false;

                ImageAndTextLayoutPanel imageAndTextPanel = button.Children[1] as ImageAndTextLayoutPanel;

                if (imageAndTextPanel == null)
                    return false;

                if (imageAndTextPanel.Children.Count < 2)
                    return false;

                if (button.DisplayStyle != DisplayStyle.ImageAndText)
                {
                    return false;
                }
                

                return true;
            }
            return false;
        }

        public override int CollapseMaxSteps
        {
            get
            {
                return 3;
            }
        }

        public SizeF CurrentSize
        {
            get
            {
                return this.button.DesiredSize;
            }
        }
    }

}
