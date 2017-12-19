using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents a tool strip grip element. It is responsible for dragging the 
	/// toolstrip items by providing the graphical and logical functionality.
	/// 
	/// </summary>
	public class RadToolStripGripElement : RadItem
	{
		private GripPrimitive gripPrimitive;
		private Rectangle ClientRectangle;

		private RadToolStripItem parentToolStripItem;
		private RadToolStripManager parentToolStripManager;
		private RadToolStripElement parentToolStripElement;

		private Point movePos;
		private Point lastPos;

        
		internal FloatingForm form;

		private bool fromFloationg;

		private int renderingFramerate;

		private bool isDragging;

		public static RoutedEvent DragStartingEvent =
			RegisterRoutedEvent("DragStartingEvent", typeof(RadToolStripGripElement));

		public static RoutedEvent DragStartedEvent =
			RegisterRoutedEvent("DragStartedEvent", typeof(RadToolStripGripElement));

		public static RoutedEvent DragEndingEvent =
			RegisterRoutedEvent("DragEndingEvent", typeof(RadToolStripGripElement));

		public static RoutedEvent DragEndedEvent =
			RegisterRoutedEvent("DragEndedEvent", typeof(RadToolStripGripElement));

		public static RoutedEvent RowChangedEvent =
			RegisterRoutedEvent("RowChangedEvent", typeof(RadToolStripGripElement));

		private Timer dragDropTimer;

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.Class = "ToolStripGrip";
            this.dragDropTimer = new Timer();
            this.dragDropTimer.Tick += new EventHandler(dragDropTimer_Tick);
        }

        protected override void DisposeManagedResources()
        {
            if (this.dragDropTimer != null)
            {
                this.dragDropTimer.Tick -= new EventHandler(dragDropTimer_Tick);
                this.dragDropTimer.Dispose();
            }

            if (this.parentToolStripManager != null)
            {
                this.parentToolStripManager.RadPropertyChanged -= new RadPropertyChangedEventHandler(value_RadPropertyChanged);
            }

            base.DisposeManagedResources();
        }

		internal bool FromFloating
		{
			get
			{
				return this.fromFloationg;
			}
			set
			{
				this.fromFloationg = value;
			}
		}

		internal RadToolStripElement ParentToolStripElement
		{
			get
			{
				if (this.parentToolStripElement == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripElement == null; res = res.Parent)
					{
						this.parentToolStripElement = res as RadToolStripElement;
					}
				}
				return this.parentToolStripElement;
			}
			set
			{
				this.parentToolStripElement = value;
			}
		}


		internal RadToolStripManager ParentToolStripManager
		{
			get
			{
				if (this.parentToolStripManager == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripManager == null; res = res.Parent)
					{
						this.parentToolStripManager = res as RadToolStripManager;
					}
				}
				return this.parentToolStripManager;
			}
			set
			{
                if (this.parentToolStripManager == value)
                {
                    return;
                }

                if (this.parentToolStripManager != null)
                {
                    this.parentToolStripManager.RadPropertyChanged -= value_RadPropertyChanged;
                }

                this.parentToolStripManager = value;

                if (this.parentToolStripManager != null)
                {
                    this.parentToolStripManager.RadPropertyChanged += value_RadPropertyChanged;
                }
			}
		}

		internal RadToolStripItem ParentToolStripItem
		{
			get
			{
				if (this.parentToolStripItem == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripItem == null; res = res.Parent)
					{
						this.parentToolStripItem = res as RadToolStripItem;
					}
				}
				return this.parentToolStripItem;
			}
			set
			{
				this.parentToolStripItem = value;
			}
		}

		private void value_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{

		}

		private Stack<RadToolStripItem> GetItemsAfterCurrent(RadToolStripItem currentItem)
		{
			Stack<RadToolStripItem> itemsAfterCurrent = new Stack<RadToolStripItem>();

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (this.ParentToolStripManager.Orientation == Orientation.Horizontal)
				{
					if (currentItem.FullBoundingRectangle.Left < item.FullBoundingRectangle.Left)
					{
						itemsAfterCurrent.Push(item);
					}
				}
				else
				{
					if (currentItem.FullBoundingRectangle.Top < item.FullBoundingRectangle.Top)
					{
						itemsAfterCurrent.Push(item);
					}

				}
			}

			return itemsAfterCurrent;
		}

		private bool HasAnyItemBeforeCurrentWithLeftMargin()
		{
			int margin = 0;
			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (this.ParentToolStripItem.FullBoundingRectangle.Left > item.FullBoundingRectangle.Left)
					margin += item.Margin.Left;
			}

			if (margin > 0)
				return true;

			return false;

		}

		private int FindPreferredIndexToInsert(RadToolStripElement element)
		{
			Point pt = this.ParentToolStripManager.PointFromScreen(Cursor.Position);
			if (this.ParentToolStripManager.Orientation == Orientation.Horizontal)
			{
				Rectangle boundingRectangle = new Rectangle(0, element.Bounds.Top, 0, element.Bounds.Height + 5);
				Rectangle boundingRectangle2 = new Rectangle(0, element.Bounds.Top, 0, element.Bounds.Height + 5);

				for (int i = 0; i < element.Items.Count; i++)
				{
					boundingRectangle = new Rectangle(element.Items[i].FullBoundingRectangle.Right + 35 - element.Items[i].Bounds.Width, boundingRectangle.Top,
						element.Items[i].Bounds.Width - 35, boundingRectangle.Height);

					boundingRectangle2 = new Rectangle(element.Items[i].Bounds.Left, boundingRectangle.Top,
						15 + element.Items[i].Margin.Left, boundingRectangle.Height);

					if (boundingRectangle2.Contains(pt))
						return i;

					if (boundingRectangle.Contains(pt))
					{
						if ( i + 1 < element.Items.Count )
							return i + 1;

						return element.Items.Count;
					}
				}

				return element.Items.Count;
			}
			else
			{
			    for (int i = 0; i < element.Items.Count; i++)
				{
				    Rectangle boundingRectangle;
				    boundingRectangle = new Rectangle(element.Bounds.Left, element.Items[i].FullBoundingRectangle.Bottom + 35 - element.Items[i].Bounds.Height,
						element.Bounds.Width, -35 + element.Items[i].Bounds.Height);

				    Rectangle boundingRectangle2;
				    boundingRectangle2 = new Rectangle(element.Bounds.Left, element.Items[i].Bounds.Top,
						element.Bounds.Width, 15 + element.Items[i].Margin.Top);

					
					if (boundingRectangle2.Contains(pt))
						return i;

					if (boundingRectangle.Contains(pt))
						return i + 1;
				}

				return element.Items.Count; 	
			}
		}


		private RadToolStripItem GetOuterMostItem(Direction direction)
		{
            RadToolStripItem outerMostItem = null;

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
                if (item.Visibility != ElementVisibility.Visible)
                {
                    continue;
                }

                if (outerMostItem == null)
                {
                    outerMostItem = item;
                    continue;
                }

                if (direction == Direction.Right)
                {
                    if (outerMostItem.FullBoundingRectangle.Left <= item.FullBoundingRectangle.Left)
                    {
                        outerMostItem = item;
                    }
                }
                else if (direction == Direction.Down)
                {
                    if (outerMostItem.FullBoundingRectangle.Top <= item.FullBoundingRectangle.Top)
                    {
                        outerMostItem = item;
                    }
                }
			}

            return outerMostItem;
		}

		private Size GetRowsTotalSize()
		{
			int heightToReturn = 0;
			int widthToReturn = 0;
			Size sz;

			if (this.ParentToolStripManager != null)
			{
				foreach (RadToolStripElement element in this.ParentToolStripManager.Items)
				{
					heightToReturn += element.Bounds.Height;
					widthToReturn += element.Bounds.Width;
				}
			}

			sz = new Size(widthToReturn, heightToReturn);

			return sz;
		}

		private int NotCollapsedItemsCount(RadToolStripElement element)
		{
			int i = 0;

			foreach (RadToolStripItem item in element.Items)
			{
				if (item.Visibility == ElementVisibility.Visible)
					i++;
			}

			return i;
		}

		private int lastX;
		private int lastY;

		private RadToolStripElement GetVerticalElementFromLocation(Point pt, int totalHeight, int elementIndex)
		{
			Rectangle newRowBoundingRect = new Rectangle(this.ParentToolStripElement.Bounds.Left,
				this.ParentToolStripElement.Bounds.Top, this.ParentToolStripElement.Bounds.Width / 8,
			this.ParentToolStripElement.Bounds.Height);

			if (this.fromFloationg)
			{
				newRowBoundingRect = new Rectangle(this.ParentToolStripElement.Bounds.Left - 25,
				   this.ParentToolStripElement.Bounds.Top, 28,
			   this.ParentToolStripElement.Bounds.Height);
				this.fromFloationg = false;
			}

			if (newRowBoundingRect.Contains(pt))
			{
				RadToolStripElement element = new RadToolStripElement();
				element.Orientation = Orientation.Vertical;
				if (NotCollapsedItemsCount(this.ParentToolStripElement) > 0)
				{
					{
						if (lastX != pt.X)
						{
							this.lastX = pt.X;
							this.lastY = pt.Y;

							this.ParentToolStripManager.Items.Insert(elementIndex, element);
							return element;
						}
					}
				}
			}
			else
				if ((elementIndex == this.ParentToolStripManager.Items.Count - 1) && (pt.X > this.ParentToolStripElement.Bounds.Right))
				{
					RadToolStripElement element = new RadToolStripElement();
					element.Orientation = Orientation.Vertical;
					if (NotCollapsedItemsCount(this.ParentToolStripElement) > 0)
					{
						{
							if (lastX != pt.X)
							{
								this.lastX = pt.X;
								this.lastY = pt.Y;

								this.ParentToolStripManager.Items.Add(element);
								return element;
							}
						}
					}
				}
			return null;
		}

		private RadToolStripElement GetHorizontalElementFromLocation(Point pt, int totalHeight, int elementIndex)
		{
			Rectangle newRowBoundingRectTop = new Rectangle(this.ParentToolStripElement.Bounds.X, this.ParentToolStripElement.Bounds.Top, this.ParentToolStripElement.Bounds.Width,
					this.ParentToolStripElement.Bounds.Height / 8);
			//Checks whether a new row should be inserted
			if (newRowBoundingRectTop.Contains(pt))
			{
				RadToolStripElement element = new RadToolStripElement();
				if (NotCollapsedItemsCount(this.ParentToolStripElement) > 1)
				{
					totalHeight = GetRowsTotalSize().Height + this.ParentToolStripItem.Bounds.Height;
					if (this.parentToolStripManager.parentAutoSize)
					{
						if (lastY != pt.Y)
						{
							this.lastX = pt.X;
							this.lastY = pt.Y;

							this.ParentToolStripManager.Items.Insert(elementIndex, element);
							return element;

						}
					}
					else
						if (totalHeight < this.ParentToolStripManager.Bounds.Height)
						{
							if (lastY != pt.Y)
							{
								this.lastX = pt.X;
								this.lastY = pt.Y;

								this.ParentToolStripManager.Items.Insert(elementIndex, element);
								return element;
							}
						}
				}

			}
			else // checks whether a new row will be the last row 
				if ((elementIndex == this.ParentToolStripManager.Items.Count - 1) && (pt.Y > this.ParentToolStripElement.Bounds.Bottom))
				{
					RadToolStripElement element = new RadToolStripElement();
					if (NotCollapsedItemsCount(this.ParentToolStripElement) > 1)
					{
						totalHeight = GetRowsTotalSize().Height + this.ParentToolStripItem.Bounds.Height;

						if (this.ParentToolStripManager.parentAutoSize)
						{
							if (lastY != pt.Y)
							{
								this.lastX = pt.X;
								this.lastY = pt.Y;

								this.ParentToolStripManager.Items.Add(element);
								return element;
							}
						}
						else
						{
							if (totalHeight < this.ParentToolStripManager.Bounds.Height)
							{
								if (lastY != pt.Y)
								{
									this.lastX = pt.X;
									this.lastY = pt.Y;

									this.ParentToolStripManager.Items.Add(element);
									return element;
								}
							}
						}
					}
				}
			return null;
		}

		private RadToolStripElement GetElementFromLocation(Point localPoint)
		{
			Point pt = this.ParentToolStripManager.PointFromScreen(Cursor.Position);
			if (this.lastX == 0) this.lastX = pt.X;
			if (this.lastY == 0) this.lastY = pt.Y;

			if ((lastY == pt.Y) && (this.ParentToolStripManager.Orientation == Orientation.Horizontal))
			{
				return null;
			}

			if ((lastX == pt.X) && (this.ParentToolStripManager.Orientation == Orientation.Vertical))
			{
				return null;
			}

			int elementIndex = this.ParentToolStripManager.Items.IndexOf(this.ParentToolStripElement);
			int totalHeight = -1;

			if (this.ParentToolStripManager.Orientation == Orientation.Horizontal)
			{
				RadToolStripElement element = GetHorizontalElementFromLocation(pt, totalHeight, elementIndex);
				if (element != null)
					return element;
			}
			else // Vertical Rows 
			{
				RadToolStripElement element = GetVerticalElementFromLocation(pt, totalHeight, elementIndex);
				if (element != null)
					return element;

			}
			// returns an available RadToolStripElement from the current items collection
			foreach (RadToolStripElement element in this.ParentToolStripManager.Items)
			{
				if ( element != this.ParentToolStripElement )
				if (element.FullBoundingRectangle.Contains(pt))
				{
					return element;

				}
			}
			return null;
		}

		private void ResetVerticalAffectedRow(RadToolStripElement element)
		{
            this.ParentToolStripElement.Items.Remove(this.ParentToolStripItem);

			int preferredIndexToInsert = this.FindPreferredIndexToInsert(element);
			
			if  ( preferredIndexToInsert < 0 )
				preferredIndexToInsert = 0;

			element.Items.Insert(preferredIndexToInsert, this.ParentToolStripItem);

            ResetTopMargins(false);


            if (this.NotCollapsedItemsCount(this.ParentToolStripElement) == 0)
            {
                RadToolStripElement myElement = new RadToolStripElement();
                foreach (RadToolStripItem currentItem in this.ParentToolStripElement.Items)
                {
                    currentItem.ParentToolStripElement = myElement;
                    currentItem.Grip.ParentToolStripElement = myElement;
                    myElement.Items.Add(currentItem);
                }

                this.ParentToolStripManager.elementList.Add(myElement);

                this.ParentToolStripManager.Items.Remove(this.ParentToolStripElement);
            }
            // Resets all elements in the row where the toolStripItem is inserted


            this.InitializeVerticalNewItem(element);

		}

		private void ResetHorizontalAffectedRow(RadToolStripElement element)
		{
			int preferredIndexToInsert = FindPreferredIndexToInsert(element);
		
			this.ParentToolStripElement.Items.Remove(this.ParentToolStripItem);
		
			if (preferredIndexToInsert < 0) preferredIndexToInsert = 0;
			element.Items.Insert(preferredIndexToInsert, this.ParentToolStripItem);
			
			ResetLeftMargins(false);


			if (this.NotCollapsedItemsCount(this.ParentToolStripElement) == 0)
			{
				RadToolStripElement myElement = new RadToolStripElement();
				foreach (RadToolStripItem currentItem in this.ParentToolStripElement.Items)
				{
					currentItem.ParentToolStripElement = myElement;
					currentItem.Grip.ParentToolStripElement = myElement;
					myElement.Items.Add(currentItem);
				}

				this.ParentToolStripManager.elementList.Add(myElement);

				this.ParentToolStripManager.Items.Remove(this.ParentToolStripElement);
			}
			// Resets all elements in the row where the toolStripItem is inserted

			this.InitializeHorizontalNewItem(element);
		}

		private void InitializeVerticalNewItem(RadToolStripElement element)
		{
			// Resets all elements in the row where the toolStripItem is inserted
			element.Orientation = Orientation.Vertical;
			
			foreach (RadToolStripItem item in element.Items)
			{
				item.ParentToolStripManager.CallBoundsChanged();

				if (item != this.ParentToolStripItem)
				{
					item.Margin = new Padding(item.Margin.Left, 0,
					    item.Margin.Right, item.Margin.Bottom);
				}

				item.ParentToolStripElement = element;
				item.itemsLayout.ParentToolStripElement = element;
				item.itemsLayout.ParentToolStripItem = item;
				item.itemsLayout.ParentToolStripManager = this.ParentToolStripManager;
			}
			this.ParentToolStripItem.OverflowManager.ToolStripItem = this.ParentToolStripItem;
			this.ParentToolStripElement = element;
		}

		private void InitializeHorizontalNewItem(RadToolStripElement element)
		{
			foreach (RadToolStripItem item in element.Items)
			{
				item.ParentToolStripManager.CallBoundsChanged();

                if (item != this.ParentToolStripItem)
                {
                    item.Margin = new Padding(0, item.Margin.Top,
                        item.Margin.Right, item.Margin.Bottom);
                }

				item.ParentToolStripElement = element;
				item.itemsLayout.ParentToolStripElement = element;
				item.itemsLayout.ParentToolStripItem = item;
				item.itemsLayout.ParentToolStripManager = this.ParentToolStripManager;
		
				this.ParentToolStripElement = element;
			}
			this.ParentToolStripItem.OverflowManager.ToolStripItem = this.ParentToolStripItem;
			this.ParentToolStripElement = element;
		}

		private RadToolStripItem GetItemBeforeCurrentWithTopMargin(RadToolStripItem movingItem)
		{
			RadToolStripItem itemToReturn = null;
			int difference = 2000;

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (item != this.ParentToolStripItem)
				{
					if ((item.FullBoundingRectangle.Top < movingItem.FullBoundingRectangle.Top) && (item.Margin.Top > 1))
					{
						if (movingItem.FullBoundingRectangle.Top - item.FullBoundingRectangle.Top > 0)
							if (difference > movingItem.FullBoundingRectangle.Top - item.FullBoundingRectangle.Top)
							{
								difference = movingItem.FullBoundingRectangle.Top - item.FullBoundingRectangle.Top;
								itemToReturn = item;
							}

					}
				}
			}

			return itemToReturn;
		}

		private RadToolStripItem GetItemBeforeCurrentWithLeftMargin(RadToolStripItem movingItem)
		{
			RadToolStripItem itemToReturn = null;
			int difference = 2000;

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (item != movingItem)
				{
					if ((item.FullBoundingRectangle.Left < movingItem.FullBoundingRectangle.Left) && (item.Margin.Left > 0))
					{
						if (item.Visibility == ElementVisibility.Visible)
							if (movingItem.FullBoundingRectangle.Left - item.FullBoundingRectangle.Left > 0)
								if (difference > movingItem.FullBoundingRectangle.Left - item.FullBoundingRectangle.Left)
								{
									difference = movingItem.FullBoundingRectangle.Left - item.FullBoundingRectangle.Left;
									itemToReturn = item;
								}

					}
				}
			}

			return itemToReturn;
		}

		private RadToolStripItem GetItemAfterCurrentWithTopMargin(RadToolStripItem movingItem)
		{

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (item != this.ParentToolStripItem)
				{
					if ((item.FullBoundingRectangle.Top > movingItem.FullBoundingRectangle.Top) && (item.Margin.Top > 1))
					{
						return item;
					}
				}
			}

			return null;
		}

		private RadToolStripItem GetNearestItemAfterCurrentWithLeftMargin(RadToolStripItem movingItem)
		{
			int difference = 2000;
			RadToolStripItem itemToReturn = null;

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (item != movingItem)
				{
					if ((item.FullBoundingRectangle.Left - movingItem.FullBoundingRectangle.Left > 0) && (item.Margin.Left > 1))
						if (difference > item.FullBoundingRectangle.Left - movingItem.FullBoundingRectangle.Left)
						{
							itemToReturn = item;
							difference = item.FullBoundingRectangle.Left - movingItem.FullBoundingRectangle.Left;
						}

				}
			}

			return itemToReturn;
		}

		private RadToolStripItem GetNearestItemAfterCurrentWithTopMargin(RadToolStripItem movingItem)
		{
			int difference = 2000;
			RadToolStripItem itemToReturn = null;

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (item != movingItem)
				{
					if ((item.FullBoundingRectangle.Top - movingItem.FullBoundingRectangle.Top > 0) && (item.Margin.Top > 1))
						if (difference > item.FullBoundingRectangle.Top - movingItem.FullBoundingRectangle.Top)
						{
							itemToReturn = item;
							difference = item.FullBoundingRectangle.Top - movingItem.FullBoundingRectangle.Top;
						}

				}
			}

			return itemToReturn;
		}

		private RadToolStripItem GetItemAfterCurrentWithLeftMargin(RadToolStripItem movingItem)
		{

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (item != this.ParentToolStripItem)
				{
					if (item.Visibility == ElementVisibility.Visible)
						if ((item.FullBoundingRectangle.Left > movingItem.FullBoundingRectangle.Left) && (item.Margin.Left > 1))
						{
							return item;
						}
				}
			}

			return null;
		}


        private RadToolStripElement previous;
        private RadToolStripElement next;

		private RadToolStripElement ChangeToolStripItemHorizontalRow(Point pt)
		{
			RadToolStripElement element = GetElementFromLocation(pt);
			if (element != null)
				if (element != this.ParentToolStripElement)
				{
					element.Visibility = ElementVisibility.Visible;

					if (!element.Items.Contains(this.ParentToolStripItem))
					{
                        RadToolStripElement parent = this.parentToolStripElement;

                        ToolStripChangedEventArgs originalArgs = new ToolStripChangedEventArgs(
                        parent, element);

						this.ResetHorizontalAffectedRow(element);

                        if (previous != parent && next != element)
                        {
                            RoutedEventArgs args = new RoutedEventArgs(originalArgs, RowChangedEvent);
                            this.RaiseBubbleEvent(this, args);
                        }

                        previous = parent;
                        next = element;

						return element;
					}
				}

			return null;
		}

		private RadToolStripElement ChangeToolStripItemVerticalRow(Point pt)
		{
            RadToolStripElement element = GetElementFromLocation(pt);

			if (element != null)
				if (element != this.ParentToolStripElement)
				{
					element.Visibility = ElementVisibility.Visible;

					if (!element.Items.Contains(this.ParentToolStripItem))
					{

						this.ResetVerticalAffectedRow(element);

						ToolStripChangedEventArgs originalArgs = new ToolStripChangedEventArgs(
					this.parentToolStripElement, element);
						RoutedEventArgs args = new RoutedEventArgs(originalArgs, RowChangedEvent);
						this.RaiseBubbleEvent(this, args);
		
						return element;
					}
				}
			return null;
		}



		private bool HasAnyItemBeforeCurrentWithTopMargin()
		{
			int margin = 0;
			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				if (this.ParentToolStripItem.FullBoundingRectangle.Top > item.FullBoundingRectangle.Top)
					margin += item.Margin.Top;
			}

			if (margin > 0)
				return true;

			return false;
		}

		private void PerformVerticalDragging(Point cursorLocation)
		{
            if (this.ParentToolStripManager == null)
            {
                return;
            }

            RadToolStripItem bottomMostItem = this.GetOuterMostItem(Direction.Down);
			Point pt = this.ParentToolStripManager.PointFromScreen(Cursor.Position);

			this.movePos.Offset(cursorLocation.X - this.lastPos.X, cursorLocation.Y - this.lastPos.Y);
			this.movePos.Y = pt.Y - this.ParentToolStripItem.FullBoundingRectangle.Top - this.ParentToolStripItem.Margin.Top;

			int marginToSet = this.ParentToolStripItem.Margin.Top + this.movePos.Y;

			if (lastPos.Y == 0) marginToSet = Math.Abs(marginToSet);
			int currentMargin = this.ParentToolStripItem.Margin.Top;

			if (this.movePos.Y < 0)
			{
				if (this.HasAnyItemBeforeCurrentWithTopMargin() && (this.ParentToolStripItem.Margin.Top < 2))
				{
					RadToolStripItem topItem = this.GetItemBeforeCurrentWithTopMargin(this.ParentToolStripItem);
					if (topItem != null)
					{
						if (topItem.Margin.Top + movePos.Y >= 0)
						{

							topItem.Margin = new Padding(topItem.Margin.Left, topItem.Margin.Top + movePos.Y, topItem.Margin.Right,
							topItem.Margin.Bottom);

							RadToolStripItem poppeditem = this.GetNearestItemAfterCurrentWithTopMargin(this.ParentToolStripItem);
							if (poppeditem != null)
								poppeditem.Margin = new Padding(poppeditem.Margin.Left, poppeditem.Margin.Top - this.movePos.Y, poppeditem.Margin.Right,
									poppeditem.Margin.Bottom);

						}
					}

					return;
				}
			}
			/////////////////
			if (marginToSet > 0)
			{
				RadToolStripItem item = this.GetItemAfterCurrentWithTopMargin(this.ParentToolStripItem);

				// Sets the new margin to the item which we are dragging
				if (bottomMostItem != null)
					if (((bottomMostItem.FullBoundingRectangle.Bottom + this.movePos.Y) <= this.ParentToolStripManager.FullBoundingRectangle.Bottom)
						|| ((bottomMostItem != this.ParentToolStripItem) && (bottomMostItem.Margin.Top > 3)) || (this.lastPos.Y == 0 ))
					{
						{
							if (item != null)
								
								item.Margin = new Padding(item.Margin.Left, item.Margin.Top - this.movePos.Y, item.Margin.Right,
									item.Margin.Bottom);

							this.ParentToolStripItem.Margin = new Padding(this.ParentToolStripItem.Margin.Left, marginToSet, this.ParentToolStripItem.Margin.Right,
								this.ParentToolStripItem.Margin.Bottom);

							if (this.ParentToolStripItem.FullBoundingRectangle.Bottom > this.ParentToolStripManager.Bounds.Bottom)
							{
								int margin = this.ParentToolStripItem.FullBoundingRectangle.Bottom - this.ParentToolStripManager.Bounds.Bottom;
								this.ParentToolStripItem.Margin = new Padding(this.ParentToolStripItem.Margin.Left, marginToSet - margin, this.ParentToolStripItem.Margin.Right,
									this.ParentToolStripItem.Margin.Bottom);
							}
						}
					}
					else
					{
						if ((this.movePos.Y < 0) || (item != null))
						{
							
							this.ParentToolStripItem.Margin = new Padding(this.ParentToolStripItem.Margin.Left, marginToSet, this.ParentToolStripItem.Margin.Right,
								this.ParentToolStripItem.Margin.Bottom);

							if (item != null)
								if ((item.Margin.Top > this.movePos.Y) && (bottomMostItem.FullBoundingRectangle.Bottom + this.movePos.Y) <= this.ParentToolStripManager.FullBoundingRectangle.Bottom)
								{
									if (item.Margin.Top - this.movePos.Y >= 0)
										item.Margin = new Padding(item.Margin.Left, item.Margin.Top - this.movePos.Y, item.Margin.Right,
											item.Margin.Bottom);
								}
							
						}
					}
			}
			else
			{
				this.ParentToolStripItem.Margin = new Padding(this.ParentToolStripItem.Margin.Left, 0, this.ParentToolStripItem.Margin.Right,
			this.ParentToolStripItem.Margin.Bottom);
			}
		}



		private void PerformHorizontalDragging(Point cursorLocation)
		{
			if (this.ParentToolStripManager == null) return;
		
			Point pt = this.ParentToolStripManager.PointFromScreen(Cursor.Position);

			this.movePos.Offset(cursorLocation.X - this.lastPos.X, cursorLocation.Y - this.lastPos.Y);

			this.movePos.X = pt.X - this.ParentToolStripItem.FullBoundingRectangle.Left - this.ParentToolStripItem.Margin.Left;

			int marginToSet = this.ParentToolStripItem.Margin.Left + this.movePos.X;
		
			if (marginToSet + this.Bounds.Right > this.ParentToolStripManager.Bounds.Right)
				marginToSet = marginToSet + this.Bounds.Right - this.ParentToolStripManager.Bounds.Right; 
			
			int currentMargin = this.ParentToolStripItem.Margin.Left;



            RadToolStripItem rightMostItem = this.GetOuterMostItem(Direction.Right);
			this.SuspendLayout();
			if (this.movePos.X < 0)
			{
				if (HasAnyItemBeforeCurrentWithLeftMargin() && this.ParentToolStripItem.Margin.Left < 2)
				{
					RadToolStripItem leftItem = this.GetItemBeforeCurrentWithLeftMargin(this.ParentToolStripItem);
					if (leftItem != null)
					{
						if ((leftItem.Margin.Left + this.movePos.X) >= 0)
						{
							leftItem.Margin = new Padding(leftItem.Margin.Left + this.movePos.X, leftItem.Margin.Top, leftItem.Margin.Right,
							leftItem.Margin.Bottom);

							this.GetItemsAfterCurrent(this.ParentToolStripItem);

							RadToolStripItem poppeditem = this.GetNearestItemAfterCurrentWithLeftMargin(this.ParentToolStripItem);
							if (poppeditem != null)
							{
								poppeditem.Margin = new Padding(poppeditem.Margin.Left - this.movePos.X, poppeditem.Margin.Top, poppeditem.Margin.Right,
										poppeditem.Margin.Bottom);
								if (poppeditem.Margin.Left < 0 )
									poppeditem.Margin = new Padding(0, poppeditem.Margin.Top, poppeditem.Margin.Right,
																		poppeditem.Margin.Bottom);
							
							}
						}
					}
					this.ResumeLayout(true);
					return;
				}

			}


			if (marginToSet > 0)
			{

				RadToolStripItem item = this.GetItemAfterCurrentWithLeftMargin(this.ParentToolStripItem);

				if (((rightMostItem.FullBoundingRectangle.Right + this.movePos.X) <= this.ParentToolStripManager.FullBoundingRectangle.Right)
					|| ((rightMostItem != this.ParentToolStripItem) && (rightMostItem.Margin.Left > 3)))
				{
					if (item != null)
					{
						if (item.Margin.Left - this.movePos.X >= 0)
							item.Margin = new Padding(item.Margin.Left - this.movePos.X, item.Margin.Top, item.Margin.Right,
												item.Margin.Bottom);				
					}

					this.ParentToolStripItem.Margin = new Padding(marginToSet, this.ParentToolStripItem.Margin.Top, this.ParentToolStripItem.Margin.Right,
					this.ParentToolStripItem.Margin.Bottom);

					this.ResumeLayout(true);
					return;
				}
				else
				{

					if (this.movePos.X > 0)
					{
						int difference = rightMostItem.FullBoundingRectangle.Right - this.ParentToolStripManager.Bounds.Right;
						rightMostItem.Margin = new Padding(rightMostItem.Margin.Left - difference, rightMostItem.Margin.Top,
							rightMostItem.Margin.Right, rightMostItem.Margin.Bottom);
						if (rightMostItem.Margin.Left < 0)
							rightMostItem.Margin = new Padding(0, rightMostItem.Margin.Top, rightMostItem.Margin.Right, rightMostItem.Margin.Bottom);

					}

					if ((this.movePos.X < 0) || (item != null))
					{

						this.ParentToolStripItem.Margin = new Padding(marginToSet, this.ParentToolStripItem.Margin.Top, this.ParentToolStripItem.Margin.Right,
							this.ParentToolStripItem.Margin.Bottom);

						if (item != null)
							if (item.Margin.Left - this.movePos.X >= 0)
								item.Margin = new Padding(item.Margin.Left - this.movePos.X, item.Margin.Top, item.Margin.Right,
												item.Margin.Bottom);

					}
				}
			}
			else
			{
				this.ParentToolStripItem.Margin = new Padding(0, this.ParentToolStripItem.Margin.Top, this.ParentToolStripItem.Margin.Right,
					this.ParentToolStripItem.Margin.Bottom);

			}
			this.ResumeLayout(true);
		}

		private void ResetLeftMargins(bool invalidateWithToolStripItem)
		{

			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				item.InvalidateLayout();
				if ((item == this.ParentToolStripItem) && (!invalidateWithToolStripItem))
				{
					continue;
				}

				item.Margin = new Padding(0, item.Margin.Top, item.Margin.Right, item.Margin.Bottom);
			}
		}

		private void ResetTopMargins(bool invalidateWithToolStripItem)
		{
			foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
			{
				item.InvalidateLayout();
				if ((item == this.ParentToolStripItem) && (!invalidateWithToolStripItem))
					continue;
				item.Margin = new Padding(item.Margin.Left, 0, item.Margin.Right, item.Margin.Bottom);
			}
		}

		private void SetThemeToFloatingToolStrip(RadToolStripItemControl control, FloatingForm form )
		{
            if (this.ParentToolStripManager.ElementTree.ComponentTreeHandler.ThemeName == "")
			{
				control.ThemeName = "ControlDefault";
				form.BackColor = Color.FromArgb(49, 101, 165);
				control.BackColor = Color.FromArgb(243, 248, 255);
			}
			else
			{

                control.ThemeName = this.ParentToolStripManager.ElementTree.ComponentTreeHandler.ThemeName;
				if (control.ThemeName == "Office2007Black")
				{
					form.ForeColor = Color.White;
					form.BackColor = Color.FromArgb(74, 81, 90);
					control.BackColor = Color.FromArgb(197, 201, 210);
				}

				if (control.ThemeName == "Office2007Silver")
				{
					form.ForeColor = Color.White;
					form.BackColor = Color.FromArgb(220, 218, 213);
					control.BackColor = Color.FromArgb(234, 236, 238);
				}

				if (control.ThemeName == "Telerik")
				{
					form.ForeColor = Color.White;
					form.BackColor = Color.FromArgb(152, 188, 44);
					control.BackColor = Color.White;
				}

			}	
		}

		public RadDropDownButton dropDownButton = new RadDropDownButton();
		public RadButton closeButton = new RadButton(); 

		private Bitmap normalBitmap;
		private Bitmap hoveredBitmap;

		internal void CreateFloatingToolStrip()
        {
            CancelEventArgs cancelArgs = new CancelEventArgs();
            OnFloatingFormCreating(cancelArgs);
            if (cancelArgs.Cancel == true)
                return;
            EventArgs floatingFormCreatedArgs = new EventArgs();
            OnFloatingFormCreated(floatingFormCreatedArgs);


			this.ParentToolStripManager.SuspendLayout();
			form = new FloatingForm(this.ParentToolStripManager);

			RadToolStripContainterElement toolStripItemsContainer = new RadToolStripContainterElement(form);

			int width = 0;
			int height = 0;

			form.Owner = this.ElementTree.Control.FindForm();
			foreach (RadItem item in this.ParentToolStripItem.Items)
			{
				
				item.AngleTransform = 0;

				item.MinSize = item.Size;

				width += item.FullBoundingRectangle.Width;
			
				if (height < item.FullBoundingRectangle.Height)
				{
					height = item.FullBoundingRectangle.Height;
				}	
				item.Location = Point.Empty;

				if (item is ToolStripItemsOverFlow.RadFakeElement)
				{
					toolStripItemsContainer.Items.Add((item as ToolStripItemsOverFlow.RadFakeElement).AssociatedItem);
			
				}
				else
					toolStripItemsContainer.Items.Add(item);
				
				item.Visibility = ElementVisibility.Visible;
			}


			RadToolStripItemControl control = new RadToolStripItemControl(toolStripItemsContainer);

            // Set image list for the floating control in order to preserve the images
            // in the reparented elements
            if (this.ElementTree != null)
                control.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;

			toolStripItemsContainer.Style = this.ParentToolStripManager.RootElement.Style;

			SetThemeToFloatingToolStrip(control, form );
			this.NotifyToolStripItemsChanged();

			this.ParentToolStripItem.Items.Clear();
            // !!! Removing ParentToolStripItem from the main toolstrip element will force
            // this.ElementTree.Control to be null
			this.ParentToolStripElement.Items.Remove(this.ParentToolStripItem);
			if (this.ParentToolStripElement.Items.Count == 0)
				this.ParentToolStripManager.Items.Remove(this.ParentToolStripElement);

			this.ParentToolStripManager.formList.Add(form);

			closeButton.MinimumSize = new Size(16, 14);
            closeButton.ThemeName = this.ParentToolStripManager.ElementTree.ComponentTreeHandler.ThemeName;
			closeButton.Image = new Bitmap(14, 14);
			closeButton.Visible = true;

			Stream stream = TelerikHelper.GetStreamFromResource(typeof(RadToolStripGripElement).Module.Assembly, "Telerik.WinControls.UI.ToolStrip.close.png");
			this.normalBitmap = Bitmap.FromStream(stream) as Bitmap;
			this.normalBitmap = new Bitmap(this.normalBitmap);
			stream.Dispose();

			stream = TelerikHelper.GetStreamFromResource(typeof(RadToolStripGripElement).Module.Assembly, "Telerik.WinControls.UI.ToolStrip.close2.png");
			this.hoveredBitmap = Bitmap.FromStream(stream) as Bitmap;
			this.hoveredBitmap = new Bitmap(this.hoveredBitmap);
			stream.Dispose();

			closeButton.Image = this.normalBitmap;
			closeButton.MouseEnter += new EventHandler(button_MouseEnter);

			this.dropDownButton.MinimumSize = new Size(14, 14);
            this.dropDownButton.ThemeName = this.ParentToolStripManager.ElementTree.ComponentTreeHandler.ThemeName;
			this.dropDownButton.Visible = true;

			closeButton.ImageAlignment = ContentAlignment.TopLeft;
			
			this.dropDownButton.ThemeName = "Plain";
			closeButton.ThemeName = "Plain";

			form.FadeDelay = this.ParentToolStripManager.FadeDelay;
			form.Key = this.ParentToolStripItem.Key;

			this.ParentToolStripManager.ResumeLayout(true);
			this.ParentToolStripManager.LayoutEngine.SetLayoutInvalidated(true);
			this.ParentToolStripManager.LayoutEngine.PerformParentLayout();
			this.ParentToolStripItem = null;
			this.ParentToolStripManager = null;
			this.ParentToolStripElement = null;
		
			form.Text = this.ParentToolStripItem.Text;

			if (this.ParentToolStripItem.FloatingFormPreferredSize == Size.Empty)
				form.Size = new Size(width + 6, height + 4 + form.CaptionHeight);
			else
				form.Size = this.ParentToolStripItem.FloatingFormPreferredSize;
			form.ShrinkWidths = this.ParentToolStripItem.ShrinkWidths;
			control.ContainerElement.ToolStripContainterElementFill.Size = form.Size;

            form.Controls.Add(control);

			SetControlGradientFill(control, form);

			closeButton.Bounds = new Rectangle(form.Bounds.Width - 20, 3, 12, 12);
			closeButton.Click += new EventHandler(button_Click);
			closeButton.MouseLeave += new EventHandler(button_MouseLeave);

			this.dropDownButton.Bounds = new Rectangle(form.Bounds.Width - 38, 3, 10, 10);
			this.dropDownButton.Click += new EventHandler(dropDownButton_Click);

			form.Controls.Add(closeButton);
			form.Controls.Add(dropDownButton);
                        
			Point mousePos = Control.MousePosition;
			form.Location = mousePos;
            NativeMethods.ShowWindow(form.Handle, NativeMethods.SW_SHOWNOACTIVATE);

            //offset the mouse pos by 5 (4 pixels is the border of the floating Form) along the two axis
            mousePos.Offset(5, 5);
            this.form.InitializeMove(mousePos);
			control.ContainerElement.Invalidate();
		}

        void form_Disposed(object sender, EventArgs e)
        {
        }

		private void button_MouseLeave(object sender, EventArgs e)
		{
			((RadButton)sender).Image = this.normalBitmap;
		
		}

		private void button_MouseEnter(object sender, EventArgs e)
		{
            ((RadButton)sender).Image = this.hoveredBitmap;
		}

		private void SetControlGradientFill(RadToolStripItemControl control, FloatingForm form)
		{

			switch (control.ThemeName)
			{
				case "ControlDefault":
					{
						control.ContainerElement.ToolStripContainterElementFill.BackColor2 = Color.FromArgb(198,223,255);
						form.BorderPen = new Pen(Color.FromArgb(198, 223, 255));
						break;
					}

				case "Office2007Black":
					{
						form.BorderPen = new Pen(Color.FromArgb(115, 130, 140));
						control.ContainerElement.ToolStripContainterElementFill.BackColor2 = Color.FromArgb(156, 166, 173);	
					
						break;
					}
				case "Office2007Silver":
					{
						control.ContainerElement.ToolStripContainterElementFill.BackColor2 = Color.FromArgb(156, 166, 173);
						form.BorderPen = new Pen(Color.FromArgb(115, 130, 140));
					
					
						break;
					}
				case "Telerik":
					{
						form.BorderPen = new Pen(Color.FromArgb(213, 234, 158));
					
						break;
					}
			}			
		}

		private void PrepareDropDown()
		{
			RadMenuItem customizeItem = new RadMenuItem();
            customizeItem.Text = RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.Customize);

			this.dropDownButton.Items.Add(customizeItem);
			customizeItem.Click += new EventHandler(customizeItem_Click);
		}

		private void customizeItem_Click(object sender, EventArgs e)
		{
            if (this.form.ToolStripManager.OverFlowDialog.Visible)
            {
                return;
            }

			this.form.ToolStripManager.OverFlowDialog.CleadDataFromPanel();
			this.form.ToolStripManager.OverFlowDialog.LoadDataInPanel();
			this.form.ToolStripManager.OverFlowDialog.ShowDialog();	
		
		}
	
		private void dropDownButton_Click(object sender, EventArgs e)
		{
			if (this.dropDownButton.Items.Count == 0)
			{
				PrepareDropDown();
				this.dropDownButton.DropDownDirection = RadDirection.Up;
			}

			this.dropDownButton.ShowDropDown();	
		}

		private void button_Click(object sender, EventArgs e)
		{
			if (this.form != null)
			{
				this.form.Visible = false;
			}

		}

	
		private void NotifyToolStripItemsChanged()
		{
			foreach (RadToolStripElement element in this.ParentToolStripManager.Items)
			{
				foreach (RadToolStripItem item in element.Items)
				{
					item.OverflowManager.ManagerChanged = true;
				}
			}
		}

		private void ResetNecessaryLeftMargins(int resetValue)
		{
			int difference = 0;
			RadToolStripItem itemWithLargestMargin = null;

			while (true)
			{
				foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
				{
					if (item.Margin.Left > difference)
					{
						difference = item.Margin.Left;
						itemWithLargestMargin = item;
					}
				}

				if (difference <= 0) break;
				if (resetValue <= 0) break;

				if (itemWithLargestMargin != null)
				{
					if (difference > resetValue)
					{
						itemWithLargestMargin.Margin = new Padding(itemWithLargestMargin.Margin.Left - resetValue,
							itemWithLargestMargin.Margin.Top, itemWithLargestMargin.Margin.Right, itemWithLargestMargin.Margin.Bottom);
						break;
					}
					else
					{
						itemWithLargestMargin.Margin = new Padding(itemWithLargestMargin.Margin.Left - resetValue,
							itemWithLargestMargin.Margin.Top, itemWithLargestMargin.Margin.Right, itemWithLargestMargin.Margin.Bottom);
						resetValue -= difference;
						difference = 0;
						itemWithLargestMargin = null;

					}
				}
				else 
					break;
			}
		}

		private void ResetNecessaryTopMargins(int resetValue)
		{
			int difference = 0;
			RadToolStripItem itemWithLargestMargin = null;

			while (true)
			{
				foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
				{
					if (item.Margin.Left > difference)
					{
						difference = item.Margin.Top;
						itemWithLargestMargin = item;
					}
				}

				if (difference <= 0) break;
				if (resetValue <= 0) break;

				if (itemWithLargestMargin != null)
				{
					if (difference > resetValue)
					{
						itemWithLargestMargin.Margin = new Padding(itemWithLargestMargin.Margin.Left,
							itemWithLargestMargin.Margin.Top - resetValue, itemWithLargestMargin.Margin.Right, itemWithLargestMargin.Margin.Bottom);
						break;
					}
					else
					{
						itemWithLargestMargin.Margin = new Padding(itemWithLargestMargin.Margin.Left,
							itemWithLargestMargin.Margin.Top - resetValue, itemWithLargestMargin.Margin.Right, itemWithLargestMargin.Margin.Bottom);
						resetValue -= difference;
						difference = 0;
						itemWithLargestMargin = null;

					}
				}
				else
					break;
			}
		}


        private bool DragStarted = false;
        private Padding marginBeforeDrag;
        private RadToolStripElement elementBeforeDrag;


		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.ParentToolStripManager != null)
			{
                int edgeOffset = 20;
                Rectangle rangeRect = new Rectangle(this.ParentToolStripManager.RectangleToScreen(this.ParentToolStripManager.Bounds).X - edgeOffset, this.ParentToolStripManager.RectangleToScreen(this.ParentToolStripManager.Bounds).Y - edgeOffset,
                            this.ParentToolStripManager.RectangleToScreen(this.ParentToolStripManager.Bounds).Width + 2 * edgeOffset, this.ParentToolStripManager.RectangleToScreen(this.ParentToolStripManager.Bounds).Height + 2 * edgeOffset);

				if (this.ParentToolStripManager.AllowDragging)
				{
					if (this.ClientRectangle == Rectangle.Empty)
						this.ClientRectangle = new Rectangle(0, 0, this.FullBoundingRectangle.Size.Width, this.FullBoundingRectangle.Size.Height);
                                        
					if ( this.Capture && (e.Button == MouseButtons.Left))
						if (this.ParentToolStripItem != null)
						{
                            if (DragStarted == false)
                            {
                                ToolStripDragEventArgs cancelEventArgs = new ToolStripDragEventArgs(
                                   this.parentToolStripElement, this.parentToolStripItem);

                                RoutedEventArgs routedCancelEventArgs =
                                        new RoutedEventArgs(cancelEventArgs, DragStartingEvent);

                                this.RaiseBubbleEvent(this, routedCancelEventArgs);

                                if (routedCancelEventArgs.Canceled == true)
                                {
                                    return;
                                }
                                
                                DragStarted = true;

                                marginBeforeDrag = this.ParentToolStripItem.Margin;
                                this.elementBeforeDrag = this.ParentToolStripElement;
                                                                
                                ToolStripDragEventArgs originalArgs = new ToolStripDragEventArgs(
                                    this.ParentToolStripElement, this.parentToolStripItem);
                                RoutedEventArgs args = new RoutedEventArgs(originalArgs, DragStartedEvent);
                                this.RaiseBubbleEvent(this, args);
                            }

							if (this.ParentToolStripManager.Orientation == Orientation.Horizontal)
							{
								this.renderingFramerate = RenderingMaxFramerate;
								RenderingMaxFramerate = 1000;

                                Control host = this.ElementTree.Control;
                                if(host != null)
                                {
                                    host.SuspendLayout();
                                }
								RadToolStripElement element = ChangeToolStripItemHorizontalRow(e.Location);
								this.PerformHorizontalDragging(e.Location);
                                if (host != null)
                                {
                                    host.ResumeLayout(true);
                                }

                                RadToolStripItem rightMostItem = this.GetOuterMostItem(Direction.Right);
								if (rightMostItem != null )
								{
									if (rightMostItem.FullBoundingRectangle.Right > this.ParentToolStripManager.Bounds.Right)
									{
										ResetNecessaryLeftMargins(rightMostItem.FullBoundingRectangle.Right - this.ParentToolStripManager.Bounds.Right);				
									}
								}

								if (! rangeRect.Contains(Cursor.Position))
								{
									if (this.ParentToolStripManager.AllowFloating)
										CreateFloatingToolStrip();

								}

								RenderingMaxFramerate = this.renderingFramerate;
							}
							else
							{
								this.renderingFramerate = RenderingMaxFramerate;
								RenderingMaxFramerate = 1000;

                                Control host = this.ElementTree.Control;
                                if (host != null)
                                {
                                    host.SuspendLayout();
                                }
                                RadToolStripElement element = ChangeToolStripItemVerticalRow(e.Location);
                                this.PerformVerticalDragging(e.Location);
                                if (host != null)
                                {
                                    host.ResumeLayout(true);
                                }

                                RadToolStripItem bottomMostItem = this.GetOuterMostItem(Direction.Down);
                                if (bottomMostItem != null)
                                {
                                    if (bottomMostItem.FullBoundingRectangle.Bottom > this.ParentToolStripManager.Bounds.Bottom)
                                    {
                                        ResetNecessaryTopMargins(bottomMostItem.FullBoundingRectangle.Bottom - this.ParentToolStripManager.Bounds.Bottom);
                                    }
                                }

								if ( ! rangeRect.Contains(Cursor.Position) )
								{
									if (this.ParentToolStripManager.AllowFloating)
										CreateFloatingToolStrip();

								}

                                RenderingMaxFramerate = this.renderingFramerate;
							}

							this.lastPos = e.Location;
						}


					if (this.form == null)
						Cursor.Current = Cursors.SizeAll;
				}
			}
			else
				if (this.form != null)
				{
					form.Location = Cursor.Position;
				}

			base.OnMouseMove(e);
		}


		protected override void OnMouseLeave(EventArgs e)
		{
			if (!this.Capture)
				Cursor.Current = Cursors.Default;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Capture = true;

			if ( this.ClientRectangle.Contains(e.Location) || (e.Button == MouseButtons.Left))
			{
				Cursor.Current = Cursors.SizeAll;

				this.parentToolStripItem = this.Parent.Parent as RadToolStripItem;

				if (this.parentToolStripItem != null)
				{
					if (this.ParentToolStripManager != null)
					{

						this.dragDropTimer.Start();

						if (this.ParentToolStripManager.Orientation == Orientation.Horizontal)
						{
							this.lastPos = new Point(this.parentToolStripItem.Margin.Left, 0);
							this.movePos = new Point(this.parentToolStripItem.Margin.Left, 0);
							this.isDragging = true;
						}
						else
						{
							this.lastPos = new Point(0, this.parentToolStripItem.Margin.Top);
							this.movePos = new Point(0, this.parentToolStripItem.Margin.Top);
							this.isDragging = true;
     					}
					}
				}
			}
			else
				Cursor.Current = Cursors.Default;

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
            if (DragStarted == true)
            {
                ToolStripDragEventArgs cancelEventArgsEnding = new ToolStripDragEventArgs(
                                    this.parentToolStripElement, this.parentToolStripItem);

                RoutedEventArgs routedCancelEventArgsEnding =
                        new RoutedEventArgs(cancelEventArgsEnding, DragEndingEvent);

                this.RaiseBubbleEvent(this, routedCancelEventArgsEnding);

                if (routedCancelEventArgsEnding.Canceled == true)
                {
                    if (this.ParentToolStripManager != null)
                        this.ParentToolStripManager.SuspendLayout();

                    this.ParentToolStripItem.Margin = marginBeforeDrag;
                                        
                    if (this.ParentToolStripManager != null)
                        this.ParentToolStripManager.ResumeLayout(true);

                    DragStarted = false;
                  
                    this.Capture = false;
                    
                    if (this.form != null)
                        this.form.Capture = false;

                    this.isDragging = false;

                    Cursor.Current = Cursors.Default;

                    this.dragDropTimer.Start();

                    base.OnMouseUp(e);
                    return;
                }

                ToolStripDragEventArgs cancelEventArgsEnded = new ToolStripDragEventArgs(
                                    this.parentToolStripElement, this.parentToolStripItem);

                RoutedEventArgs routedCancelEventArgsEnded =
                        new RoutedEventArgs(cancelEventArgsEnded, DragEndedEvent);

                this.RaiseBubbleEvent(this, routedCancelEventArgsEnded);
            }

            DragStarted = false;

			this.Capture = false;
			if (this.form != null)
				this.form.Capture = false;

			this.isDragging = false;

			Cursor.Current = Cursors.Default;

			this.dragDropTimer.Start();

			base.OnMouseUp(e);
		}


        private void OnFloatingFormCreating(CancelEventArgs args) 
        {
            ((RadToolStrip)this.ElementTree.Control).OnFloatingFormCreating(args);               
        }

        private void OnFloatingFormCreated(EventArgs args)
        {
            ((RadToolStrip)this.ElementTree.Control).OnFloatingFormCreated(args);
        }

		private void dragDropTimer_Tick(object sender, EventArgs e)
		{
			if (this.isDragging)
			{
				ToolStripDragEventArgs originalArgs = new ToolStripDragEventArgs(
							this.parentToolStripElement, this.parentToolStripItem);
				RoutedEventArgs args = new RoutedEventArgs(originalArgs, DragEndedEvent);
				this.RaiseBubbleEvent(this, args);
			}

			this.dragDropTimer.Stop();
		}

		protected override void CreateChildElements()
		{
			this.gripPrimitive = new GripPrimitive();
			this.gripPrimitive.Class = "ToolStripGripPrimitive";
			this.gripPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

			this.Children.Add(this.gripPrimitive);

		}
	}
}
