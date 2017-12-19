using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
namespace Telerik.WinControls.UI
{
    //class TabStripDragDropBuilder
    //{
    //    private RadTabStrip tabStrip;
    //    private TabItem hover_tab;
    //    private TabItem drag_tab;
    //    private int item_drag_index;
    //    private int drop_location_index;
    //    private Point pt1;
    //    private Point pt2;
    //    private PositionPointer upPointer;
    //    private PositionPointer downPointer;
    //    private bool allowDrop;
    //    private int tabsidth;
    //    private Rectangle scrollArea;
    //    public System.Windows.Forms.Form outlineForm;


    //    public TabStripDragDropBuilder(RadTabStrip tabStrip )
    //    {
    //        this.tabStrip = tabStrip;
    //        this.tabStrip.MouseDown += new System.Windows.Forms.MouseEventHandler(tabStrip_MouseDown);
    //        this.tabStrip.DragDrop += new System.Windows.Forms.DragEventHandler(tabStrip_DragDrop);
    //        this.tabStrip.DragOver += new System.Windows.Forms.DragEventHandler(tabStrip_DragOver);
    //        this.tabStrip.MouseMove += new MouseEventHandler(tabStrip_MouseMove);
    //        this.tabStrip.MouseUp += new MouseEventHandler(tabStrip_MouseUp);
    //        this.outlineForm = null;
    //    }

    //    void tabStrip_MouseUp(object sender, MouseEventArgs e)
    //    {
    //        if (tabStrip.TabStripElement.AllowDragDrop)
    //        {
    //            if (tabsidth > tabStrip.Width)
    //            {
    //                if (scrollArea.Contains(pt1))
    //                    return;
    //            }

    //            pt2 = new Point(e.X, e.Y);
    //            RefreshTabsAfterDragDrop();
    //            if (outlineForm != null)
    //            {
    //                this.outlineForm.Dispose();
    //                this.outlineForm = null;

    //            }
    //        }
    //    }

    //    private void RefreshTabsAfterDragDrop()
    //    {
    //        TabItem itemToDrop = new TabItem();
    //        TabItem itemToMove = new TabItem();
    //        Rectangle rec = Rectangle.Empty;

    //        int index1 = -1;
    //        int index2 = -1;

    //        for (int i = 0; i < tabStrip.TabStripElement.Items.Count; i++)
    //        {
    //            TabItem item = (TabItem)tabStrip.TabStripElement.Items[i];
    //            rec = item.GetItemRectangleToTabStrip();
    //            if (rec.Contains(pt1)) { itemToDrop = (TabItem)tabStrip.TabStripElement.Items[i]; index1 = i; }
    //            if (rec.Contains(pt2)) { itemToMove = (TabItem)tabStrip.TabStripElement.Items[i]; index2 = i; }
    //        }
    //        if ((itemToMove != null) && (itemToDrop != null))
    //        {
    //            if ((index1 != -1) && (index2 != -1))
    //            {
    //                tabStrip.TabStripElement.Items.RemoveAt(index1);
    //                tabStrip.TabStripElement.Items.Insert(index2, itemToDrop);
    //                itemToDrop.Invalidate();
    //            }
    //        }
    //    }

    //      void tabStrip_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    //    {
    //        pt1 = new Point(e.X, e.Y);
       
    //        if (tabStrip.TabStripElement.AllowDragDrop)
    //        {
    //            tabsidth = 0;

    //            /*scrollArea = new Rectangle(tabStrip.Bounds.Right - tabStrip.TabStripElement.leftButton.Size.Width
    //                     - tabStrip.TabStripElement.rightButton.Size.Width - 10, tabStrip.TabStripElement.Bounds.Y,
    //            tabStrip.TabStripElement.leftButton.Size.Width + tabStrip.TabStripElement.rightButton.Size.Width + 10,
    //                     tabStrip.TabStripElement.Bounds.Bottom);*/

    //            foreach (TabItem tabItem in tabStrip.TabStripElement.Items)
    //            {
    //                tabsidth += tabItem.Size.Width;
    //            }

    //            if (tabsidth > tabStrip.Width)
    //            {
    //                if (scrollArea.Contains(pt1))
    //                    return;
    //            }

    //            TabItem item = tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y));
    //            if ( item != null)
    //            {
    //                tabStrip.TabStripElement.SelectedTab = item;
    //                tabStrip.TabStripElement.RefreshTabs(item);
    //                outlineForm = TelerikHelper.CreateOutlineForm();
    //                upPointer = new PositionPointer(Direction.Up);
    //                upPointer.Opacity = 0.3;
    //                downPointer = new PositionPointer(Direction.Down);
    //                downPointer.Opacity = 0.3;
    //                upPointer.ArrowColor = Color.FromArgb(0, 0, 139);
    //                downPointer.ArrowColor = Color.FromArgb(0, 0, 139);
    //                if (tabStrip.TabStripElement.GetItemByPoint(pt1) != null)
    //                {
    //                    outlineForm.BackgroundImage = tabStrip.TabStripElement.GetItemByPoint(pt1).GetItemBitmap();
    //                    outlineForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
    //                    outlineForm.Size = tabStrip.TabStripElement.GetItemByPoint(pt1).GetItemBitmap().Size;
    //                    outlineForm.MinimumSize = tabStrip.TabStripElement.GetItemByPoint(pt1).GetItemBitmap().Size;
    //                    outlineForm.Location = Cursor.Position;
    //                }

    //            }
    //        }
    //      }

    //    void tabStrip_MouseMove(object sender, MouseEventArgs e)
    //    {
    //        Point pt = this.tabStrip.PointToClient(new Point(e.X, e.Y));
    //        if (tabStrip.TabStripElement.AllowDragDrop)
    //        {
    //            if (outlineForm != null)
    //            {
    //                TabItem item = tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y));                      
    //                if ((upPointer != null) && (downPointer != null))
    //                {
    //                    outlineForm.AddOwnedForm(upPointer);
    //                    outlineForm.AddOwnedForm(downPointer);
    //                    if ((item != null) && ( item.Bounds.Left >= tabStrip.Left && item.Bounds.Right <= tabStrip.Right ))
    //                    {
    //                        upPointer.Show();
    //                        downPointer.Show();
    //                        upPointer.Size = new Size(6, 6);
    //                        downPointer.Size = new Size(5, 5);
    //                        upPointer.Location = this.tabStrip.TabStripElement.PointToScreen(new Point(tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y)).Location.X,
    //                            tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y)).Location.Y + tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y)).Size.Height));
    //                        downPointer.Location = this.tabStrip.TabStripElement.PointToScreen(new Point(tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y)).Location.X, 
    //                            tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y)).Location.Y - downPointer.Size.Height));
    //                    }
    //                }

    //                if (((item != null) && (item.Bounds.Left >= tabStrip.Left && item.Bounds.Right <= tabStrip.Right))                  
    //                && (tabStrip.TabStripElement.Items.Contains(item)))  
    //                {
    //                    Cursor.Current = Cursors.Default;
    //                }
    //                else
    //                    Cursor.Current = Cursors.No;

    //                if (!(tabStrip.ClientRectangle.Contains(tabStrip.PointToClient(Cursor.Position))))
    //                {
    //                    outlineForm.Visible = false;
    //                }
    //                else
    //                {
    //                    outlineForm.Visible = true;
    //                    outlineForm.Location = Cursor.Position;
    //                }
    //            }
    //        }
    //        ////////////
    //        if (outlineForm != null)
    //        {
    //            //    outlineForm.Location = System.Windows.Forms.Cursor.Position;

    //        }
    //    }
    //    private System.Drawing.Point loc;
    //    void tabStrip_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
    //    {

    //        /*      //Get the tab we are hovering over.
    //              Point pt = this.tabStrip.PointToClient(new Point(e.X, e.Y));
    //              for (int i = 0; i < tabStrip.TabStripElement.Items.Count; i++)
    //              {
    //                  TabItem item = (TabItem)tabStrip.TabStripElement.Items[i];
    //                  if (item.GetItemRectangleToTabStrip().Contains(pt))
    //                  {
    //                      hover_tab = item;
    //                  }
    //              }
    //            //  MessageBox.Show(hover_tab.Text);      
    //              //Make sure we are on a tab.
    //              if (hover_tab != null)
    //              {
    //                  //Make sure there is a TabItem being dragged.
    //                  if (e.Data.GetDataPresent(typeof(TabItem)))
    //                  {
    //                      e.Effect = DragDropEffects.Move;
    //                      drag_tab = (TabItem)e.Data.GetData(typeof(TabItem));

    //                      item_drag_index = tabStrip.TabStripElement.FindIndex(drag_tab);
    //                      drop_location_index = tabStrip.TabStripElement.FindIndex(hover_tab);
    //                  }
    //              }
    //              else
    //              {
    //                  e.Effect = DragDropEffects.None;
    //              }
    //         */
    //    }

    //    void tabStrip_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
    //    {
    //        /*   if (item_drag_index != drop_location_index)
    //           {
                
    //               ArrayList pages = new ArrayList();

    //               //Put all tab pages into an array.
    //               for (int i = 0; i < tabStrip.TabStripElement.Items.Count; i++)
    //               {
    //                   //Except the one we are dragging.
    //                   if (i != item_drag_index)
    //                       pages.Add(tabStrip.TabStripElement.Items[i]);
    //               }
              
    //               //Now put the one we are dragging it at the proper location.
    //               TabItem newItem = (TabItem)e.Data.GetData(typeof(TabItem));

    //               pages.Insert(drop_location_index, newItem);//drag_tab);

    //               //Make them all go away for a nanosec.
    //               tabStrip.TabStripElement.Items.Clear();
    //               tabStrip.TabStripElement.Items.AddRange((TabItem[])pages.ToArray(typeof(TabItem)));
             
    //               //Add them all back in.
    //               //Make sure the drag tab is selected.
    //               tabStrip.TabStripElement.SelectedTab = newItem;//drag_tab;
    //           }*/
    //    }

      
    //        /////////////////////////////////
    //        // if ( tabStrip.TabStripElement.outlineForm != null )
    //        //  if (tabStrip.TabStripElement.GetItemByPoint(new Point(e.X, e.Y)) != null)

    //        //     tabStrip.TabStripElement.outlineForm.BackgroundImage = TelerikPaintHelper.GetControlBmp(tabStrip);

    //        //       MessageBox.Show(e.Location.ToString());
    //        /*   TabItem itemToDrag = null;
    //       loc = new Point(e.X, e.Y);
    //        for( int i = 0; i < tabStrip.TabStripElement.Items.Count; i++ )
    //       {
    //           TabItem item = (TabItem)tabStrip.TabStripElement.Items[i];
    //           if (item.GetItemRectangleToTabStrip().Contains(e.Location))
    //           {
    //               itemToDrag = item;
    //           }
    //       }

    //       if ( itemToDrag != null )
    //       {
    //           tabStrip.DoDragDrop(itemToDrag, System.Windows.Forms.DragDropEffects.All);
    //       }*/        
    //}
}
