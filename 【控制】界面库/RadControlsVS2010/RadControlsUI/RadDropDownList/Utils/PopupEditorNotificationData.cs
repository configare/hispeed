using Telerik.WinControls.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI.Data;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PopupEditorNotificationData
    {
        public enum Context
        {
            None,
            SelectedIndexChanged,
            SelectedIndexChanging,
            SelectedValueChanged,
            ListItemDataBinding,
            ListItemDataBound,
            CreatingVisualItem,
            KeyPress,
            TextChanged,
            MouseEvent,
            SortStyleChanged,
            VisualItemFormatting,
            MouseWheel,
            TextBoxDoubleClick,
            MouseUpOnEditorElement,
            DisplayMemberChanged,
            ValueMemberChanged,
            F4Press,
            Esc,
            KeyUpKeyDownPress,
            ItemsChanged,
            ItemsClear, 
            KeyDown,
            KeyUp
        }

        public PopupEditorNotificationData()
        {
            this.context = Context.None;
            this.valueChangedEventArgs = null;
            this.positionChangingCancelEventArgs = null;
            this.positionChangedEventArgs = null;
            this.listItemDataBindingEventArgs = null;
            this.listItemDataBoundEventArgs = null;
            this.creatingVisualListItemEventArgs = null;
            this.keyPressEventArgs = null;
            this.mouseEventArgs = null;
            this.sortStyleChanged = null;
            this.visualItemFormatting = null;
            this.keyEventArgs = null;
        }

        public PopupEditorNotificationData(ValueChangedEventArgs valueChangedEventArgs)
        : this()
        {
            this.context = Context.SelectedValueChanged;
            this.valueChangedEventArgs = valueChangedEventArgs;
        }

        public PopupEditorNotificationData(PositionChangingCancelEventArgs positionChangingCancelEventArgs)
        : this()
        {
            this.context = Context.SelectedIndexChanging;
            this.positionChangingCancelEventArgs = positionChangingCancelEventArgs;
        }

        public PopupEditorNotificationData(PositionChangedEventArgs positionChangedEventArgs)
        : this()
        {
            this.context = Context.SelectedIndexChanged;
            this.positionChangedEventArgs = positionChangedEventArgs;
        }

        public PopupEditorNotificationData(ListItemDataBindingEventArgs listItemDataBindingEventArgs)
        : this()
        {
            this.context = Context.ListItemDataBinding;
            this.listItemDataBindingEventArgs = listItemDataBindingEventArgs;
        }

        public PopupEditorNotificationData(ListItemDataBoundEventArgs listItemDataBoundEventArgs)
        : this()
        {
            this.context = Context.ListItemDataBound;
            this.listItemDataBoundEventArgs = listItemDataBoundEventArgs;
        }

        public PopupEditorNotificationData(CreatingVisualListItemEventArgs creatingVisualListItemEventArgs)
        {
            this.context = Context.CreatingVisualItem;
            this.creatingVisualListItemEventArgs = creatingVisualListItemEventArgs;
        }

        public PopupEditorNotificationData(KeyPressEventArgs keyPressEventArgs)
        : this()
        {
            this.context = Context.KeyPress;
            this.keyPressEventArgs = keyPressEventArgs;
        }

        public PopupEditorNotificationData(MouseEventArgs mouseEventArgs)
        {
            this.context = Context.MouseEvent;
            this.mouseEventArgs = mouseEventArgs;
        }

        public PopupEditorNotificationData(SortStyleChangedEventArgs sortStyleChanged)
        {
            this.context = Context.SortStyleChanged;
            this.sortStyleChanged = sortStyleChanged;
        }

        public PopupEditorNotificationData(VisualItemFormattingEventArgs visualItemFormatting)
        {
            this.context = Context.VisualItemFormatting;
            this.visualItemFormatting = visualItemFormatting;
        }


        public PopupEditorNotificationData(KeyEventArgs keyEventArgs)
        {
            this.context = Context.KeyUpKeyDownPress;
            this.keyEventArgs = keyEventArgs;
        }

        public Context context;
        public ValueChangedEventArgs valueChangedEventArgs;
        public PositionChangingCancelEventArgs positionChangingCancelEventArgs;
        public PositionChangedEventArgs positionChangedEventArgs;
        public ListItemDataBindingEventArgs listItemDataBindingEventArgs;
        public ListItemDataBoundEventArgs listItemDataBoundEventArgs;
        public CreatingVisualListItemEventArgs creatingVisualListItemEventArgs;
        public KeyPressEventArgs keyPressEventArgs;
        public MouseEventArgs mouseEventArgs;
        public SortStyleChangedEventArgs sortStyleChanged;
        public VisualItemFormattingEventArgs visualItemFormatting;
        public KeyEventArgs keyEventArgs;
    }
}
