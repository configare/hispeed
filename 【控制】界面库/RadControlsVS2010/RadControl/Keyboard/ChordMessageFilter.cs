using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.Keyboard
{
    public class ChordMessageFilter : IMessageFilter
    {
        // Fields
        private ChordModifier chordModifier;
        private Stack<Keys> stack = new Stack<Keys>();
		private Stack<Keys> tipsStack = new Stack<Keys>();
        ObservableCollection<Shortcuts> chordsConsumers = new ObservableCollection<Shortcuts>();
        private List<IComponentTreeHandler /*RadControl*/> keyTipsConsumers = new List<IComponentTreeHandler /*RadControl*/>();
        private static ChordMessageFilter filterInstance = null;
        private static readonly Dictionary<int, Keys> mappings = new Dictionary<int, Keys>(1);
        private static List<int> constants;
        private static Keys[] validKeys;
        private static List<Keys> testKeys;
        private static Chord testChord;
		private bool keyMapActivated = false;
        private IComponentTreeHandler /*RadControl*/ keyMapActivatedInstance = null;
        protected static List<ChordMessageFilter> registeredFilters = null;

        #region Constructors
        static ChordMessageFilter()
        {
            constants = new List<int>(6);
            constants.Add((int)(Keys.Control));
            constants.Add((int)(Keys.ControlKey));
            constants.Add((int)(Keys.Menu));
            constants.Add((int)(Keys.Alt));
            constants.Add((int)(Keys.Shift));
            constants.Add((int)(Keys.ShiftKey));
            Keys[] keysArray1 = new Keys[] { 
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Delete, Keys.Down, 
                Keys.E, Keys.End, Keys.F, Keys.F1, Keys.F10, Keys.F11, Keys.F12, Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F2, Keys.F20, 
                Keys.F21, Keys.F22, Keys.F23, Keys.F24, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, 
                Keys.K, Keys.L, Keys.Left, Keys.M, Keys.N, Keys.NumLock, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, 
                Keys.O, Keys.OemBackslash, Keys.OemClear, Keys.OemCloseBrackets, Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.OemPipe, Keys.Oemplus, Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, 
                Keys.Q, Keys.R, Keys.Right, Keys.S, Keys.Space, Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
             };
            validKeys = keysArray1;

            testKeys = new List<Keys>();
            testKeys.Add(Keys.D1);
            testKeys.Add(Keys.D2);
            ChordModifier testModifier = new ChordModifier(false, true, true);
            testChord = new Chord(testKeys, testModifier);

            registeredFilters = new List<ChordMessageFilter>();
        }

        internal ChordMessageFilter()
        {
        }

        #endregion

        public int RegistrationCount
        {
            get 
            {
                return registeredFilters.Count; 
            }
        }

        public static ChordMessageFilter Current
        {
            get 
            {
                return ChordMessageFilter.CreateInstance(); 
            }
        }

        public static bool InstanceRegistered = false;
        public static bool InstanceCreated = false;
        public readonly static int MaxChordSymbols = 5;

        public static int filterCount = 0;
        /// <summary>
        /// Initializes a new instance of the ChordMessageFilter class.
        /// </summary>
        /// <returns>Instance of the ChordMessageFilter class</returns>
        public static ChordMessageFilter CreateInstance() 
        {
            if (!InstanceCreated)
            {
                ChordMessageFilter.filterInstance = new ChordMessageFilter();
                InstanceCreated = true;                
            }
            return ChordMessageFilter.filterInstance;
        }

        public static bool RegisterInstance()
        {
            if (filterInstance != null)
	        {
                return RegisterInstance(filterInstance);
	        }
            return false;
        }

        internal static bool RegisterInstance(ChordMessageFilter filterInstance)
        {
            try
            {
                if (!registeredFilters.Contains(filterInstance))
                {
                    registeredFilters.Add(filterInstance);
                    Application.AddMessageFilter(filterInstance);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while registering ChordMessageFilter instance", ex);
            }
        }

        public static bool UnregisterInstance()
        {
            if (filterInstance != null)
            {
                return UnregisterInstance(filterInstance);
            }
            return false;
        }

        public static bool UnregisterInstance(ChordMessageFilter filterInstance)
        {
            try
            {
                if (registeredFilters.Contains(filterInstance))
                {
                    registeredFilters.Remove(filterInstance);
                    Application.RemoveMessageFilter(filterInstance);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while unregistering ChordMessageFilter instance", ex);
            }
        }

        public static bool UnregisterInstance(Shortcuts consumer)
        {
            try
            {
                if (registeredFilters.Contains(filterInstance))
                {
                    registeredFilters.Remove(filterInstance);
                    Application.RemoveMessageFilter(filterInstance);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while unregistering ChordMessageFilter instance", ex);
            }
        }

        /// <summary>
        /// Filters out a message before it is dispatched. 
        /// </summary>
        /// <remarks>
        /// Use PreFilterMessage to filter out a message before it is dispatched to a control or form. 
        /// For example, to stop the Click event of a Button control from being dispatched to the control, 
        /// you implement the PreFilterMessage method and return a true value when the Click message occurs. 
        /// You can also use this method to perform code work that you might need to do before the message is
        /// dispatched.
        /// </remarks>
        /// <param name="msg">The message to be dispatched. You cannot modify this message. </param>
        /// <returns>true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        bool IMessageFilter.PreFilterMessage(ref Message msg)
        {
            if ((msg.Msg < NativeMethods.WM_KEYFIRST) || (msg.Msg > NativeMethods.WM_KEYLAST))
            {
                return false;
            }
            // proccessing all the kyboard messages, and putting the resolved keys/chords in stack/queue
            int pressedKey = (int)msg.WParam;
            this.SetModifiers(pressedKey, true);
           
            if ((msg.Msg == NativeMethods.WM_KEYDOWN) ||
                (msg.Msg == NativeMethods.WM_SYSKEYDOWN))
            {
                int hiWord = ((msg.LParam.ToInt32() >> 0x10) & 0xffff);
                bool isRepeatingKey = ((hiWord & 0x4000) > 0);
                if (!isRepeatingKey)
                {
                    if (!constants.Contains(pressedKey))
                    {
                        Keys realKey = IsValidKey((Keys)pressedKey);
                        stack.Push(realKey);

						if (this.keyMapActivated)
						{
							tipsStack.Push(realKey);
						}
                    }
                }
                if (this.IsRuntimeChord())
                {
					if (this.ProccessChord())
					{
                        this.keyMapActivatedInstance = null;
						stack.Clear();
						return true;
					}
                }
				else if ((Keys)pressedKey == Keys.Menu ||
						(Keys)pressedKey == Keys.F10)
				{
                    this.InvokeKeyMaps();
				}
            }
            if ((msg.Msg == NativeMethods.WM_KEYUP) ||
                (msg.Msg == NativeMethods.WM_SYSKEYUP))
            {
                this.SetModifiers(pressedKey, false);
                if (this.chordModifier.ShiftModifier == false && this.chordModifier.ControlModifier == false &&
                    this.chordModifier.AltModifier == false)
                {
                    this.chordModifier = null;
                    stack.Clear();

                    if ((Keys)pressedKey == Keys.Menu ||
                        (Keys)pressedKey == Keys.F10)
                    {
                        return this.ProccessKeyMaps();
                    }
                }
            }
            return false;
        }

		#region Key Tips
		[EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RegisterKeyTipsConsumer(IComponentTreeHandler /*RadControl*/ consumer)
		{
			if (consumer.GetType().Name == "RadRibbonBar")
			{
				Current.keyTipsConsumers.Add(consumer);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void UnregisterKeyTipsConsumer(IComponentTreeHandler /*RadControl*/ consumer)
		{
			if (Current.keyTipsConsumers.Contains(consumer))
			{
				Current.keyTipsConsumers.Remove(consumer);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void UnregisterKeyTipsConsumer(int index)
		{
			if ((0 <= index) && (index < Current.keyTipsConsumers.Count))
			{
				Current.keyTipsConsumers.RemoveAt(index);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void ClearKeyTipsConsumers()
		{
			Current.keyTipsConsumers.Clear();
		}

		public bool ProccessKeyMaps()
		{
            RadControl control = this.keyMapActivatedInstance as RadControl;
            if (this.keyMapActivatedInstance != null && (control != null && control.EnableKeyMap))
			{
                IComponentTreeHandler temp = this.keyMapActivatedInstance;
                this.keyMapActivatedInstance = null;
                return temp.Behavior.SetKeyMap();
			}
			return false;
		}

        public void InvokeKeyMaps()
        {
            if (this.keyMapActivatedInstance == null)
            {
                for (int i = 0; i < keyTipsConsumers.Count; i++)
                {
                    if (keyTipsConsumers[i].Behavior.IsParentFormActive)
                    {
                        this.keyMapActivatedInstance = keyTipsConsumers[i];
                        return;
                    }
                }
            }
        }

		#endregion

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RegisterChordsConsumer(Shortcuts consumer)
        {
            if (Current.chordsConsumers.Count == 0)
            {
                ChordMessageFilter.RegisterInstance();
            }

            if (!Current.chordsConsumers.Contains(consumer))
            {
                Current.chordsConsumers.Add(consumer);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void UnregisterChordsConsumer(Shortcuts consumer)
        {
            if (Current.chordsConsumers.Contains(consumer))
            {
                Current.chordsConsumers.Remove(consumer);
            }

            if (Current.chordsConsumers.Count == 0)
            {
                ChordMessageFilter.UnregisterInstance();
            }
        }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void UnregisterChordsConsumer(int index)
		{
			if ((0 <= index) && (index < Current.chordsConsumers.Count))
			{
                Shortcuts consumer = Current.chordsConsumers[index];

                UnregisterChordsConsumer(consumer);
			}
		}

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void ClearChordsConsumers()
        {
            Current.chordsConsumers.Clear();

            ChordMessageFilter.UnregisterInstance();
        }

        protected virtual InputBinding FindChordPattern()
        {
			if (chordsConsumers.Count == 0)
			{
				return null;
			}
			return FindChordPattern(chordsConsumers);
        }

        protected virtual InputBinding FindChordPattern(IList<Shortcuts> list)
        {
			Form activeForm = Form.ActiveForm;

			for (int i = (list.Count-1); i >= 0; i--)
            {
				Form compareForm = null;
				if (list[i].OwnerForm != null) 
				{ 
					compareForm = list[i].OwnerForm;
				}

				if (compareForm == null || compareForm.Disposing ||
					(compareForm != activeForm) ||
					list[i].InputBindings.Count == 0)
				{
					continue;
				}
                InputBinding foundBinding = FindChordPattern(list[i].InputBindings);
                if (foundBinding != null)
                {
                    return foundBinding;
                }
            }
            return null;
        }

        protected virtual InputBinding FindChordPattern(InputBindingsCollection list) 
        {
            Chord runtimeChord = CreateRuntimeChord();

            for (int i = 0; i < list.Count; i++)
            {
                if (runtimeChord.CompareTo(list[i].Chord) == 0)
                {
					if (list[i].CommandContext != null)
					{
						if(typeof(Control).IsAssignableFrom(list[i].CommandContext.GetType()))
						{
							if (!(list[i].CommandContext as Control).Disposing ) //&&(list[i].CommandContext as Control).FindForm() != null
							{ 
								return list[i];						
							}
						}
						if (typeof(RadItem).IsAssignableFrom(list[i].CommandContext.GetType()))
						{
							return list[i];
						}					
					}
                }
            }
            return null;
        }
        protected virtual Chord FindChordPattern(Chord[] list) 
        {
            Chord runtimeChord = CreateRuntimeChord();
            for (int i = 0; i < list.Length; i++)
            {
                if (runtimeChord.CompareTo(list[i]) == 0)
                {
                    return list[i];
                }
            }
            return null;
        }
        
        protected virtual bool ProccessChord()
        {
			InputBinding binding = FindChordPattern();
			if (binding != null)
			{
				Chord foundChord = binding.Chord;
				if (foundChord != null)
				{
					if (binding.Command != null)
					{
						binding.Command.Execute(binding.CommandContext, null);
					}
					return true;
				}
			}
            return false;
        }
        protected virtual Chord CreateRuntimeChord()
        {
            List<Keys> tempKeys = new List<Keys>(1);
            tempKeys.AddRange(stack);
            return new Chord(tempKeys, this.chordModifier);
        }

        protected void SetModifiers(int pressedKey, bool set)
        {
            Keys modifierKeys = Control.ModifierKeys;
            if (this.chordModifier == null)
            {
                this.chordModifier = new ChordModifier();
            }
            if ((modifierKeys & Keys.Shift) == Keys.Shift)
            {
                this.chordModifier.ShiftModifier = true;
            }
            else
            {
                this.chordModifier.ShiftModifier = false;
            }
            if ((modifierKeys & Keys.Control) == Keys.Control)
            {
                this.chordModifier.ControlModifier = true;
            }
            else
            {
                this.chordModifier.ControlModifier = false;
            }
            if ((modifierKeys & Keys.Alt) == Keys.Alt)
            {
                this.chordModifier.AltModifier = true;
            }
            else
            {
                this.chordModifier.AltModifier = false;
            }
        }

        protected virtual bool IsRuntimeChord() 
        {
            return this.stack.Count > 0;
        }

        /// <summary>
        /// Calculates the character code of alphanumeric key of the Keys enum instance
        /// </summary>
        /// <param name="k">An instance of the Keys enumaration</param>
        /// <returns>The character code of the alphanumeric key</returns>
        public static byte CharCodeFromKeys(Keys k)
        {
            byte charCode = 0;
            if ((k.ToString().Length == 1) || ((k.ToString().Length > 2) && (k.ToString()[1] == ',')))
                charCode = (byte)k.ToString()[0];
            else if ((k.ToString().Length > 3) && (k.ToString()[0] == 'D') && (k.ToString()[2] == ','))
                charCode = (byte)k.ToString()[1];
            return charCode;
        }

        public static Keys KeysFromInt(int keys)
        {
            if ( mappings.ContainsKey(keys))
            {
                return mappings[keys];
            }
            return Keys.None;
        }

        private static Keys IsValidKey(Keys keyCode)
        {
            Keys[] keysArray1 = validKeys;
            for (int num1 = 0; num1 < keysArray1.Length; num1++)
            {
                Keys keys1 = keysArray1[num1];
                if (keys1 == keyCode)
                {
                    return keys1;
                }
            }
            return Keys.None;
        }

    }

    
}