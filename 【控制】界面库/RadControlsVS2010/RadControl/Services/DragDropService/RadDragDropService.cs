using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a service that manages drag and drop actions.
    /// </summary>
    public class RadDragDropService : RadService, IMessageListener
    {
        #region Event Keys

        private static readonly object PreviewDragOverEventKey;
        private static readonly object PreviewDragDropEventKey;
        private static readonly object PreviewDropTargetEventKey;
        private static readonly object PreviewDragStartEventKey;
        private static readonly object PreviewDragHintEventKey;

        #endregion

        #region Fields

        private Cursor validCursor;
        private Cursor invalidCursor;

        private RadLayeredWindow hintWindow;
        protected bool messageFilterAdded = false;
        private int xOutlineFormOffset = 0;
        private int yOutlineFormOffset = 0;
        protected Point? beginPoint;
        private Point dropLocation;
        private ISupportDrop target;
        private bool doCommit = false;
        private bool initialized = false;
        private bool useDefaultPreview;
        private Cursor originalMouseCursor = null;
        private static Cursor DefaultValidCursor;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DragDropService class.
        /// </summary>
        public RadDragDropService()
        {
            this.validCursor = ValidCursor;
            this.hintWindow = new RadLayeredWindow();
            this.hintWindow.TopMost = true;
            this.hintWindow.Alpha = 0.7F;
            this.hintWindow.HitTestable = false;
            this.useDefaultPreview = true;
        }

        static RadDragDropService()
        {
            PreviewDragOverEventKey = new object();
            PreviewDragDropEventKey = new object();
            PreviewDropTargetEventKey = new object();
            PreviewDragStartEventKey = new object();
            PreviewDragHintEventKey = new object();

            string path = "Telerik.WinControls.Resources.Cursors.";
            DefaultValidCursor = ResourceHelper.CursorFromResource(typeof(RadDragDropService), path + "DragMove.cur");
        }

        protected override void DisposeManagedResources()
        {
            if (this.hintWindow != null)
            {
                this.hintWindow.Dispose();
            }

            base.DisposeManagedResources();
        }

        #endregion

        #region Events

        public event EventHandler<RadDropEventArgs> PreviewDragDrop
        {
            add
            {
                this.Events.AddHandler(PreviewDragDropEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PreviewDragDropEventKey, value);
            }
        }

        public event EventHandler<RadDragOverEventArgs> PreviewDragOver
        {
            add
            {
                this.Events.AddHandler(PreviewDragOverEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PreviewDragOverEventKey, value);
            }
        }

        public event EventHandler<PreviewDropTargetEventArgs> PreviewDropTarget
        {
            add
            {
                this.Events.AddHandler(PreviewDropTargetEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PreviewDropTargetEventKey, value);
            }
        }

        public event EventHandler<PreviewDragStartEventArgs> PreviewDragStart
        {
            add
            {
                this.Events.AddHandler(PreviewDragStartEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PreviewDragStartEventKey, value);
            }
        }

        public event EventHandler<PreviewDragHintEventArgs> PreviewDragHint
        {
            add
            {
                this.Events.AddHandler(PreviewDragHintEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PreviewDragHintEventKey, value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether a drop operation will be committed (a valid drop target is found).
        /// </summary>
        protected bool CanCommit
        {
            get
            {
                return this.doCommit;
            }
        }

        /// <summary>
        /// Gets or sets the cursor to be used when a valid drop target is hit-tested.
        /// </summary>
        public Cursor ValidCursor
        {
            get
            {
                if (this.validCursor != null)
                {
                    return this.validCursor;
                }

                return DefaultValidCursor;
            }
            set
            {
                this.validCursor = value;
            }
        }

        protected bool Initialized
        {
            get
            {
                return this.initialized;
            }
        }

        /// <summary>
        /// Gets or sets the cursor to be used when a valid drop target is hit-tested.
        /// </summary>
        public Cursor InvalidCursor
        {
            get
            {
                if (this.invalidCursor != null)
                {
                    return this.invalidCursor;
                }

                return Cursors.No;
            }
            set
            {
                this.invalidCursor = value;
            }
        }

        /// <summary>
        /// Determines whether a default preview is generated for a ISupportDrag instance if its GetPreview method returns null.
        /// </summary>
        [DefaultValue(true)]
        public bool UseDefaultPreview
        {
            get
            {
                return this.useDefaultPreview;
            }
            set
            {
                this.useDefaultPreview = value;
            }
        }

        /// <summary>
        /// Gets current drop target, where the mouse cursor points.
        /// </summary>
        public ISupportDrop DropTarget
        {
            get
            {
                return this.target;
            }
        }

        /// <summary>
        /// Gets the current drop location in the context of the current target.
        /// </summary>
        public Point DropLocation
        {
            get
            {
                return this.dropLocation;
            }
        }

        /// <summary>
        /// Gets the Hint window.
        /// </summary>
        /// <value>The hint window.</value>
        protected RadLayeredWindow HintWindow
        {
            get { return this.hintWindow; }
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
                    return MessagePreviewResult.ProcessedNoDispatch;

                case NativeMethods.WM_NCLBUTTONUP:
                case NativeMethods.WM_LBUTTONUP:
                    MessagePreviewResult result = this.initialized ? MessagePreviewResult.ProcessedNoDispatch : MessagePreviewResult.Processed;
                    this.Stop(this.doCommit);
                    return result;
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                    if ((Keys)msg.WParam.ToInt32() == Keys.Escape)
                    {
                        this.Stop(false);
                        return MessagePreviewResult.ProcessedNoDispatch;
                    }
                    break;
            }


            return MessagePreviewResult.Processed;
        }

        protected virtual void HandleMouseMove(Point mousePos)
        {
            if (!this.initialized && ShouldBeginDrag(this.beginPoint.Value, mousePos))
            {
                this.initialized = this.PrepareContext();
            }

            if (!this.initialized)
            {
                return;
            }

            try
            {
                this.DoDrag(mousePos);
                this.beginPoint = mousePos;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected virtual bool PrepareContext()
        {
            //remember the cursor so that we may restore it when service is stopped.
            this.originalMouseCursor = Cursor.Current;

            RadItem contextElement = this.Context as RadItem;
            if (!contextElement.Capture)
            {
                contextElement.Capture = true;
            }

            //prepare the hint window
            ISupportDrag dragObject = this.Context as ISupportDrag;
            PreviewDragHintEventArgs args = new PreviewDragHintEventArgs(dragObject);
            this.OnPreviewDragHint(args);

            if (args.DragHint == null && args.UseDefaultHint)
            {
                args.DragHint = dragObject.GetDragHint();
            }

            this.hintWindow.BackgroundImage = args.DragHint;

            Size offsetSize;
            if (args.DragHint == null)
            {
                offsetSize = contextElement == null ? Size.Empty : contextElement.Size;
            }
            else
            {
                offsetSize = args.DragHint.Size;
            }

            this.xOutlineFormOffset = offsetSize.Width / 2;
            this.yOutlineFormOffset = offsetSize.Height / 2;
            return true;
        }

        protected virtual void OnPreviewDragHint(PreviewDragHintEventArgs e)
        {
            EventHandler<PreviewDragHintEventArgs> eh = this.Events[PreviewDragHintEventKey] as EventHandler<PreviewDragHintEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
            if (msg.Msg == NativeMethods.WM_ACTIVATEAPP)
            {
                this.Stop(false);
            }
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        private void DoDrag(Point mousePt)
        {
            RadItem draggedElement = this.Context as RadItem;
            this.SetHintWindowPosition(mousePt);

            Point oldDropLocation = this.dropLocation;
            ISupportDrop dropTarget = this.GetDropTarget(Control.MousePosition, out this.dropLocation);

            if (dropTarget == null || !this.IsDropTargetValid(dropTarget))
            {
                Cursor.Current = this.InvalidCursor;
                this.doCommit = false;
                return;
            }

            if (dropTarget != null)
            {
                ISupportDrag draggedContext = this.Context as ISupportDrag;
                if (this.target != dropTarget)
                {
                    if (this.target != null)
                    {
                        this.target.DragLeave(oldDropLocation, draggedContext);
                    }

                    this.target = dropTarget;
                    this.target.DragEnter(this.dropLocation, draggedContext);
                }

                this.doCommit = this.target.DragOver(this.dropLocation, draggedContext);

                RadDragOverEventArgs args = new RadDragOverEventArgs(draggedContext, this.target);
                args.CanDrop = this.doCommit;
                this.OnPreviewDragOver(args);

                //use the CanDrop member of the event as it may be altered
                this.doCommit = args.CanDrop;

                if (this.doCommit)
                {
                    //TODO: Provide logic for updating cursor with events and DragDropEffects
                    Cursor.Current = this.ValidCursor;
                }
                else
                {
                    Cursor.Current = this.InvalidCursor;
                }
            }
            else
            {
                this.target = null;
                this.doCommit = false;
            }
        }

        protected virtual bool IsDropTargetValid(ISupportDrop dropTarget)
        {
            return dropTarget != this.Context;
        }

        protected virtual void OnPreviewDragOver(RadDragOverEventArgs e)
        {
            EventHandler<RadDragOverEventArgs> eh = this.Events[PreviewDragOverEventKey] as EventHandler<RadDragOverEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        private void SetInvalidTargetMouseCursor()
        {
            if (this.originalMouseCursor == null)
            {
                this.originalMouseCursor = Cursor.Current;
            }

            Cursor.Current = this.InvalidCursor;
        }

        private void RestoreOriginalMouseCursort()
        {
            if (this.originalMouseCursor != null)
            {
                Cursor.Current = this.originalMouseCursor;
                this.originalMouseCursor = null;
            }
        }

        private void SetHintWindowPosition(Point mousePt)
        {
            //no image is specified for the hint
            if (this.hintWindow.BackgroundImage == null)
            {
                return;
            }

            Point location = new Point(mousePt.X - this.xOutlineFormOffset, mousePt.Y - this.yOutlineFormOffset);
            this.hintWindow.ShowWindow(location);
        }

        public static bool ShouldBeginDrag(Point current, Point capture)
        {
            Size dragSize = SystemInformation.DragSize;
            Rectangle dragRect = new Rectangle(capture.X - dragSize.Width / 2,
                                               capture.Y - dragSize.Height / 2,
                                               dragSize.Width, dragSize.Height);
            return !dragRect.Contains(current);
        }

        #endregion

        #region Override

        protected override bool CanStart(object context)
        {
            RadItem draggedItem = context as RadItem;
            if (draggedItem == null)
            {
                return false;
            }

            if (!this.beginPoint.HasValue)
            {
                this.beginPoint = Control.MousePosition;
            }

            Point mouseDownStartPostion = draggedItem.ElementTree.Control.PointToClient(this.beginPoint.Value);
            bool canStartDrag = (draggedItem as ISupportDrag).CanDrag(mouseDownStartPostion);

            PreviewDragStartEventArgs args = new PreviewDragStartEventArgs(draggedItem);
            args.CanStart = canStartDrag;
            this.OnPreviewDragStart(args);

            if (!args.CanStart)
            {
                this.beginPoint = null;
            }

            return args.CanStart;
        }

        protected virtual void OnPreviewDragStart(PreviewDragStartEventArgs e)
        {
            EventHandler<PreviewDragStartEventArgs> eh = this.Events[PreviewDragStartEventKey] as EventHandler<PreviewDragStartEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected override void PerformStart()
        {
            if (!messageFilterAdded)
            {
                RadMessageFilter.Instance.AddListener(this);
                messageFilterAdded = true;
            }
        }

        protected override void Commit()
        {
            RadItem draggedItem = this.Context as RadItem;
            Debug.Assert(draggedItem != null, "Invalid drag instance");

            draggedItem.Capture = false;

            RadDropEventArgs args = new RadDropEventArgs(draggedItem, this.target, this.dropLocation);
            this.OnPreviewDragDrop(args);
            if (args.Handled)
            {
                return;
            }

            this.target.DragDrop(this.dropLocation, draggedItem);
        }

        protected virtual void OnPreviewDragDrop(RadDropEventArgs e)
        {
            EventHandler<RadDropEventArgs> eh = this.Events[PreviewDragDropEventKey] as EventHandler<RadDropEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        private ISupportDrop GetDropTarget(Point mousePosition, out Point resultDropLocation)
        {
            //hittest the current control under the mouse
            ComponentThemableElementTree elementTree = this.ElementTreeFromPoint(mousePosition);
            return this.HitTestElementTree(elementTree, mousePosition, out resultDropLocation);
        }

        private ComponentThemableElementTree ElementTreeFromPoint(Point mousePosition)
        {
            IntPtr hitWindow = NativeMethods.WindowFromPoint(mousePosition.X, mousePosition.Y);

            if (hitWindow == IntPtr.Zero)
            {
                return null;
            }

            Control hitControl = Control.FromHandle(hitWindow);

            while (hitControl != null)
            {
                IComponentTreeHandler treeHandler = hitControl as IComponentTreeHandler;

                if (treeHandler != null)
                {
                    return treeHandler.ElementTree;
                }

                hitControl = hitControl.Parent;
            }

            return null;
        }

        private ISupportDrop HitTestElementTree(ComponentThemableElementTree elementTree, Point screenMouse, out Point resultDropLocation)
        {
            if (elementTree == null)
            {
                resultDropLocation = Point.Empty;
                return null;
            }

            resultDropLocation = Point.Empty;
            Point clientMouse = elementTree.Control.PointToClient(screenMouse);
            RadElement hitElement = elementTree.GetElementAtPoint(clientMouse);
            ISupportDrag dragInstance = this.Context as ISupportDrag;

            ISupportDrop dropTarget = null;
            ISupportDrop hitTarget = null;

            while (hitElement != null)
            {
                hitTarget = hitElement as ISupportDrop;
                dropTarget = null;

                if (hitTarget != null && hitTarget.AllowDrop)
                {
                    RadElement dropTargetElement = hitTarget as RadElement;

                    if (dropTargetElement != null)
                    {
                        resultDropLocation = dropTargetElement.PointFromControl(clientMouse);
                    }
                    else
                    {
                        resultDropLocation = clientMouse;
                    }

                    dropTarget = hitTarget;
                }

                //raise PreviewDropTarget event
                PreviewDropTargetEventArgs args = new PreviewDropTargetEventArgs(dragInstance, hitTarget);
                args.DropTarget = dropTarget;
                this.OnPreviewDropTarget(args);
                dropTarget = args.DropTarget;

                if (dropTarget != null)
                {
                    return dropTarget;
                }

                hitElement = hitElement.Parent;
            }

            return null;
        }

        protected virtual void OnPreviewDropTarget(PreviewDropTargetEventArgs e)
        {
            EventHandler<PreviewDropTargetEventArgs> eh = this.Events[PreviewDropTargetEventKey] as EventHandler<PreviewDropTargetEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected override void PerformStop()
        {
            base.PerformStop();

            if (this.messageFilterAdded)
            {
                RadMessageFilter.Instance.RemoveListener(this);
                this.messageFilterAdded = false;
            }

            if (this.initialized)
            {
                RadItem item = this.Context as RadItem;
                if (item != null && item.ElementState == ElementState.Loaded)
                {
                    item.IsMouseDown = false;
                    item.Capture = false;
                    item.ElementTree.ComponentTreeHandler.Behavior.ItemCapture = null;
                    item.ElementTree.ComponentTreeHandler.Behavior.selectedElement = null;
                }
            }

            this.beginPoint = null;
            this.doCommit = false;
            this.dropLocation = Point.Empty;

            this.xOutlineFormOffset = 0;
            this.yOutlineFormOffset = 0;

            this.hintWindow.Visible = false;
            this.hintWindow.BackgroundImage = null;

            this.target = null;
            this.initialized = false;

            this.RestoreOriginalMouseCursort();
        }

        #endregion

        #region Programatically API For Drag & Drop

        /// <summary>
        /// Begins a drag pass. Allows for service automation.
        /// </summary>
        /// <param name="mouseBeginPoint">The position of the mouse cursor in screen coordinates.</param>
        /// <param name="draggedObject">An instance of IDraggable that is dragged.</param>
        public void BeginDrag(Point mouseBeginPoint, ISupportDrag draggedObject)
        {
            if (this.State == RadServiceState.Working)
            {
                return;
            }

            this.beginPoint = mouseBeginPoint;

            this.Start(draggedObject);

            if (this.State == RadServiceState.Working)
            {
                this.PrepareContext();
                this.initialized = true;
            }
        }

        /// <summary>
        /// Ends a drag pass. Allows for service automation.
        /// </summary>
        /// <param name="mouseEndPoint">The end position of the mouse cursor in screen coordinates.</param>
        /// <param name="targetControl">An instance of <see cref="RadControl"/>.</param>
        public void EndDrag(Point mouseEndPoint, RadControl targetControl)
        {
            if (this.State == RadServiceState.Stopped)
            {
                return;
            }

            this.target = this.HitTestElementTree(targetControl.ElementTree, mouseEndPoint, out this.dropLocation);

            if (this.target != null)
            {
                this.Stop(true);
            }
            else
            {
                this.Stop(false);
            }
        }

        #endregion
    }
}
