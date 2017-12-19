using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Elements;
using Telerik.WinControls.Layout;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.Xml;
using System.IO;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the main element of the <see cref="RadCommandBar">RadCommandBar</see> control.
    /// Contains a collection of <see cref="CommandBarRowElement"/> element.
    /// </summary>
    public class RadCommandBarElement : RadCommandBarVisualElement
    {
        #region Fields

        protected RadCommandBarLinesElementCollection lines;
        protected StackLayoutPanel layoutPanel;
        protected Size dragSize = SystemInformation.DragSize;
        protected CommandBarStripInfoHolder stripInfoHolder;

        #endregion

        #region Cstors

        static RadCommandBarElement()
        {
            new Themes.ControlDefault.CommandBar().DeserializeTheme();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before dragging is started.
        /// </summary>
        public event CancelEventHandler BeginDragging;

        /// <summary>
        /// Occurs when item is being dragged.
        /// </summary>
        public event MouseEventHandler Dragging;

        /// <summary>
        /// Occurs when item is released and dragging is stopped.
        /// </summary>
        public event EventHandler EndDragging;

        /// <summary>
        /// Occurs when Orientation property is changed.
        /// </summary>
        public event EventHandler OrientationChanged;

        /// <summary>
        /// Occurs before Orientation property is changed.
        /// </summary>
        public event CancelEventHandler OrientationChanging;

        /// <summary>
        /// Occurs before a floating form is created.
        /// </summary>
        public event CancelEventHandler FloatingStripCreating;

        /// <summary>
        /// Occurs before a floating strip is docked.
        /// </summary>
        public event CancelEventHandler FloatingStripDocking;

        /// <summary>
        /// Occurs when a floating strip is created.
        /// </summary>
        public event EventHandler FloatingStripCreated;

        /// <summary>
        /// Occurs when a floating strip is docked.
        /// </summary>
        public event EventHandler FloatingStripDocked;
        #endregion

        #region Overrides

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF measuredSize = base.MeasureOverride(availableSize);
            if (float.IsInfinity(availableSize.Width))
            {
                availableSize.Width = measuredSize.Width;
            }

            if (float.IsInfinity(availableSize.Height))
            {
                availableSize.Height = measuredSize.Height;
            }
            return availableSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.AllowDrop = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.stripInfoHolder = new CommandBarStripInfoHolder();

            this.Text = "";
            this.layoutPanel = new StackLayoutPanel();

            this.lines = new RadCommandBarLinesElementCollection(this.layoutPanel);
            this.lines.ItemTypes = new Type[] { typeof(CommandBarRowElement) };

            this.DrawBorder = false;
            this.DrawFill = true;
            this.SetDefaultValueOverride(RadElement.MinSizeProperty, new Size(30, 30));
            this.Children.Add(layoutPanel);

            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.SetOrientationCore(this.Orientation);
            this.WireEvents();
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();
            base.DisposeManagedResources();
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            if (args.RoutedEvent == RadCommandBarGrip.BeginDraggingEvent)
            {
                CancelEventArgs dragArgs = (CancelEventArgs)args.OriginalEventArgs;
                OnBeginDragging(sender, dragArgs);
                args.Canceled = dragArgs.Cancel;
            }

            if (args.RoutedEvent == RadCommandBarGrip.EndDraggingEvent)
            {
                EventArgs dragArgs = args.OriginalEventArgs;
                OnEndDragging(sender, dragArgs);
            }

            if (args.RoutedEvent == RadCommandBarGrip.DraggingEvent)
            {
                MouseEventArgs dragArgs = (MouseEventArgs)args.OriginalEventArgs;
                OnDragging(sender, dragArgs);
            }
        }

        #endregion

        #region Events Management

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.FloatinStripCreating"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event - usually this is the strip that is going to be floating.</param>
        /// <returns>True if the creating of a floating form should be canceled, False otherwise.</returns>
        protected virtual bool OnFloatingStripCreating(object sender)
        {
            if(this.FloatingStripCreating != null)
            {
                CancelEventArgs cancelArgs = new CancelEventArgs();
                this.FloatingStripCreating(sender,cancelArgs);
                return cancelArgs.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.FloatinStripCreated"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event - usually this is the strip that is made floating.</param>
        protected virtual void OnFloatingStripCreated(object sender)
        {
            if (this.FloatingStripCreated != null)
            {
                this.FloatingStripCreated(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.FloatingStripDocking"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event - usually this is the strip that is going to be docked.</param>
        /// <returns>True if the docking of a floating form should be canceled, False otherwise.</returns>
        protected virtual bool OnFloatingStripDocking(object sender)
        {
            if (this.FloatingStripDocking != null)
            {
                CancelEventArgs cancelArgs = new CancelEventArgs();
                this.FloatingStripDocking(sender, cancelArgs);
                return cancelArgs.Cancel;
            }
            return false;
        }

        internal bool CallOnFloatingStripDocking(object sender)
        {
            return this.OnFloatingStripDocking(sender);
        }

        internal void CallOnFloatingStripDocked(object sender)
        {
            this.OnFloatingStripDocked(sender);
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.FloatingStripDocked"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event - usually this is the strip that was docked.</param>
        protected virtual void OnFloatingStripDocked(object sender)
        {
            if (this.FloatingStripDocked != null)
            {
                this.FloatingStripDocked(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.BeginDragging"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnBeginDragging(object sender, CancelEventArgs args)
        {
            if (BeginDragging != null)
            {
                BeginDragging(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.EndDragging"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnEndDragging(object sender, EventArgs args)
        {
            if (EndDragging != null)
            {
                EndDragging(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.Dragging"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnDragging(object sender, MouseEventArgs args)
        {
            if (Dragging != null)
            {
                Dragging(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.OrientationChanged"/> event
        /// </summary> 
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOrientationChanged(EventArgs e)
        {
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarElement.OrientationChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>True if the change of orientation should be canceled, false otherwise.</returns>
        protected virtual bool OnOrientationChanging(CancelEventArgs e)
        {
            if (this.OrientationChanging != null)
            {
                this.OrientationChanging(this, e);
                return e.Cancel;
            }
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Moves a specific <see cref="CommandBarStripElement"/> to the upper <see cref="CommandBarRowElement"/>.
        /// </summary>
        /// <param name="element">The element to move.</param>
        /// <param name="currentHolder">The <see cref="CommandBarRowElement"/> that contains the element to move.</param>
        public void MoveToUpperLine(CommandBarStripElement element, CommandBarRowElement currentHolder)
        {
            Debug.Assert(currentHolder.Strips.Contains(element), "Current holder must contains element");

            int index = this.Rows.IndexOf(currentHolder);
            Debug.Assert(index > -1, "Rows must contain currentHolder");

            int prevHolderPanelIndex = index - 1;
            if (prevHolderPanelIndex < 0 && currentHolder.Strips.Count == 1)
            {
                CreateFloatingStrip(element, currentHolder, Control.MousePosition);
                return;
            }

            if (prevHolderPanelIndex < 0)
            {
                CommandBarRowElement newPanel = RadCommandBarToolstripsHolderFactory.CreateLayoutPanel(this);
                this.Rows.Insert(0, newPanel);
                prevHolderPanelIndex = 0;
            }

            bool capture = element.Grip.Capture;
            currentHolder.Strips.Remove(element);
            CommandBarRowElement panelToAdd = this.Rows[prevHolderPanelIndex];
            Debug.Assert(panelToAdd != null, "Panel to add cannot be null");
            panelToAdd.Strips.Add(element);
            if (currentHolder.Strips.Count == 0)
            {
                this.Rows.Remove(currentHolder);
            }
            // this.ElementTree.Control.FindForm();
            element.Grip.Capture = capture;
        }

        /// <summary>
        /// Moves a specific <see cref="CommandBarStripElement"/> to the lower <see cref="CommandBarRowElement"/>.
        /// </summary>
        /// <param name="element">The element to move.</param>
        /// <param name="currentHolder">The <see cref="CommandBarRowElement"/> that contains the element to move.</param>
        public void MoveToDownerLine(CommandBarStripElement element, CommandBarRowElement currentHolder)
        {
            Debug.Assert(currentHolder.Strips.Contains(element), "Current holder must contains element");

            int index = this.Rows.IndexOf(currentHolder);
            Debug.Assert(index > -1, "Lines must contains currentHolder");

            int nextHolderPanelIndex = index + 1;
            if (nextHolderPanelIndex >= this.Rows.Count && currentHolder.Strips.Count == 1)
            {
                CreateFloatingStrip(element, currentHolder, Control.MousePosition);
                return;
            }

            if (nextHolderPanelIndex >= this.Rows.Count)
            {
                CommandBarRowElement newPanel = RadCommandBarToolstripsHolderFactory.CreateLayoutPanel(this);
                this.Rows.Add(newPanel);
                nextHolderPanelIndex = this.Rows.Count - 1;
            }


            bool capture = element.Grip.Capture;
            currentHolder.Strips.Remove(element);
            CommandBarRowElement panelToAdd = this.Rows[nextHolderPanelIndex];
            Debug.Assert(panelToAdd != null, "Panel to add cannot be null");
            panelToAdd.Strips.Add(element);

            if (currentHolder.Strips.Count == 0)
            {
                this.Rows.Remove(currentHolder);
            }



            element.Grip.Capture = capture;
        }

        /// <summary>
        /// Saves the visual state of the <see cref="RadCommandBarElement"/> to a specified file.
        /// </summary>
        /// <param name="filename">The name of the destination file.</param>
        public void SaveLayout(string filename)
        {
            XmlDocument doc = this.SaveLayoutCore();
            doc.Save(filename);
        }

        /// <summary>
        /// Saves the visual state of the <see cref="RadCommandBarElement"/> to a specified stream.
        /// </summary>
        /// <param name="destination">The destination stream.</param>
        public void SaveLayout(Stream destination)
        {
            XmlDocument doc = this.SaveLayoutCore();
            doc.Save(destination);
        }

        /// <summary>
        /// Saves the visual state of the <see cref="RadCommandBarElement"/> to a specified XmlWriter.
        /// </summary>
        /// <param name="writer">The XmlWriter to save the visual state data.</param>
        public void SaveLayout(XmlWriter writer)
        {
            XmlDocument doc = this.SaveLayoutCore();
            doc.Save(writer);
        }


        /// <summary>
        /// Loads the visual state of the <see cref="RadCommandBarElement"/> from a specified file.
        /// </summary>
        /// <param name="filename">The name of the file containing the visual state data.</param>
        public void LoadLayout(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(filename));
            this.LoadLayoutCore(doc);
        }

        /// <summary>
        /// Loads the visual state of the <see cref="RadCommandBarElement"/> from a specified stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        public void LoadLayout(Stream source)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(source);
            this.LoadLayoutCore(doc);
        }

        /// <summary>
        /// Loads the visual state of the <see cref="RadCommandBarElement"/> from a specified XmlReader.
        /// </summary>
        /// <param name="xmlReader">The XmlReader to read the visual state data.</param>
        public void LoadLayout(XmlReader xmlReader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlReader);
            this.LoadLayoutCore(doc);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a floating form of a specified <c ref="CommandBarStripElement"/>.
        /// </summary>
        /// <param name="stripElement">The strip element of which the floating form should be created.</param>
        /// <param name="currentRow">The <c ref="CommandBarRowElement"/> that contains the strip element.</param>
        /// <param name="initialLocation">The initial location of the floating form.</param>
        public void CreateFloatingStrip(CommandBarStripElement stripElement, CommandBarRowElement currentRow, Point initialLocation)
        {
            if (!stripElement.EnableFloating || this.OnFloatingStripCreating(stripElement))
            {
                return;
            }

            CommandBarFloatingForm floatingForm = new CommandBarFloatingForm();
            floatingForm.ParentControl = this.ElementTree.Control.Parent;
            Point initialDragOffset = Point.Empty;

            if (!this.RightToLeft)
            {
                floatingForm.RightToLeft = System.Windows.Forms.RightToLeft.No;
                floatingForm.Location = Point.Add(initialLocation, new Size(-5, -5));
                initialDragOffset = new Point(5, 5);
            }
            else
            {
                floatingForm.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                floatingForm.Location = Point.Add(Point.Add(initialLocation, new Size(-stripElement.Size.Width, 0)), new Size(5, -5));
                initialDragOffset = new Point(stripElement.Size.Width - 5, 5);
            }

            floatingForm.Text = stripElement.Name;
            stripElement.EnableDragging = false;
            stripElement.Capture = false;
            stripElement.ForceEndDrag();

            Size screenSize = Screen.GetWorkingArea(this.ElementTree.Control).Size;

            stripElement.Orientation = System.Windows.Forms.Orientation.Horizontal;
            floatingForm.ClientSize = (stripElement.GetExpectedSize(screenSize).ToSize());

            stripElement.FloatingForm = floatingForm;
            floatingForm.StripElement = stripElement;

            if (currentRow != null)
            {
                currentRow.Strips.Remove(stripElement);
            }

            floatingForm.StripInfoHolder.AddStripInfo(stripElement);
            floatingForm.Show();

            this.OnFloatingStripCreated(stripElement);

            floatingForm.InitializeMove(initialDragOffset);
            Cursor.Current = Cursors.SizeAll;

            if (currentRow != null && currentRow.Strips.Count == 0)
            {
                this.Rows.Remove(currentRow);
            }
        }

        protected internal void SetOrientationCore(Orientation newOrientation)
        {
            if (newOrientation == Orientation.Vertical)
            {
                this.layoutPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
                this.layoutPanel.StretchHorizontally = true;
                this.layoutPanel.StretchVertically = false;

            }
            else if (newOrientation == Orientation.Horizontal)
            {
                this.layoutPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
                this.layoutPanel.StretchHorizontally = false;
                this.layoutPanel.StretchVertically = true;
            }

            foreach (CommandBarRowElement line in lines)
            {
                line.SetOrientationCore(newOrientation);
            }

            base.Orientation = newOrientation;
        }

        protected virtual void WireEvents()
        {
            this.lines.ItemsChanged += ItemsChanged;
        }

        protected virtual void UnwireEvents()
        {
            this.lines.ItemsChanged -= ItemsChanged;
        }

        protected virtual void ItemsChanged(RadCommandBarLinesElementCollection changed, CommandBarRowElement target, ItemsChangeOperation operation)
        {
            if (operation != ItemsChangeOperation.Inserted)
            {
                return;
            }

            CommandBarRowElement panel = target as CommandBarRowElement;
            if (panel == null)
            {
                return;
            }

            panel.Owner = this;
            panel.Orientation = this.Orientation;
        }

        /// <summary>
        /// Creates an XmlDocument containing the current visual state data of the <see cref="RadCommandBarElement"/>.
        /// </summary>
        /// <returns>The created document.</returns>
        protected virtual XmlDocument SaveLayoutCore()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);
            XmlElement rootNode = doc.CreateElement("RadCommandBarElement");
            rootNode.SetAttribute("Orientation", this.Orientation.ToString());
            rootNode.SetAttribute("Name", this.Name);
            rootNode.SetAttribute("RTL", this.RightToLeft.ToString());

            for (int i = 0; i < this.Rows.Count; i++)
            {
                CommandBarRowElement lineElement = this.Rows[i];
                XmlElement lineNode = doc.CreateElement("CommandBarRowElement");
                lineNode.SetAttribute("Orientation", lineElement.Orientation.ToString());
                lineNode.SetAttribute("Name", lineElement.Name);
                lineNode.SetAttribute("LineIndex", i.ToString());

                foreach (CommandBarStripElement stripElement in lineElement.Strips)
                {
                    XmlElement stripNode = doc.CreateElement("CommandBarStripElement");
                    stripNode.SetAttribute("Orientation", stripElement.Orientation.ToString());
                    stripNode.SetAttribute("Name", stripElement.Name);
                    stripNode.SetAttribute("DesiredLocationX", stripElement.DesiredLocation.X.ToString());
                    stripNode.SetAttribute("DesiredLocationY", stripElement.DesiredLocation.Y.ToString());
                    stripNode.SetAttribute("VisibleInCommandBar", stripElement.VisibleInCommandBar.ToString());
                    stripNode.SetAttribute("StretchHorizontally", stripElement.StretchHorizontally.ToString());
                    stripNode.SetAttribute("StretchVertically", stripElement.StretchVertically.ToString());
                    stripNode.SetAttribute("EnableFloating", stripElement.EnableFloating.ToString());
                    stripNode.SetAttribute("EnableDragging", stripElement.EnableDragging.ToString());

                    int currentIndex = 0;
                    for (int j = 0; j < stripElement.Items.Count; j++)
                    {
                        RadCommandBarBaseItem itemElement = stripElement.Items[j];
                        XmlElement itemNode = doc.CreateElement("RadCommandBarBaseItem");
                        itemNode.SetAttribute("Orientation", itemElement.Orientation.ToString());
                        itemNode.SetAttribute("Name", itemElement.Name);
                        itemNode.SetAttribute("VisibleInStrip", itemElement.VisibleInStrip.ToString());
                        itemNode.SetAttribute("StretchHorizontally", itemElement.StretchHorizontally.ToString());
                        itemNode.SetAttribute("StretchVertically", itemElement.StretchVertically.ToString());
                        itemNode.SetAttribute("Index", currentIndex.ToString());
                        stripNode.AppendChild(itemNode);
                        ++currentIndex;
                    }

                    for (int j = 0; j < stripElement.OverflowButton.OverflowPanel.Layout.Children.Count; j++)
                    {
                        RadCommandBarBaseItem itemElement = stripElement.OverflowButton.OverflowPanel.Layout.Children[j] as RadCommandBarBaseItem;
                        if (itemElement == null)
                        {
                            continue;
                        }

                        XmlElement itemNode = doc.CreateElement("RadCommandBarBaseItem");
                        itemNode.SetAttribute("Orientation", itemElement.Orientation.ToString());
                        itemNode.SetAttribute("Name", itemElement.Name);
                        itemNode.SetAttribute("VisibleInStrip", itemElement.VisibleInStrip.ToString());
                        itemNode.SetAttribute("StretchHorizontally", itemElement.StretchHorizontally.ToString());
                        itemNode.SetAttribute("StretchVertically", itemElement.StretchVertically.ToString());
                        itemNode.SetAttribute("Index", currentIndex.ToString());
                        stripNode.AppendChild(itemNode);
                        ++currentIndex;
                    }


                    lineNode.AppendChild(stripNode);
                }

                rootNode.AppendChild(lineNode);
            }

            XmlElement floatingFormsNode = SaveFloatingStripsLayout(doc);

            rootNode.AppendChild(floatingFormsNode);


            doc.AppendChild(rootNode);



            return doc;

        }

        private XmlElement SaveFloatingStripsLayout(XmlDocument doc)
        {
            XmlElement floatingFormsNode = doc.CreateElement("FloatingStrips");
            foreach (CommandBarStripElement stripElement in this.stripInfoHolder.StripInfoList)
            {
                if (stripElement.FloatingForm == null || stripElement.FloatingForm.IsDisposed)
                {
                    continue;
                }

                XmlElement stripNode = doc.CreateElement("CommandBarStripElement");
                stripNode.SetAttribute("Orientation", stripElement.Orientation.ToString());
                stripNode.SetAttribute("Name", stripElement.Name);
                stripNode.SetAttribute("DesiredLocationX", stripElement.DesiredLocation.X.ToString());
                stripNode.SetAttribute("DesiredLocationY", stripElement.DesiredLocation.Y.ToString());
                stripNode.SetAttribute("FormLocationX", stripElement.FloatingForm.Location.X.ToString());
                stripNode.SetAttribute("FormLocationY", stripElement.FloatingForm.Location.Y.ToString());
                stripNode.SetAttribute("VisibleInCommandBar", stripElement.VisibleInCommandBar.ToString());
                stripNode.SetAttribute("StretchHorizontally", stripElement.StretchHorizontally.ToString());
                stripNode.SetAttribute("StretchVertically", stripElement.StretchVertically.ToString());
                stripNode.SetAttribute("RTL", stripElement.RightToLeft.ToString());
                stripNode.SetAttribute("EnableFloating", stripElement.EnableFloating.ToString());
                stripNode.SetAttribute("EnableDragging", stripElement.EnableDragging.ToString());

                int currentIndex = 0;
                for (int j = 0; j < stripElement.Items.Count; j++)
                {
                    RadCommandBarBaseItem itemElement = stripElement.Items[j];
                    XmlElement itemNode = doc.CreateElement("RadCommandBarBaseItem");
                    itemNode.SetAttribute("Orientation", itemElement.Orientation.ToString());
                    itemNode.SetAttribute("Name", itemElement.Name);
                    itemNode.SetAttribute("VisibleInStrip", itemElement.VisibleInStrip.ToString());
                    itemNode.SetAttribute("StretchHorizontally", itemElement.StretchHorizontally.ToString());
                    itemNode.SetAttribute("StretchVertically", itemElement.StretchVertically.ToString());
                    itemNode.SetAttribute("Index", currentIndex.ToString());
                    stripNode.AppendChild(itemNode);
                    ++currentIndex;
                }

                for (int j = 0; j < stripElement.FloatingForm.ItemsHostControl.Element.Layout.Children.Count; j++)
                {
                    RadCommandBarBaseItem itemElement = stripElement.FloatingForm.ItemsHostControl.Element.Layout.Children[j] as RadCommandBarBaseItem;
                    if (itemElement == null)
                    {
                        continue;
                    }

                    XmlElement itemNode = doc.CreateElement("RadCommandBarBaseItem");
                    itemNode.SetAttribute("Orientation", itemElement.Orientation.ToString());
                    itemNode.SetAttribute("Name", itemElement.Name);
                    itemNode.SetAttribute("VisibleInStrip", itemElement.VisibleInStrip.ToString());
                    itemNode.SetAttribute("StretchHorizontally", itemElement.StretchHorizontally.ToString());
                    itemNode.SetAttribute("StretchVertically", itemElement.StretchVertically.ToString());
                    itemNode.SetAttribute("Index", currentIndex.ToString());
                    stripNode.AppendChild(itemNode);
                    ++currentIndex;
                }

                floatingFormsNode.AppendChild(stripNode);
            }
            return floatingFormsNode;
        }

        /// <summary>
        /// Restores the visual state of the <see cref="RadCommandBarElement"/> from the specified XmlDocument.
        /// </summary>
        /// <param name="doc">The document containing the visual state data.</param>
        protected virtual void LoadLayoutCore(XmlDocument doc)
        {
            this.SuspendLayout(true);

            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name != "RadCommandBarElement" || rootNode.Attributes["Name"] == null || rootNode.Attributes["Name"].Value != this.Name)
                {
                    continue;
                }

                if (rootNode.Attributes["Orientation"] != null)
                {
                    this.Orientation = (rootNode.Attributes["Orientation"].Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal;
                }
                if (rootNode.Attributes["RTL"] != null)
                {
                    this.RightToLeft = (rootNode.Attributes["RTL"].Value == "True");
                }

                int maxLineIndex = 0;
                foreach (XmlNode lineNode in rootNode.ChildNodes)
                {
                    int currentLineIndex = 0;
                    if (lineNode.Name == "CommandBarRowElement" && lineNode.Attributes["LineIndex"] != null
                        && int.TryParse(lineNode.Attributes["LineIndex"].Value, out currentLineIndex))
                    {
                        maxLineIndex = Math.Max(maxLineIndex, currentLineIndex);
                    }
                }

                while (maxLineIndex + 1 > this.lines.Count)
                {
                    this.lines.Add(new CommandBarRowElement());
                }

                foreach (XmlNode lineNode in rootNode.ChildNodes)
                {
                    if (lineNode.Name == "CommandBarRowElement")
                    {
                        LoadStripsLayout(lineNode, maxLineIndex);
                    }
                    else if (lineNode.Name == "FloatingStrips")
                    {
                        LoadFloatingStripsLayout(lineNode);
                    }
                }

            }

            this.RemoveUnusedLines();
            this.ResumeLayout(true, true);
        }

        private void LoadStripsLayout(XmlNode lineNode, int maxLineIndex)
        {
            int currentLineIndex = 0;
            int.TryParse(lineNode.Attributes["LineIndex"].Value, out currentLineIndex);
            if (currentLineIndex < 0 || currentLineIndex > maxLineIndex)
            {
                currentLineIndex = 0;
            }

            foreach (XmlNode stripNode in lineNode.ChildNodes)
            {
                if (stripNode.Name != "CommandBarStripElement")
                {
                    continue;
                }

                CommandBarStripElement strip = this.GetStripByName(stripNode.Attributes["Name"].Value);
                if (strip == null)
                {
                    continue;
                }

                if (strip.FloatingForm != null && !strip.FloatingForm.IsDisposed)
                {
                    strip.FloatingForm.TryDocking(this.ElementTree.Control as RadCommandBar);
                }

                if (strip.FloatingForm != null && !strip.FloatingForm.IsDisposed)
                {
                    continue;
                }
                strip.SuspendLayout(true);
                strip.EnableFloating = false;

                for (int attr = 0; attr < stripNode.Attributes.Count; attr++)
                {
                    switch (stripNode.Attributes[attr].Name)
                    {
                        case "Orientation":
                            strip.Orientation = (stripNode.Attributes["Orientation"].Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal; break;
                        case "VisibleInCommandBar":
                            strip.VisibleInCommandBar = (stripNode.Attributes["VisibleInCommandBar"].Value == "True"); break;
                        case "StretchHorizontally":
                            strip.StretchHorizontally = (stripNode.Attributes["StretchHorizontally"].Value == "True"); break;
                        case "StretchVertically":
                            strip.StretchVertically = (stripNode.Attributes["StretchVertically"].Value == "True"); break;
                        case "EnableFloating":
                            strip.EnableFloating = (stripNode.Attributes["EnableFloating"].Value == "True"); break;
                        case "EnableDragging":
                            strip.EnableDragging = (stripNode.Attributes["EnableDragging"].Value == "True"); break;
                    }
                }

                if (stripNode.Attributes["DesiredLocationX"] != null && stripNode.Attributes["DesiredLocationY"] != null)
                {
                    strip.DesiredLocation = new PointF(float.Parse(stripNode.Attributes["DesiredLocationX"].Value), float.Parse(stripNode.Attributes["DesiredLocationY"].Value));
                }

                CommandBarRowElement parentRow = (strip.Parent as CommandBarRowElement);
                if (parentRow != null)
                {
                    parentRow.Strips.Remove(strip);
                }

                this.lines[currentLineIndex].Strips.Add(strip);

                foreach (XmlNode itemNode in stripNode.ChildNodes)
                {
                    if (itemNode.Name != "RadCommandBarBaseItem")
                    {
                        continue;
                    }

                    RadCommandBarBaseItem item = this.GetItemByName(itemNode.Attributes["Name"].Value);
                    if (item == null)
                    {
                        continue;
                    }
                    for (int attr = 0; attr < itemNode.Attributes.Count; attr++)
                    {
                        switch (itemNode.Attributes[attr].Name)
                        {
                            case "Orientation":
                                item.Orientation = (itemNode.Attributes["Orientation"].Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal;
                                break;
                            case "VisibleInStrip":
                                item.VisibleInStrip = (itemNode.Attributes["VisibleInStrip"].Value == "True");
                                break;
                            case "StretchHorizontally":
                                item.StretchHorizontally = (itemNode.Attributes["StretchHorizontally"].Value == "True");
                                break;
                            case "StretchVertically":
                                item.StretchVertically = (itemNode.Attributes["StretchVertically"].Value == "True");
                                break;
                            case "Index":
                                int index = int.Parse(itemNode.Attributes["Index"].Value);
                                item.Parent.Children.Remove(item);
                                strip.Items.Remove(item);
                                strip.Items.Insert(index, item);
                                break;
                        }
                    }

                }

                strip.ResumeLayout(true, true);
                strip.EnableFloating = true;

            }
        }

        private void LoadFloatingStripsLayout(XmlNode floatingStripsNode)
        {
            foreach (XmlNode stripNode in floatingStripsNode.ChildNodes)
            {
                if (stripNode.Name != "CommandBarStripElement")
                {
                    continue;
                }

                CommandBarStripElement strip = this.GetStripByName(stripNode.Attributes["Name"].Value);
                if (strip == null)
                {
                    continue;
                }

                strip.SuspendLayout(true);

                bool oldRtl = this.RightToLeft;
                this.RightToLeft = false;
                if (stripNode.Attributes["FormLocationX"] != null && stripNode.Attributes["FormLocationY"] != null)
                {
                    Point formLocation = new Point(int.Parse(stripNode.Attributes["FormLocationX"].Value), int.Parse(stripNode.Attributes["FormLocationY"].Value));

                    if (strip.FloatingForm != null && !strip.FloatingForm.IsDisposed)
                    {
                        strip.FloatingForm.Location = formLocation;
                    }
                    else
                    {
                        this.CreateFloatingStrip(strip, strip.Parent as CommandBarRowElement, formLocation);
                    }
                }
                this.RightToLeft = oldRtl;
                strip.FloatingForm.EndMove();
                for (int attr = 0; attr < stripNode.Attributes.Count; attr++)
                {
                    switch (stripNode.Attributes[attr].Name)
                    {
                        case "Orientation":
                            strip.Orientation = (stripNode.Attributes["Orientation"].Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal; break;
                        case "VisibleInCommandBar":
                            strip.VisibleInCommandBar = (stripNode.Attributes["VisibleInCommandBar"].Value == "True"); break;
                        case "StretchHorizontally":
                            strip.StretchHorizontally = (stripNode.Attributes["StretchHorizontally"].Value == "True"); break;
                        case "StretchVertically":
                            strip.StretchVertically = (stripNode.Attributes["StretchVertically"].Value == "True"); break;
                        case "EnableFloating":
                            strip.EnableFloating = (stripNode.Attributes["EnableFloating"].Value == "True"); break;
                        case "EnableDragging":
                            strip.EnableDragging = (stripNode.Attributes["EnableDragging"].Value == "True"); break;
                    }
                }

                if (stripNode.Attributes["DesiredLocationX"] != null && stripNode.Attributes["DesiredLocationY"] != null)
                {
                    strip.DesiredLocation = new PointF(float.Parse(stripNode.Attributes["DesiredLocationX"].Value), float.Parse(stripNode.Attributes["DesiredLocationY"].Value));
                }

                foreach (XmlNode itemNode in stripNode.ChildNodes)
                {
                    if (itemNode.Name != "RadCommandBarBaseItem")
                    {
                        continue;
                    }

                    RadCommandBarBaseItem item = this.GetItemByName(itemNode.Attributes["Name"].Value);
                    if (item == null)
                    {
                        continue;
                    }
                    
                    for (int attr = 0; attr < itemNode.Attributes.Count; attr++)
                    {
                        switch (itemNode.Attributes[attr].Name)
                        {
                            case "Orientation":
                                item.Orientation = (itemNode.Attributes["Orientation"].Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal;
                                break;
                            case "VisibleInStrip":
                                item.VisibleInStrip = (itemNode.Attributes["VisibleInStrip"].Value == "True");
                                break;
                            case "StretchHorizontally":
                                item.StretchHorizontally = (itemNode.Attributes["StretchHorizontally"].Value == "True");
                                break;
                            case "StretchVertically":
                                item.StretchVertically = (itemNode.Attributes["StretchVertically"].Value == "True");
                                break;
                            case "Index":
                                int index = int.Parse(itemNode.Attributes["Index"].Value);
                                item.Parent.Children.Remove(item);
                                strip.Items.Remove(item);
                                if (strip.FloatingForm != null && !strip.FloatingForm.IsDisposed)
                                {
                                    strip.FloatingForm.ItemsHostControl.Element.Layout.Children.Insert(index, item);
                                }
                                else
                                {
                                    strip.Items.Insert(index, item);
                                }
                                break;
                        }
                    }
                     
                }
                 
                strip.ResumeLayout(true, true);
            }
        }

        private void RemoveUnusedLines()
        {
            List<CommandBarRowElement> linesToDelete = new List<CommandBarRowElement>();
            foreach (CommandBarRowElement line in this.lines)
            {
                if (line.Children.Count == 0)
                {
                    linesToDelete.Add(line);
                }
            }

            foreach (CommandBarRowElement line in linesToDelete)
            {
                this.lines.Remove(line);
            }
        }

        private CommandBarStripElement GetStripByName(string name)
        {
            foreach (CommandBarStripElement strip in this.stripInfoHolder.StripInfoList)
            {
                if (strip.Name == name)
                {
                    return strip;
                }
            }

            return null;
        }

        private RadCommandBarBaseItem GetItemByName(string name)
        {
            foreach (CommandBarStripElement strip in this.stripInfoHolder.StripInfoList)
            {
                foreach (RadCommandBarBaseItem item in strip.Items)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                foreach (RadElement element in strip.OverflowButton.ItemsLayout.Children)
                {
                    if ((element is RadCommandBarBaseItem) && (element as RadCommandBarBaseItem).Name == name)
                    {
                        return (element as RadCommandBarBaseItem);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="CommandBarStripInfoHolder"/> object that provides information about strips owned by the <see cref="RadCommandBarElement"/>.
        /// </summary>
        public CommandBarStripInfoHolder StripInfoHolder
        {
            get
            {
                return stripInfoHolder;
            }
            set
            {
                this.stripInfoHolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the size in pixels when current strip is being Drag and Drop in next or previous row
        /// </summary>
        [Description("Gets or sets the size in pixels when current strip is being Drag and Drop in next or previous row")]
        [DefaultValue(typeof(Size), "4,4")]
        public Size DragSize
        {
            get
            {
                return this.dragSize;
            }
            set
            {
                this.dragSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the <see cref="RadCommandBarElement"/>.
        /// </summary>
        [Description("Gets or sets the orientation of the RadCommandBarElement")]
        [DefaultValue(Orientation.Horizontal)]
        [Browsable(false)]
        public override Orientation Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                if (this.orientation != value && !this.OnOrientationChanging(new CancelEventArgs()))
                {
                    this.SetOrientationCore(value);
                    this.OnOrientationChanged(new EventArgs());
                }
            }
        }

        #endregion

        #region IItemsOwner Members

        /// <summary>
        /// Gets the rows of the <see cref="RadCommandBarElement"/>.
        /// </summary>
        [RadEditItemsAction]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [RadNewItem("", false, true, true)]
        public RadCommandBarLinesElementCollection Rows
        {
            get
            {
                return this.lines;
            }
        }

        #endregion
    }
}
