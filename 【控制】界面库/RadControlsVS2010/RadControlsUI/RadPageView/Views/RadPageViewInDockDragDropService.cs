using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewInDockDragDropService : RadPageViewDragDropService, IMessageListener
    {
        #region Fields

        private RadPageViewElement owner;
        private bool processing;

        #endregion

        #region Constructor

        public RadPageViewInDockDragDropService(RadPageViewElement owner)
            : base(owner)
        {
            this.owner = owner;
        }

        #endregion

        #region IMessageListener Members

        InstalledHook IMessageListener.DesiredHook
        {
            get
            {
                return InstalledHook.CallWndProc | InstalledHook.GetMessage;
            }
        }

        MessagePreviewResult IMessageListener.PreviewMessage(ref Message msg)
        {
            RadServiceState state = this.State;
            if (state == RadServiceState.Stopped || state == RadServiceState.Initial)
            {
                RadMessageFilter.Instance.RemoveListener(this);
                this.messageFilterAdded = false;
                return MessagePreviewResult.NotProcessed;
            }

            int message = msg.Msg;

            switch (message)
            {
                case NativeMethods.WM_MOUSEMOVE:
                case NativeMethods.WM_MOVING:
                    Point mousePos = Control.MousePosition;
                    if (mousePos != this.beginPoint.Value)
                    {
                        this.HandleMouseMove(mousePos);
                    }
                    return MessagePreviewResult.Processed;

                case NativeMethods.WM_NCLBUTTONUP:
                case NativeMethods.WM_LBUTTONUP:
                    this.Stop(this.CanCommit);
                    return MessagePreviewResult.Processed;
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                    if ((Keys)msg.WParam.ToInt32() == Keys.Escape)
                    {
                        this.Stop(false);
                        return MessagePreviewResult.Processed;
                    }
                    break;
            }

            return MessagePreviewResult.Processed;
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
            if (msg.Msg == NativeMethods.WM_ACTIVATEAPP)
            {
                base.Stop(false);
            }
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
        }

        #endregion

        #region Overrides

        protected override void PerformStart()
        {
            this.processing = true;
            base.PerformStart();
        }

        protected override void HandleMouseMove(Point mousePos)
        {
            base.HandleMouseMove(mousePos);
            this.UpdateCursor(mousePos);
        }

        #endregion

        #region Methods

        private void UpdateCursor(Point mousePos)
        {
            RadPageViewStripElement stripElement = this.owner as RadPageViewStripElement;
            if (stripElement == null)
            {
                return;
            }

            if (!this.processing)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            Point mousePt = Control.MousePosition;
            if (this.owner.IsInValidState(true))
            {
                mousePt = this.owner.ElementTree.Control.PointToClient(mousePt);
            }
            else
            {
                mousePt = mousePos;
            }

            if (!stripElement.ItemContainer.ControlBoundingRectangle.Contains(mousePt))
            {
                Cursor.Current = Cursors.Default;
                base.HintWindow.BackgroundImage = null;
                base.HintWindow.Hide();

                this.processing = false;
            }
        }

        #endregion
    }
}