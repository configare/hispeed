using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace Telerik.WinControls
{
    /// <summary>
    /// Contains definitions for the MS Windows Vista Aero theme.
    /// </summary>
    public static class VistaAeroTheme
    {
        /// <summary>
        /// Vista comboboxes
        /// </summary>
        public static class ComboBox
        {
            private static readonly string ClassName = "Combobox";

            public static class Border
            {
                private static readonly int Part = 4;
                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Focused = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }

            public static class Readonly
            {
                private static readonly int Part = 5;
                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }

            public static class DropDownButton
            {
                private static readonly int Part = 6;
                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }
        }

        /// <summary>
        /// Vista DateTimePickers
        /// </summary>
        public static class DatePicker
        {
            private static readonly string ClassName = "Datepicker";

            public static class Border
            {
                private static readonly int Part = 2;
                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Focused = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }

            public static class DropDownButton
            {
                private static readonly int Part = 3;
                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }
        }

        /// <summary>
        /// Vista TextBoxes
        /// </summary>
        public static class TextBox
        {
            private static readonly string ClassName = "Edit";

            public static class Border
            {
                private static readonly int Part = 6;
                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Focused = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }
        }

        /// <summary>
        /// Vista Headers
        /// </summary>
        public static class Header
        {
            private static readonly string ClassName = "Header";

            public static class Item
            {
                private static readonly int Part = 1;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement SortedNormal = VisualStyleElement.CreateElement(ClassName, Part, 4);
                public static readonly VisualStyleElement SortedHot = VisualStyleElement.CreateElement(ClassName, Part, 5);
                public static readonly VisualStyleElement SortedPressed = VisualStyleElement.CreateElement(ClassName, Part, 6);
            }

            public static class DropDown
            {
                private static readonly int Part = 5;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }
        }

        /// <summary>
        /// Vista Listboxes
        /// </summary>
        public static class ListBox
        {
            private static readonly string ClassName = "Listbox";

            public static class Border
            {
                private static readonly int Part = 1;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Focused = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }

            public static class Item
            {
                private static readonly int Part = 5;

                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement HotSelected = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Selected = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement SelectedNoFocus = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }
        }

        /// <summary>
        /// Vista ListViews
        /// </summary>
        public static class ListView
        {
            private static readonly string ClassName = "Listview";

            public static class Item
            {
                private static readonly int Part = 1;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Selected = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
                public static readonly VisualStyleElement SelectedNoFocus = VisualStyleElement.CreateElement(ClassName, Part, 5);
                public static readonly VisualStyleElement HotSelected = VisualStyleElement.CreateElement(ClassName, Part, 6);
            }

            public static class GroupHeaderLine
            {
                private static readonly int Part = 7;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
            }

            public static class GroupExpandButton
            {
                private static readonly int Part = 8;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
            }

            public static class GroupCollapseButton
            {
                private static readonly int Part = 9;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hot = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Pressed = VisualStyleElement.CreateElement(ClassName, Part, 3);
            }
        }

        /// <summary>
        /// Vista Flyout
        /// </summary>
        public static class FlyOut
        {
            private static readonly string ClassName = "Flyout";

            public static class Header
            {
                private static readonly int Part = 1;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hover = VisualStyleElement.CreateElement(ClassName, Part, 2);
            }

            public static class Body
            {
                private static readonly int Part = 2;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Emphasized = VisualStyleElement.CreateElement(ClassName, Part, 2);
            }

            public static class Label
            {
                private static readonly int Part = 3;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Selected = VisualStyleElement.CreateElement(ClassName, Part, 2);
                public static readonly VisualStyleElement Emphasized = VisualStyleElement.CreateElement(ClassName, Part, 3);
                public static readonly VisualStyleElement Disabled = VisualStyleElement.CreateElement(ClassName, Part, 4);
            }

            public static class Link
            {
                private static readonly int Part = 4;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Hover = VisualStyleElement.CreateElement(ClassName, Part, 2);
            }
        }

        /// <summary>
        /// Vista Flyout
        /// </summary>
        public static class Menu
        {
            private static readonly string ClassName = "Menu";

            public static class BarBackground
            {
                private static readonly int Part = 7;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
                public static readonly VisualStyleElement Inactive = VisualStyleElement.CreateElement(ClassName, Part, 1);
            }

            public static class PopupBackground
            {
                private static readonly int Part = 9;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
            }

            public static class PopupBorder
            {
                private static readonly int Part = 10;

                public static readonly VisualStyleElement Normal = VisualStyleElement.CreateElement(ClassName, Part, 1);
            }
        }
    }
}
