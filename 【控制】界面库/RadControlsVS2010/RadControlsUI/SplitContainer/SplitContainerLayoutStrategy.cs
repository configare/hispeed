using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the layout strategy for a RadSplitContainer.
    /// </summary>
    public class SplitContainerLayoutStrategy
    {
        #region Constructor

        public SplitContainerLayoutStrategy()
        {
            this.layoutInfo = new SplitContainerLayoutInfo();
        }

        #endregion

        #region Overridables

        /// <summary>
        /// Entry point for the entire layout operation.
        /// Called in the OnLayout override of RadSplitContainer.
        /// </summary>
        /// <param name="container"></param>
        public virtual void PerformLayout(RadSplitContainer container)
        {
            //check whether we have a direct child or a child which contains a panel with SizeMode.Fill
            this.layoutInfo.Reset();

            this.FindFillPanels(container);
            this.UpdateLayoutInfo(container);

            if (this.layoutInfo.layoutTargets.Count == 0)
            {
                return;
            }

            if (this.layoutInfo.layoutTargets.Count == 1)
            {
                this.layoutInfo.layoutTargets[0].Bounds = this.layoutInfo.contentRect;
            }
            else
            {
                this.Measure();
                this.Layout(container);
            }
        }

        /// <summary>
        /// Applies a correction in both of the specified panels, after a successful spliter drag operation.
        /// </summary>
        /// <param name="left">The panel left (top) on the splitter.</param>
        /// <param name="right">The panel right (bottom) on the splitter.</param>
        /// <param name="dragLength">The dragged distance.</param>
        public virtual void ApplySplitterCorrection(SplitPanel left, SplitPanel right, int dragLength)
        {
            this.ApplySplitterCorrection(left, dragLength, true);
            this.ApplySplitterCorrection(right, dragLength, false);

            //update all panels' auto-size scale factor
            foreach (SplitPanel panel in this.layoutInfo.layoutTargets)
            {
                //left, right anf fill panels do not need update
                if (panel == left || panel == right || panel == layoutInfo.fillPanel)
                {
                    continue;
                }

                //only auto-sizable panels should be updated
                SplitPanelSizeInfo info = panel.SizeInfo;
                if (info.SizeMode != SplitPanelSizeMode.Auto)
                {
                    continue;
                }

                if (this.layoutInfo.fillPanel != null)
                {
                    Size absolute = panel.SizeInfo.AbsoluteSize;
                    switch (this.layoutInfo.orientation)
                    {
                        case Orientation.Horizontal:
                            absolute.Height = panel.Height;
                            break;
                        case Orientation.Vertical:
                            absolute.Width = panel.Width;
                            break;
                    }
                    info.AbsoluteSize = absolute;
                }
                else
                {
                    SizeF scale = info.AutoSizeScale;
                    float ratio;
                    switch (this.layoutInfo.Orientation)
                    {
                        case Orientation.Vertical:
                            ratio = (float)panel.Width / this.layoutInfo.autoSizeLength;
                            scale.Width = ratio - this.layoutInfo.autoSizeCountFactor;
                            break;
                        case Orientation.Horizontal:
                            ratio = (float)panel.Height / this.layoutInfo.autoSizeLength;
                            scale.Height = ratio - this.layoutInfo.autoSizeCountFactor;
                            break;
                    }
                    info.AutoSizeScale = scale;
                }
            }
        }

        /// <summary>
        /// Updates the layout info for a pending layout operation.
        /// </summary>
        /// <param name="container"></param>
        protected virtual void UpdateLayoutInfo(RadSplitContainer container)
        {
            this.layoutInfo.contentRect = container.ContentRectangle;
            this.layoutInfo.orientation = container.Orientation;
            this.layoutInfo.availableLength = this.GetLength(this.layoutInfo.contentRect.Size);
            this.layoutInfo.autoSizeLength = this.layoutInfo.availableLength;
            this.layoutInfo.splitterLength = container.SplitterWidth;

            //collect layout targets
            int count = container.SplitPanels.Count;
            for (int i = 0; i < count; i++)
            {
                SplitPanel panel = container.SplitPanels[i];
                if (panel.Collapsed)
                {
                    continue;
                }

                this.layoutInfo.layoutTargets.Add(panel);

                SplitPanelSizeInfo sizeInfo = panel.SizeInfo;
                sizeInfo.minLength = this.GetLength(this.GetMinimumSize(panel));
                this.layoutInfo.totalMinLength += sizeInfo.minLength;
                

                switch (sizeInfo.SizeMode)
                {
                    case SplitPanelSizeMode.Absolute:
                        int length = this.GetLength(sizeInfo.AbsoluteSize);
                        if (length > 0)
                        {
                            this.layoutInfo.autoSizeLength -= length;
                            this.layoutInfo.absoluteSizeTargets.Add(panel);
                        }
                        else
                        {
                            this.layoutInfo.autoSizeTargets.Add(panel);
                        }
                        break;
                    case SplitPanelSizeMode.Auto:
                    case SplitPanelSizeMode.Relative:
                        this.layoutInfo.autoSizeTargets.Add(panel);
                        break;
                }
            }

            if (this.layoutInfo.layoutTargets.Count > 0)
            {
                this.layoutInfo.totalSplitterLength = (this.layoutInfo.layoutTargets.Count - 1) * this.layoutInfo.splitterLength;
            }

            if (this.layoutInfo.autoSizeTargets.Count > 0)
            {
                this.layoutInfo.autoSizeCountFactor = 1F / this.layoutInfo.autoSizeTargets.Count;
                this.layoutInfo.autoSizeLength -= (this.layoutInfo.autoSizeTargets.Count - 1) * this.layoutInfo.splitterLength;
            }
        }

        /// <summary>
        /// Performs the core measure logic.
        /// This is the pass which determines the desired size for each panel.
        /// </summary>
        protected virtual void Measure()
        {
            if (this.layoutInfo.fillPanel != null)
            {
                this.MeasureWithFillPanel();
            }
            else
            {
                this.MeasureDefault();
            }

            this.ClampMeasuredLength();
        }

        /// <summary>
        /// Performs the core layout logic. Updates each panel's bounds, keeping in mind restrictions like Minimum and Maximum size.
        /// </summary>
        /// <param name="container"></param>
        protected virtual void Layout(RadSplitContainer container)
        {
            SplitPanel panel;
            Rectangle panelBounds;
            Rectangle splitterBounds;

            int length = 0;
            int offset = this.GetLayoutOffset();
            int remaining = this.layoutInfo.availableLength;

            int count = this.layoutInfo.LayoutTargets.Count;
            for (int i = 0; i < count; i++)
            {
                panel = this.layoutInfo.LayoutTargets[i];
                SplitPanelSizeInfo sizeInfo = panel.SizeInfo;
                length = sizeInfo.measuredLength;
                if (i == count - 1)
                {
                    length = remaining;
                }

                switch (this.layoutInfo.Orientation)
                {
                    case Orientation.Vertical:
                        panelBounds = new Rectangle(offset, this.layoutInfo.contentRect.Top, length, this.layoutInfo.contentRect.Height);
                        splitterBounds = new Rectangle(panelBounds.Left - this.layoutInfo.splitterLength, panelBounds.Top, this.layoutInfo.splitterLength, panelBounds.Height);
                        break;
                    default:
                        panelBounds = new Rectangle(this.layoutInfo.contentRect.Left, offset, this.layoutInfo.contentRect.Width, length);
                        splitterBounds = new Rectangle(panelBounds.Left, panelBounds.Top - this.layoutInfo.splitterLength, panelBounds.Width, this.layoutInfo.splitterLength);
                        break;
                }

                panel.Bounds = panelBounds;
                offset += (length + this.layoutInfo.splitterLength);
                remaining -= (length + this.layoutInfo.splitterLength);

                if (i > 0)
                {
                    container.UpdateSplitter(this.layoutInfo, i, splitterBounds);
                }
            }
        }

        /// <summary>
        /// Gets an integer value for the specified size (depending on the orientation of the current laid-out container).
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        protected virtual int GetLength(Size size)
        {
            if (this.layoutInfo.orientation == Orientation.Horizontal)
            {
                return size.Height;
            }

            return size.Width;
        }

        /// <summary>
        /// Gets a single-precision value from the provides SizeF struct.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        protected virtual float GetLengthF(SizeF size)
        {
            if (this.layoutInfo.orientation == Orientation.Horizontal)
            {
                return size.Height;
            }

            return size.Width;
        }

        /// <summary>
        /// Gets the available length left for the panel at the specified index.
        /// </summary>
        /// <param name="panels"></param>
        /// <param name="index"></param>
        /// <param name="remaining"></param>
        /// <returns></returns>
        protected virtual int GetAvailableLength(List<SplitPanel> panels, int index, int remaining)
        {
            int count = panels.Count;
            int minLength = 0;
            SplitPanel panel;

            for (int i = index + 1; i < count; i++)
            {
                panel = panels[i];
                minLength += panel.SizeInfo.minLength;
            }

            return remaining - minLength;
        }

        /// <summary>
        /// Gets the minimum size for the specified split panel.
        /// If it is a container, the sum of minimum sizes of all child panels is calculated.
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        protected internal virtual Size GetMinimumSize(SplitPanel panel)
        {
            Size defaultMin = panel.SizeInfo.MinimumSize;
            RadSplitContainer container = panel as RadSplitContainer;
            if (container == null)
            {
                return defaultMin;
            }

            int count = container.SplitPanels.Count;
            if (count <= 1)
            {
                return defaultMin;
            }

            int minWidth = 0;
            int minHeight = 0;

            for (int i = 0; i < count; i++)
            {
                SplitPanel childPanel = container.SplitPanels[i];
                if (childPanel.Collapsed)
                {
                    continue;
                }

                Size minSize = this.GetMinimumSize(childPanel);

                switch(container.Orientation)
                {
                    case Orientation.Horizontal:
                        minWidth = Math.Max(minWidth, minSize.Width);
                        minHeight += minSize.Height;
                        if (i < count - 1)
                        {
                            minHeight += container.SplitterWidth;
                        }
                        break;
                    case Orientation.Vertical:
                        minWidth += minSize.Width;
                        if (i < count - 1)
                        {
                            minWidth += container.SplitterWidth;
                        }
                        minHeight = Math.Max(minHeight, minSize.Height);
                        break;
                }
            }

            minWidth = Math.Max(minWidth, defaultMin.Width);
            minHeight = Math.Max(minHeight, defaultMin.Height);

            return new Size(minWidth, minHeight);
        }

        #endregion

        #region Private Implementation

        /// <summary>
        /// Special measure logic, used when there is at least one fill panel in the layout info.
        /// </summary>
        private void MeasureWithFillPanel()
        {
            int remaining = this.layoutInfo.availableLength;
            SplitPanel panel;

            //calculate the desired size of all non-fill panels
            int desiredNonFillLength = 0;

            int count = this.layoutInfo.layoutTargets.Count;
            for (int i = 0; i < count; i++)
            {
                panel = this.layoutInfo.layoutTargets[i];
                if (panel == this.layoutInfo.fillPanel)
                {
                    continue;
                }

                desiredNonFillLength += this.GetLength(panel.SizeInfo.AbsoluteSize);
            }

            SplitPanelSizeInfo fillInfo = this.layoutInfo.fillPanel.SizeInfo;
            //calculate the correction that needs to be applied
            int layoutableLength = this.layoutInfo.availableLength - this.layoutInfo.totalSplitterLength;
            int correction = 0;
            int desiredFillLength = fillInfo.minLength;
            if (desiredNonFillLength + desiredFillLength > layoutableLength)
            {
                correction = (desiredNonFillLength + desiredFillLength) - layoutableLength;
            }

            int remainingCorrection = correction;
            for (int i = 0; i < this.layoutInfo.layoutTargets.Count; i++)
            {
                panel = this.layoutInfo.layoutTargets[i];
                if (panel == this.layoutInfo.fillPanel)
                {
                    continue;
                }

                int length = this.GetLength(panel.SizeInfo.AbsoluteSize);
                if (remainingCorrection > 0 && panel.SizeInfo.SizeMode != SplitPanelSizeMode.Absolute)
                {
                    float factor = (float)length / desiredNonFillLength;
                    int panelCorrection = Math.Max(1, (int)(factor * correction));
                    remainingCorrection -= panelCorrection;
                    length -= panelCorrection;
                }

                panel.SizeInfo.measuredLength = length;
                remaining -= (panel.SizeInfo.measuredLength + this.layoutInfo.splitterLength);
            }

            //finally update fill panel
            this.layoutInfo.fillPanel.SizeInfo.measuredLength = remaining;
        }

        /// <summary>
        /// Default measure logic.
        /// </summary>
        private void MeasureDefault()
        {
            int remainingAutoSize = this.layoutInfo.autoSizeLength;
            SplitPanel panel;

            //distribute length for absolute sized panels
            for (int i = 0; i < this.layoutInfo.absoluteSizeTargets.Count; i++)
            {
                panel = this.layoutInfo.absoluteSizeTargets[i];
                switch (this.layoutInfo.orientation)
                {
                    case Orientation.Vertical:
                        panel.SizeInfo.measuredLength = panel.SizeInfo.AbsoluteSize.Width;
                        break;
                    case Orientation.Horizontal:
                        panel.SizeInfo.measuredLength = panel.SizeInfo.AbsoluteSize.Height;
                        break;
                }

                remainingAutoSize -= this.layoutInfo.splitterLength;
            }

            List<SplitPanel> remainingPanels = new List<SplitPanel>(this.layoutInfo.autoSizeTargets);

            //first size panels with relative size mode
            for (int i = 0; i < remainingPanels.Count; i++)
            {
                panel = remainingPanels[i];
                if (panel.SizeInfo.SizeMode == SplitPanelSizeMode.Relative)
                {
                    float scale = this.GetLengthF(panel.SizeInfo.RelativeRatio);
                    if (scale != 0)
                    {
                        panel.SizeInfo.measuredLength = (int)Math.Round(scale * this.layoutInfo.autoSizeLength);
                        remainingPanels.RemoveAt(i--);
                        remainingAutoSize -= panel.SizeInfo.measuredLength;
                    }
                }
            }

            //distribute remaining size among all panels with splitter correction
            int count = remainingPanels.Count;
            for (int i = 0; i < count; i++)
            {
                panel = remainingPanels[i];
                if (i == count - 1)
                {
                    panel.SizeInfo.measuredLength = remainingAutoSize;
                    break;
                }

                float scale = GetLengthF(panel.SizeInfo.AutoSizeScale) + this.layoutInfo.autoSizeCountFactor;
                panel.SizeInfo.measuredLength = (int)Math.Round(scale * this.layoutInfo.autoSizeLength);
                remainingAutoSize -= panel.SizeInfo.measuredLength;
            }
        }

        /// <summary>
        /// Apply constraints on measured length for each layout target,
        /// having in mind MinSize, MaxSize, available size and other conditions.
        /// </summary>
        private void ClampMeasuredLength()
        {
            int count = this.layoutInfo.layoutTargets.Count;
            if(count == 1)
            {
                return;
            }

            int remainingLength = this.layoutInfo.availableLength - this.layoutInfo.totalSplitterLength;
            int measured;
            int clamped;
            int minLength;
            int maxLength;

            for (int i = 0; i < count; i++)
            {
                SplitPanel panel = this.layoutInfo.layoutTargets[i];
                measured = panel.SizeInfo.measuredLength;
                minLength = panel.SizeInfo.minLength;
                maxLength = this.GetLength(panel.SizeInfo.MaximumSize);

                clamped = measured;
                //apply min-max size restrictions
                if (maxLength > 0)
                {
                    clamped = Math.Min(maxLength, clamped);
                }
                clamped = Math.Max(minLength, clamped);

                //check whether the panel is target of further size adjustments
                if (clamped != measured)
                {
                    this.layoutInfo.autoSizeTargets.Remove(panel);
                }
                
                measured = clamped;
                panel.SizeInfo.measuredLength = measured;

                this.layoutInfo.totalMeasuredLength += measured;
                remainingLength -= measured;
            }

            //final measure pass, that removes or adds additional sizes among all auto-sized panels
            this.FitMeasuredAndLayoutLength();
        }

        /// <summary>
        /// Final pass that determines whether we have less
        /// or more measured length than the currently available one and performs the needed corrections.
        /// </summary>
        private void FitMeasuredAndLayoutLength()
        {
            int layoutLength = this.layoutInfo.availableLength - this.layoutInfo.totalSplitterLength;
            if (this.layoutInfo.totalMinLength >= layoutLength)
            {
                for (int i = 0; i < this.layoutInfo.layoutTargets.Count; i++)
                {
                    SplitPanel panel = this.layoutInfo.layoutTargets[i];
                    if (panel.SizeInfo.SizeMode != SplitPanelSizeMode.Absolute)
                    {
                        panel.SizeInfo.measuredLength = panel.SizeInfo.minLength;
                    }
                }

                return;
            }

            int correction = layoutLength - this.layoutInfo.totalMeasuredLength;
            if (correction == 0)
            {
                return;
            }

            if (this.layoutInfo.autoSizeTargets.Count > 0)
            {
                this.PerformMeasureCorrection(correction);
            }
        }

        private void PerformMeasureCorrection(int correction)
        {
            int sign = Math.Sign(correction);
            if (sign > 0)
            {
                correction = Math.Max(1, correction);
            }
            else
            {
                correction = Math.Min(-1, correction);
            }

            SplitPanel panel;
            float measuredAutoSizeLength = 0;

            for (int i = 0; i < this.layoutInfo.autoSizeTargets.Count; i++)
            {
                panel = this.layoutInfo.autoSizeTargets[i];
                measuredAutoSizeLength += panel.SizeInfo.measuredLength;
            }

            this.ApplyLengthCorrection(this.layoutInfo.autoSizeTargets, correction, measuredAutoSizeLength);
        }

        private void ApplyLengthCorrection(List<SplitPanel> panels, int correction, float measuredLength)
        {
            int sign = Math.Sign(correction);
            int remainingCorrection = correction;
            int counter = 0;
            SplitPanel panel;

            while (panels.Count > 0)
            {
                if (sign < 0 && remainingCorrection >= 0)
                {
                    break;
                }
                if (sign > 0 && remainingCorrection <= 0)
                {
                    break;
                }
                if (counter > panels.Count - 1)
                {
                    counter = 0;
                }

                panel = panels[counter];
                SplitPanelSizeInfo info = panel.SizeInfo;

                float factor = (info.measuredLength - this.GetLength(info.SplitterCorrection)) / measuredLength;
                int panelCorrection = (int)(factor * correction + (.5F * sign));

                if (sign < 0)
                {
                    panelCorrection = Math.Max(panelCorrection, remainingCorrection);
                    panelCorrection = Math.Min(-1, panelCorrection);
                }
                else
                {
                    panelCorrection = Math.Min(panelCorrection, remainingCorrection);
                    panelCorrection = Math.Max(1, panelCorrection);
                }

                int finalLength = info.measuredLength + panelCorrection;
                int minLength = info.minLength;
                if (finalLength < minLength)
                {
                    finalLength = minLength;
                    panels.RemoveAt(counter--);
                }

                remainingCorrection += (info.measuredLength - finalLength);
                info.measuredLength = finalLength;

                counter++;
            }
        }

        /// <summary>
        /// Updates the provides panel after a splitter drag operation.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="dragLength"></param>
        /// <param name="left"></param>
        private void ApplySplitterCorrection(SplitPanel panel, int dragLength, bool left)
        {
            SizeF autoSizeScale = panel.SizeInfo.AutoSizeScale;
            Size absolute = panel.SizeInfo.AbsoluteSize;
            Size correction = panel.SizeInfo.SplitterCorrection;
            SizeF relativeRatio = panel.SizeInfo.RelativeRatio;
            int offset;

            if (left)
            {
                dragLength *= -1;
            }

            switch(this.layoutInfo.Orientation)
            {
                case Orientation.Vertical:
                    correction.Width += dragLength;
                    offset = panel.Width + dragLength;
                    relativeRatio.Width = offset / (float)this.layoutInfo.autoSizeLength;
                    autoSizeScale.Width = relativeRatio.Width - this.layoutInfo.autoSizeCountFactor;
                    absolute.Width = offset;
                    break;
                default:
                    correction.Height += dragLength;
                    offset = panel.Height + dragLength;
                    relativeRatio.Height = offset / (float)this.layoutInfo.autoSizeLength;
                    autoSizeScale.Height = offset / (float)this.layoutInfo.autoSizeLength - this.layoutInfo.autoSizeCountFactor;
                    absolute.Height = offset;
                    break;
            }

            panel.SizeInfo.SplitterCorrection = correction;

            if (this.layoutInfo.fillPanel != null)
            {
                panel.SizeInfo.AbsoluteSize = absolute;
            }
            else
            {
                switch(panel.SizeInfo.SizeMode)
                {
                    case SplitPanelSizeMode.Absolute:
                        panel.SizeInfo.AbsoluteSize = absolute;
                        break;
                    case SplitPanelSizeMode.Relative:
                        panel.SizeInfo.RelativeRatio = relativeRatio;
                        break;
                    case SplitPanelSizeMode.Auto:
                        panel.SizeInfo.AutoSizeScale = autoSizeScale;
                        break;
                }
            }

            if (panel is RadSplitContainer)
            {
                PropagateSplitterChangeOnChildren((RadSplitContainer)panel);
            }
        }

        /// <summary>
        /// Propagates a splitter change down to all children of the specified container.
        /// </summary>
        /// <param name="container"></param>
        private void PropagateSplitterChangeOnChildren(RadSplitContainer container)
        {
        }

        /// <summary>
        /// Gets the viewport origin for the current layout operation.
        /// </summary>
        /// <returns></returns>
        private int GetLayoutOffset()
        {
            int offset;
            if (this.layoutInfo.orientation == Orientation.Horizontal)
            {
                offset = this.layoutInfo.contentRect.Top;
            }
            else
            {
                offset = this.layoutInfo.contentRect.Left;
            }

            return offset;
        }

        /// <summary>
        /// Gets a list with all the descendant panels which SizeMode is SplitPanelSizeMode.Fill
        /// </summary>
        /// <param name="container"></param>
        private void FindFillPanels(RadSplitContainer container)
        {
            foreach (SplitPanel panel in container.SplitPanels)
            {
                if (panel.Collapsed)
                {
                    continue;
                }

                if (panel.SizeInfo.SizeMode == SplitPanelSizeMode.Fill)
                {
                    this.layoutInfo.fillPanel = panel;
                    break;
                }

                RadSplitContainer childContainer = panel as RadSplitContainer;
                if (childContainer != null && this.ContainsFillPanel(childContainer))
                {
                    this.layoutInfo.fillPanel = childContainer;
                }

                if (this.layoutInfo.fillPanel != null)
                {
                    break;
                }
            }
        }

        private bool ContainsFillPanel(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                SplitPanel panel = child as SplitPanel;
                if (panel == null)
                {
                    continue;
                }

                if (panel.SizeInfo.SizeMode == SplitPanelSizeMode.Fill)
                {
                    return true;
                }

                if (this.rootContainerType != null && this.rootContainerType.IsInstanceOfType(panel))
                {
                    continue;
                }

                bool contains = ContainsFillPanel(panel);
                if (contains)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the layout info associated with this layout strategy.
        /// </summary>
        protected SplitContainerLayoutInfo LayoutInfo
        {
            get
            {
                return this.layoutInfo;
            }
        }

        /// <summary>
        /// Gets or sets the Type that is treated as Root for the layout strategy.
        /// Allows for defining how deep the search for a Fill panel should be.
        /// </summary>
        public Type RootContainerType
        {
            get
            {
                return this.rootContainerType;
            }
            set
            {
                this.rootContainerType = value;
            }
        }

        #endregion

        #region Fields

        private SplitContainerLayoutInfo layoutInfo;
        private Type rootContainerType;

        #endregion
    }
}
