using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Establishes the common events and also the event-related properties and methods for basic input processing by 
    /// Telerik Presentation Foundation (TPF) elements. 
    /// </summary>
    public interface IInputElement
    {
        event KeyEventHandler KeyDown;

        event KeyEventHandler KeyUp;

        event KeyPressEventHandler KeyPress;

        event EventHandler MouseEnter;

        event EventHandler MouseLeave;

        event MouseEventHandler MouseMove;

        event MouseEventHandler MouseUp;

        event MouseEventHandler MouseDown;

        event MouseEventHandler MouseWheel;

        bool CaptureMouse();

        bool Focus();

        void ReleaseMouseCapture();

        bool Focusable { get; set; }

        bool IsEnabled { get; }

        bool IsFocused { get; }

        bool IsMouseCaptured { get; }

        bool IsMouseOver { get; }
    }
}