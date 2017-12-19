using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a popup layered window that displays a docking guide
    /// </summary>
    internal class DockingGuidePopup : RadLayeredWindow
    {
        #region Constructor

        public DockingGuidePopup(DragDropService owner, DockingGuidesPosition position)
        {
            this.owner = owner;
            this.position = position;
            this.lastHitPosition = null;

            this.TopMost = true;
            this.HitTestable = false;

            UpdateFromPosition();
        }

        private void UpdateFromPosition()
        {
            Image background = null;
            IDockingGuidesTemplate template = this.owner.DockManager.DockingGuidesTemplate;

            if (this.position == DockingGuidesPosition.Center)
            {
                this.allowedDockPosition = AllowedDockPosition.All;
                background = template.CenterBackgroundImage.Image;
                this.UpdateCenterRects();
            }
            else
            {
                switch (this.position)
                {
                    case DockingGuidesPosition.Bottom:
                        background = template.BottomImage.Image;
                        this.allowedDockPosition = AllowedDockPosition.Bottom;
                        break;
                    case DockingGuidesPosition.Left:
                        background = template.LeftImage.Image;
                        this.allowedDockPosition = AllowedDockPosition.Left;
                        break;
                    case DockingGuidesPosition.Right:
                        background = template.RightImage.Image;
                        this.allowedDockPosition = AllowedDockPosition.Right;
                        break;
                    case DockingGuidesPosition.Top:
                        background = template.TopImage.Image;
                        this.allowedDockPosition = AllowedDockPosition.Top;
                        break;
                }
            }

            this.BackgroundImage = background;
        }

        private void UpdateCenterRects()
        {
            this.centerPositionRects = new KeyValuePair<AllowedDockPosition, Rectangle>[5];
            Rectangle bounds;
            IDockingGuidesTemplate template = this.owner.DockManager.DockingGuidesTemplate;
            int index = 0;

            //left rect
            
            bounds = new Rectangle(template.LeftImage.LocationOnCenterGuide, template.LeftImage.PreferredSize);
            this.centerPositionRects[index++] = new KeyValuePair<AllowedDockPosition, Rectangle>(AllowedDockPosition.Left, bounds);

            //top rect
            bounds = new Rectangle(template.TopImage.LocationOnCenterGuide, template.TopImage.PreferredSize);
            this.centerPositionRects[index++] = new KeyValuePair<AllowedDockPosition, Rectangle>(AllowedDockPosition.Top, bounds);

            //right rect
            bounds = new Rectangle(template.RightImage.LocationOnCenterGuide, template.RightImage.PreferredSize);
            this.centerPositionRects[index++] = new KeyValuePair<AllowedDockPosition, Rectangle>(AllowedDockPosition.Right, bounds);

            //bottom rect
            bounds = new Rectangle(template.BottomImage.LocationOnCenterGuide, template.BottomImage.PreferredSize);
            this.centerPositionRects[index++] = new KeyValuePair<AllowedDockPosition, Rectangle>(AllowedDockPosition.Bottom, bounds);

            //fill rect
            bounds = new Rectangle(template.FillImage.LocationOnCenterGuide, template.FillImage.PreferredSize);
            this.centerPositionRects[index++] = new KeyValuePair<AllowedDockPosition, Rectangle>(AllowedDockPosition.Fill, bounds);
        }

        private void UpdateTemplate()
        {
            if (this.position == DockingGuidesPosition.Center)
            {
                return;
            }

            Image background = null;
            IDockingGuidesTemplate template = this.owner.DockManager.DockingGuidesTemplate;

            switch (this.position)
            {
                case DockingGuidesPosition.Bottom:
                    background = template.BottomImage.Image;
                    break;
                case DockingGuidesPosition.Left:
                    background = template.LeftImage.Image;
                    break;
                case DockingGuidesPosition.Right:
                    background = template.RightImage.Image;
                    break;
                case DockingGuidesPosition.Top:
                    background = template.TopImage.Image;
                    break;
            }
            this.BackgroundImage = background;
        }

        #endregion

        #region Overrides

        public override Image BackgroundImage
        {
            get
            {
                DockPosition pos = DockPositionFromGuidePosition(this.position);
                Image image = base.BackgroundImage;

                if (pos == this.lastHitPosition)
                {
                    IDockingGuidesTemplate template = this.owner.DockManager.DockingGuidesTemplate;
                    switch (pos)
                    {
                        case DockPosition.Left:
                            image = template.LeftImage.HotImage;
                            break;
                        case DockPosition.Top:
                            image = template.TopImage.HotImage;
                            break;
                        case DockPosition.Right:
                            image = template.RightImage.HotImage;
                            break;
                        case DockPosition.Bottom:
                            image = template.BottomImage.HotImage;
                            break;
                    }
                }
                
                return image;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        protected override void PaintWindow(Graphics g, Bitmap graphicsBitmap)
        {
            base.PaintWindow(g, graphicsBitmap);

            //Center docking position needs special handling
            if (this.position != DockingGuidesPosition.Center)
            {
                return;
            }

            IDockingGuidesTemplate template = this.owner.DockManager.DockingGuidesTemplate;
            Rectangle imageBounds;
            Image image;

            //if (template.CenterBackgroundImage != null)
            //{
            //    g.DrawImage(template.CenterBackgroundImage.Image, 
            //        new Rectangle(template.CenterBackgroundImage.LocationOnCenterGuide, template.CenterBackgroundImage.PreferredSize));
            //}

            if ((this.allowedDockPosition & AllowedDockPosition.Left) == AllowedDockPosition.Left)
            {
                image = this.lastHitPosition == DockPosition.Left ? template.LeftImage.HotImage : template.LeftImage.Image;
                imageBounds = new Rectangle(template.LeftImage.LocationOnCenterGuide, template.LeftImage.PreferredSize);

                g.DrawImage(image, imageBounds);
            }
            if ((this.allowedDockPosition & AllowedDockPosition.Top) == AllowedDockPosition.Top)
            {
                image = this.lastHitPosition == DockPosition.Top ? template.TopImage.HotImage : template.TopImage.Image;
                imageBounds = new Rectangle(template.TopImage.LocationOnCenterGuide, template.TopImage.PreferredSize);

                g.DrawImage(image, imageBounds);
            }
            if ((this.allowedDockPosition & AllowedDockPosition.Right) == AllowedDockPosition.Right)
            {
                image = this.lastHitPosition == DockPosition.Right ? template.RightImage.HotImage : template.RightImage.Image;
                imageBounds = new Rectangle(template.RightImage.LocationOnCenterGuide, template.RightImage.PreferredSize);

                g.DrawImage(image, imageBounds);
            }
            if ((this.allowedDockPosition & AllowedDockPosition.Bottom) == AllowedDockPosition.Bottom)
            {
                image = this.lastHitPosition == DockPosition.Bottom ? template.BottomImage.HotImage : template.BottomImage.Image;
                imageBounds = new Rectangle(template.BottomImage.LocationOnCenterGuide, template.BottomImage.PreferredSize);

                g.DrawImage(image, imageBounds);
            }
            if ((this.allowedDockPosition & AllowedDockPosition.Fill) == AllowedDockPosition.Fill)
            {
                image = this.lastHitPosition == DockPosition.Fill ? template.FillImage.HotImage : template.FillImage.Image;
                imageBounds = new Rectangle(template.FillImage.LocationOnCenterGuide, template.FillImage.PreferredSize);

                g.DrawImage(image, imageBounds);
            }
        }

        #endregion

        #region Methods

        internal void Display(Point screenLocation)
        {
            UpdateTemplate();
            this.ShowWindow(screenLocation);
        }

        internal void ProcessMouseMove()
        {
        }

        internal DockingGuideHitTest HitTest(Point screenMouse)
        {
            if (!this.Visible)
            {
                return DockingGuideHitTest.Empty;
            }

            DockPosition? pos = null;
            DockingGuidesPosition? guidePos = null;

            Point clientMouse = this.PointToClient(screenMouse);

            if (this.IsMouseOverNonTransparentPixel(clientMouse))
            {
                pos = this.GetDockPosition(clientMouse);
                if (pos != null)
                {
                    guidePos = this.position;
                }
            }

            bool update = this.lastHitPosition != pos;
            this.lastHitPosition = pos;
            if (update)
            {
                this.UpdateWindow();
            }

            return new DockingGuideHitTest(pos, guidePos);
        }

        private bool IsMouseOverNonTransparentPixel(Point clientMouse)
        {
            if (!this.ClientRectangle.Contains(clientMouse))
            {
                return false;
            }

            return this.Content.GetPixel(clientMouse.X, clientMouse.Y).A > 0;
        }

        private DockPosition? GetDockPosition(Point client)
        {
            DockPosition? pos = null;

            switch (this.position)
            {
                case DockingGuidesPosition.Left:
                    if ((this.allowedDockPosition & AllowedDockPosition.Left) != 0)
                    {
                        pos = DockPosition.Left;
                    }
                    break;
                case DockingGuidesPosition.Top:
                    if ((this.allowedDockPosition & AllowedDockPosition.Top) != 0)
                    {
                        pos = DockPosition.Top;
                    }
                    break;
                case DockingGuidesPosition.Right:
                    if ((this.allowedDockPosition & AllowedDockPosition.Right) != 0)
                    {
                        pos = DockPosition.Right;
                    }
                    break;
                case DockingGuidesPosition.Bottom:
                    if ((this.allowedDockPosition & AllowedDockPosition.Bottom) != 0)
                    {
                        pos = DockPosition.Bottom;
                    }
                    break;
                case DockingGuidesPosition.Center:
                    //check which part the mouse is currently over
                    foreach (KeyValuePair<AllowedDockPosition, Rectangle> pair in this.centerPositionRects)
                    {
                        if (pair.Value.Contains(client))
                        {
                            if ((this.allowedDockPosition & pair.Key) != 0)
                            {
                                pos = DockHelper.GetDockPosition(pair.Key);
                            }
                            break;
                        }
                    }
                    break;
            }

            return pos;
        }

        private static DockPosition DockPositionFromGuidePosition(DockingGuidesPosition pos)
        {
            DockPosition dockPos = DockPosition.Left;
            switch (pos)
            {
                case DockingGuidesPosition.Top:
                    dockPos = DockPosition.Top;
                    break;
                case DockingGuidesPosition.Bottom:
                    dockPos = DockPosition.Bottom;
                    break;
                case DockingGuidesPosition.Right:
                    dockPos = DockPosition.Right;
                    break;
                case DockingGuidesPosition.Center:
                    dockPos = DockPosition.Fill;
                    break;
            }

            return dockPos;
        }

        #endregion

        #region Properties

        public AllowedDockPosition AllowedDockPosition
        {
            get
            {
                return this.allowedDockPosition;
            }
            set
            {
                if (this.allowedDockPosition == value)
                {
                    return;
                }

                this.allowedDockPosition = value;
                this.Updated = false;
            }
        }

        public DockingGuidesPosition Position
        {
            get
            {
                return this.position;
            }
        }

        #endregion

        #region Fields

        private DockPosition? lastHitPosition;
        private AllowedDockPosition allowedDockPosition;
        private DockingGuidesPosition position;
        private DragDropService owner;

        private KeyValuePair<AllowedDockPosition, Rectangle>[] centerPositionRects;

        #endregion
    }
}
