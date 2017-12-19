using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.Keyboard
{
	/// <summary>
	/// Represents the state of the modifier keys (SHIFT, CTRL, and ALT) in a
	/// <strong>Chord</strong>.
	/// </summary>
	[TypeConverter(typeof(ChordModifierConverter))]
    public class ChordModifier : IComparable, INotifyPropertyChanged
    {
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <strong>ChordModifier</strong> using data
		/// provided by Keys input.
		/// </summary>
		public ChordModifier(Keys pressedKey)
		{
			GetModifiers(this, pressedKey);
		}

		/// <summary>
		/// Initializes a new instance of the <strong>ChordModifier</strong> using explicit
		/// setting for every property.
		/// </summary>
		public ChordModifier(bool altModifier, bool controlModifier, bool shiftModifier)
		{
			this.altModifier = altModifier;
			this.controlModifier = controlModifier;
			this.shiftModifier = shiftModifier;
		}

		/// <summary>
		/// Initializes a new instance of the <strong>ChordModifier</strong> using data
		/// provided by another instance.
		/// </summary>
		public ChordModifier(ChordModifier chordModifier)
		{
			this.altModifier = chordModifier.altModifier;
			this.controlModifier = chordModifier.controlModifier;
			this.shiftModifier = chordModifier.shiftModifier;
		}

		/// <summary>
		/// Initializes a new instance of the <b xmlns="">ChordModifier</b> class with
		/// default settings.
		/// </summary>
		public ChordModifier()
		{
		} 
		#endregion

		//Fields
        private bool shiftModifier = false;
        private bool altModifier = false;
        private bool controlModifier = false;

		#region Properties
		/// <summary>
		/// Gets a value indicating if any of the modifier keys (SHIFT, CTRL, and ALT) is in a pressed state. 
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEmpty
		{
			get
			{
				return !(shiftModifier || controlModifier || altModifier);
			}
		}

		/// <summary>
		/// Gets a value indicating if the SHIFT modifier key is in a pressed state. 
		/// </summary>
		public bool ShiftModifier
		{
			get
			{
				return shiftModifier;
			}
			set
			{
				shiftModifier = value;
				this.OnNotifyPropertyChanged("ShiftModifier");
			}
		}

		/// <summary>
		/// Gets a value indicating if the CTRL modifier key is in a pressed state. 
		/// </summary>
		public bool ControlModifier
		{
			get
			{
				return controlModifier;
			}
			set
			{
				controlModifier = value;
				this.OnNotifyPropertyChanged("ControlModifier");
			}
		}

		/// <summary>
		/// Gets a value indicating if the ALT modifier key is in a pressed state. 
		/// </summary>
		public bool AltModifier
		{
			get
			{
				return altModifier;
			}
			set
			{
				altModifier = value;
				this.OnNotifyPropertyChanged("AltModifier");
			}
		} 
		#endregion

		#region Methods
		/// <summary>
		/// Updates a ChordModifier instance based on a Keys input value
		/// </summary>
		/// <param name="tempModifier">ChordModifier instance to update</param>
		/// <param name="pressedKey">Keys input value</param>
		/// <returns>ChordModifier instance with updated states</returns>
		public static ChordModifier GetModifiers(ChordModifier tempModifier, Keys pressedKey)
		{
			if ((pressedKey & Keys.Shift) == Keys.Shift)
			{
				tempModifier.ShiftModifier = true;
			}
			if ((pressedKey & Keys.Control) == Keys.Control)
			{
				tempModifier.ControlModifier = true;
			}
			if ((pressedKey & Keys.Alt) == Keys.Alt)
			{
				tempModifier.AltModifier = true;
			}
			return tempModifier;
		}

		/// <summary>
		/// Creates new ChordModifier instance based on a Keys input value
		/// </summary>
		/// <param name="pressedKey">Keys input value</param>
		/// <returns>ChordModifier instance</returns>
		public static ChordModifier GetModifiers(Keys pressedKey)
		{
			ChordModifier returnModifier = new ChordModifier();
			return GetModifiers(returnModifier, pressedKey);
		}

		/// <summary>
		/// Removes all data from the ChordModifier. 
		/// </summary>
		public void Clear()
		{
			this.shiftModifier = false;
			this.altModifier = false;
			this.controlModifier = false;
		} 
		#endregion

        #region IComparable Members

		/// <summary>
		/// Compares this instance to a specified object or ChordModifier and returns an indication of their relative values. 
		/// </summary>
		/// <returns>
		/// 	<para>A signed number indicating the relative values of this instance and
		///     value.</para>
		/// 	<div class="tableSection">
		/// 		<list type="table">
		/// 			<listheader>
		/// 				<term>
		/// 					<para>Return Value</para>
		/// 				</term>
		/// 				<description>
		/// 					<para>Description</para>
		/// 				</description>
		/// 			</listheader>
		/// 			<item>
		/// 				<term>
		/// 					<para>Less than zero</para>
		/// 				</term>
		/// 				<description>
		/// 					<para>This instance is less than
		///                     <span class="parameter">value</span>.</para>
		/// 				</description>
		/// 			</item>
		/// 			<item>
		/// 				<term>
		/// 					<para>Zero</para>
		/// 				</term>
		/// 				<description>
		/// 					<para>This instance is equal to
		///                     <span class="parameter">value</span>.</para>
		/// 				</description>
		/// 			</item>
		/// 			<item>
		/// 				<term>
		/// 					<para>Greater than zero</para>
		/// 				</term>
		/// 				<description>
		/// 					<para>This instance is greater than
		///                     <span class="parameter">value</span>.</para>
		/// 					<para>-or-</para>
		/// 					<para><span class="parameter">value</span> is a null reference
		///                     (<b>Nothing</b> in Visual Basic).</para>
		/// 				</description>
		/// 			</item>
		/// 		</list>
		/// 	</div>
		/// 	<h1 class="heading"><span onkeypress="ExpandCollapse_CheckKey(exceptionsToggle)" style="CURSOR: default" onclick="ExpandCollapse(exceptionsToggle)" tabindex="0"><img class="toggle" id="exceptionsToggle" alt="Collapse image" src="ms-help://MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.WinFX4VS.1033/cpref7/local/collapse_all.gif" onload="OnLoadImage()" name="toggleSwitch"/></span>Exceptions</h1>
		/// </returns>
		/// <param name="obj">
		/// An object to compare, or a null reference (<strong>Nothing</strong> in Visual
		/// Basic).
		/// </param>
        public int CompareTo(object obj)
        {
            ChordModifier tempChordModifier = obj as ChordModifier;
            if (tempChordModifier == null && obj is Keys)
            {
                tempChordModifier = GetModifiers(new ChordModifier(), (Keys)obj);
            }
            if (tempChordModifier != null)
            {
                if (this.AltModifier == tempChordModifier.AltModifier)
                {
                    if (this.ControlModifier == tempChordModifier.ControlModifier)
                    {
                        if (this.ShiftModifier == tempChordModifier.ShiftModifier)
                        {
                            return 0;
                        }
                        else if (this.ShiftModifier)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else if (this.ControlModifier)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (this.AltModifier)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return 1;
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
		/// Notifies clients that a property value has changed.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            //PropertyChangedEventHandler handler1 =
            //    (PropertyChangedEventHandler)this.Events[RadObject.PropertyChangedEventKey];
            //if (handler1 != null)
            //{
            //    handler1(this, new PropertyChangedEventArgs(propertyName));
            //}
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler1 = PropertyChanged;
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        #endregion
    }
}