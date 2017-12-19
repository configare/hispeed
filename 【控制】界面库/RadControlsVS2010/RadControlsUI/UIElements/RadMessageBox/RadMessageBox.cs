using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public class RadMessageBox
    {
        private static RadMessageBoxForm radMessageBoxForm;// = new RadMessageBoxForm();

        /// <summary>
        /// Set theme name for the whole RadMessageBox
        /// </summary>
        /// <param name="themeName"></param>
        public static void SetThemeName(string themeName)
        {
            if (radMessageBoxForm == null)
            {
                radMessageBoxForm = new RadMessageBoxForm();
            }
            radMessageBoxForm.ThemeName = String.Empty;
            radMessageBoxForm.ThemeName = themeName;
        }

        /// <summary>
        /// Gets or set theme name for the whole RadMessageBox
        /// </summary>
        /// <param name="themeName"></param>
        public static string ThemeName
        {
            get
            {
                if (radMessageBoxForm == null)
                {
                    return String.Empty;
                }

                return radMessageBoxForm.ThemeName;
            }
            set
            {
                SetThemeName(value);
            }
        }

        /// <summary>
        /// Set the cursor that is displayed when the mouse pointer is over the control.
        /// </summary>
        public static Cursor Cursor
        {
            set
            {
                if (radMessageBoxForm == null)
                {
                    radMessageBoxForm = new RadMessageBoxForm();
                }
                radMessageBoxForm.Cursor = value;
            }
        }


        /// <summary>
        /// Set the message to be shown in windows taskbar. Default is <b>false</b>
        /// </summary>
        public static bool ShowInTaskbar
        {
            set
            {
                if (radMessageBoxForm == null)
                {
                    radMessageBoxForm = new RadMessageBoxForm();
                }

                radMessageBoxForm.ShowInTaskbar = value;
            }
        }


        /// <summary>
        /// Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </summary>
        public static bool UseCompatibleTextRendering
        {
            set
            {
                if (radMessageBoxForm == null)
                {
                    radMessageBoxForm = new RadMessageBoxForm();
                }
                radMessageBoxForm.UseCompatibleTextRendering = value;
            }
        }

        /// <summary>
        /// Displays RadMessageBox with specified text. 
        /// </summary>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values</returns>
        public static DialogResult Show(string text)
        {
            PlaySound(RadMessageIcon.None);
            return ShowCore(null, text, string.Empty, MessageBoxButtons.OK, null,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays RadMessageBox with specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(string text, string caption)
        {
            PlaySound(RadMessageIcon.None);
            return ShowCore(null, text, caption, MessageBoxButtons.OK, null,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox with specified text, caption, and buttons. 
        /// </summary>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            PlaySound(RadMessageIcon.None);
            return ShowCore(null, text, caption, buttons, null,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox with specified text, caption, buttons, and icon. 
        /// </summary>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon">One of the <see cref="T:Telerik.WinControls.RadMessageIcon"></see> values that specifies which icon to display in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons,
            RadMessageIcon icon)
        {
            PlaySound(icon);
            return ShowCore(null, text, caption, buttons, GetRadMessageIcon(icon),
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox with specified text, caption, buttons, icon and default button.
        /// </summary>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon">One of the <see cref="T:Telerik.WinControls.RadMessageIcon"></see> values that specifies which icon to display in the RadMessageBox.</param>
        /// <param name="defaultButton">One of the <see cref="T:System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons,
            RadMessageIcon icon, MessageBoxDefaultButton defaultButton)
        {
            PlaySound(icon);
            return ShowCore(null, text, caption, buttons, GetRadMessageIcon(icon), defaultButton,
                RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(IWin32Window parent, string text)
        {
            PlaySound(RadMessageIcon.None);
            return ShowCore(parent, text, string.Empty, MessageBoxButtons.OK, null,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text and caption. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption)
        {
            PlaySound(RadMessageIcon.None);
            return ShowCore(parent, text, caption, MessageBoxButtons.OK, null,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text, caption, and buttons. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption,
            MessageBoxButtons buttons)
        {
            PlaySound(RadMessageIcon.None);
            return ShowCore(parent, text, caption, buttons, null,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text, caption, buttons, and icon. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon">One of the <see cref="T:Telerik.WinControls.RadMessageIcon"></see> values that specifies which icon to display in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption,
            MessageBoxButtons buttons, RadMessageIcon icon)
        {
            PlaySound(icon);
            return ShowCore(parent, text, caption, buttons, GetRadMessageIcon(icon),
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text, caption, buttons, and icon. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon"><see cref="T:System.Drawing.Icon"></see> that displays in the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption,
            MessageBoxButtons buttons, Bitmap icon)
        {
            if (icon.Size.Height > 48 || icon.Size.Width > 48)
            {
                icon = new Bitmap(icon, new Size(48, 48));
            }

            return ShowCore(parent, text, caption, buttons, icon,
                MessageBoxDefaultButton.Button1, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text, caption, buttons, icon, and default button. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon">One of the <see cref="T:Telerik.WinControls.RadMessageIcon"></see> values that specifies which icon to display in the RadMessageBox.</param>
        /// <param name="defaultBtn">One of the <see cref="T:System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption,
            MessageBoxButtons buttons, RadMessageIcon icon, MessageBoxDefaultButton defaultBtn)
        {
            PlaySound(icon);
            return ShowCore(parent, text, caption, buttons, GetRadMessageIcon(icon),
                defaultBtn, RightToLeft.No);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text, caption, buttons, icon, and default button. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon">One of the <see cref="T:Telerik.WinControls.RadMessageIcon"></see> values that specifies which icon to display in the RadMessageBox.</param>
        /// <param name="defaultBtn">One of the <see cref="T:System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the RadMessageBox.</param>
        /// <param name="rtl">One of the <see cref="T:System.Windows.Forms.RightToLeft"></see> values that specifies right to left settings.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption,
            MessageBoxButtons buttons, RadMessageIcon icon, MessageBoxDefaultButton defaultBtn, RightToLeft rtl)
        {
            PlaySound(icon);
            return ShowCore(parent, text, caption, buttons, GetRadMessageIcon(icon),
                defaultBtn, rtl);
        }

        /// <summary>
        /// Displays a RadMessageBox in front of the specified object and with the specified text, caption, buttons, icon, and default button. 
        /// </summary>
        /// <param name="parent">An implementation of <see cref="T:System.Windows.Forms.IWin32Window"></see> that will own the RadMessageBox.</param>
        /// <param name="text">The text to display in the RadMessageBox.</param>
        /// <param name="caption">The text to display in the title bar of the RadMessageBox.</param>
        /// <param name="buttons">One of the <see cref="T:System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the RadMessageBox.</param>
        /// <param name="icon"><see cref="T:System.Drawing.Icon"></see> that displays in the RadMessageBox.</param>
        /// <param name="defaultBtn">One of the <see cref="T:System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the RadMessageBox.</param>
        /// <returns>One of the <see cref="T:System.Windows.Forms.DialogResult"></see> values.</returns>
        public static DialogResult Show(IWin32Window parent, string text, string caption,
            MessageBoxButtons buttons, Bitmap icon, MessageBoxDefaultButton defaultBtn)
        {
            if (icon.Size.Height > 48 || icon.Size.Width > 48)
            {
                icon = new Bitmap(icon, new Size(48, 48));
            }

            return ShowCore(parent, text, caption, buttons, icon,
                defaultBtn, RightToLeft.No);
        }

        private static DialogResult ShowCore(IWin32Window owner, string text, string caption,
            MessageBoxButtons buttons, Bitmap icon, MessageBoxDefaultButton defaultButton,
            RightToLeft rightToLeft)
        {

            if (radMessageBoxForm == null)
            {
                radMessageBoxForm = new RadMessageBoxForm();
            }

            radMessageBoxForm.DialogResult = DialogResult.Cancel;
            //radMessageBoxForm.DefaultButton = defaultButton;

            //Set RigtToLeft
            radMessageBoxForm.RightToLeft = rightToLeft;

            //Set message text
            radMessageBoxForm.MessageText = text;

            radMessageBoxForm.StartPosition = FormStartPosition.CenterParent;

            //Set Owner Form
            if (owner != null)
            {
                radMessageBoxForm.Owner = Control.FromHandle(owner.Handle).FindForm();
            }
            else if (Form.ActiveForm != null && !Form.ActiveForm.InvokeRequired)
            {
                radMessageBoxForm.Owner = Form.ActiveForm;
            }
            else
            {
                radMessageBoxForm.StartPosition = FormStartPosition.CenterScreen;
            }

            if (radMessageBoxForm.Owner != null)
            {
                radMessageBoxForm.TopMost = radMessageBoxForm.Owner.TopMost;
            }

            //Set Caption
            if (caption != string.Empty && caption != null)
            {
                radMessageBoxForm.Text = caption;
            }
            else
            {
                radMessageBoxForm.Text = "Message";
            }

            //Set Icon
            radMessageBoxForm.MessageIcon = icon;

            //Set Buttons
            radMessageBoxForm.ButtonsConfiguration = buttons;

            //set default button
            radMessageBoxForm.DefaultButton = defaultButton;


            //Show Message
            radMessageBoxForm.ShowDialog();

            DialogResult ds = DialogResult.OK;

            //DialogResult
            if (buttons != MessageBoxButtons.OK)
            {
                ds = radMessageBoxForm.DialogResult;
            }

            radMessageBoxForm.Dispose();
            radMessageBoxForm = null;

            return ds;

        }


        private static Bitmap GetRadMessageIcon(RadMessageIcon icon)
        {
            Stream stream;
            Bitmap image;

            switch (icon)
            {

                case RadMessageIcon.Info:
                    stream = (System.Reflection.Assembly.GetExecutingAssembly().
                        GetManifestResourceStream("Telerik.WinControls.UI.Resources.RadMessageBox.MessageInfo.png"));
                    image = Bitmap.FromStream(stream) as Bitmap;
                    stream.Close();
                    return image;
                case RadMessageIcon.Question:
                    stream = (System.Reflection.Assembly.GetExecutingAssembly().
                        GetManifestResourceStream("Telerik.WinControls.UI.Resources.RadMessageBox.MessageQuestion.png"));
                    image = Bitmap.FromStream(stream) as Bitmap;
                    stream.Close();
                    return image;
                case RadMessageIcon.Exclamation:
                    stream = (System.Reflection.Assembly.GetExecutingAssembly().
                        GetManifestResourceStream("Telerik.WinControls.UI.Resources.RadMessageBox.MessageExclamation.png"));
                    image = Bitmap.FromStream(stream) as Bitmap;
                    stream.Close();
                    return image;
                case RadMessageIcon.Error:
                    stream = (System.Reflection.Assembly.GetExecutingAssembly().
                        GetManifestResourceStream("Telerik.WinControls.UI.Resources.RadMessageBox.MessageError.png"));
                    image = Bitmap.FromStream(stream) as Bitmap;
                    stream.Close();
                    return image;
            }

            return null;
        }

        private static void PlaySound(RadMessageIcon icon)
        {
            switch (icon)
            {
                case RadMessageIcon.None:
                    SystemSounds.Beep.Play();
                    break;
                case RadMessageIcon.Info:
                    SystemSounds.Asterisk.Play();
                    break;
                case RadMessageIcon.Question:
                    SystemSounds.Question.Play();
                    break;
                case RadMessageIcon.Exclamation:
                    SystemSounds.Exclamation.Play();
                    break;
                case RadMessageIcon.Error:
                    SystemSounds.Hand.Play();
                    break;
            }

        }

        // method to retrieve an embedded icon resource
        //private System.Drawing.Icon GetIcon(string strIdentifier)
        //{
        //    System.Drawing.Icon functionReturnValue = default(System.Drawing.Icon);
        //    // use the strIdentifier argument to retrieve the 
        //    // appropriate resource from the assembly
        //    {
        //        // read the resource from the returned stream
        //        functionReturnValue = 
        //            new System.Drawing.Icon(new System.IO.StreamReader(
        //            System.Reflection.Assembly.GetEntryAssembly.
        //            GetManifestResourceStream(strIdentifier)).BaseStream);
        //        // close the stream
        //        new System.IO.StreamReader(
        //            System.Reflection.Assembly.GetEntryAssembly.
        //            GetManifestResourceStream(strIdentifier)).Close();
        //    }
        //    return functionReturnValue;
        //}
    }
}
