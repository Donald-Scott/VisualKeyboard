using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace VisualKeyboard.Control
{
    /// <summary>
    /// This is the base class for all keyboard keys.  This class contains the functionality
    /// to simulate keyboard input to the OS.  It also contains an event that can be raised
    /// to notify listeners that a key press event occured.
    /// </summary>
    internal abstract class LogicalKey
    {
        private readonly IKeyboardSimulator keyboard;
        private readonly IInputDeviceStateAdaptor inputDeviceState;
        internal event EventHandler<LogicalKeyEventArgs> KeyPressed;

        internal LogicalKey(IInputSimulator inputSimulator, VirtualKeyCode key)
        {
            keyboard = inputSimulator.Keyboard;
            inputDeviceState = inputSimulator.InputDeviceState;
            KeyCode = (WindowsInput.Native.VirtualKeyCode)key;

        }
        public WindowsInput.Native.VirtualKeyCode KeyCode { get; private set; }

        /// <summary>
        /// Causes the KeyPressed event to be raised.
        /// </summary>
        internal virtual void ScreenKeyPress()
        {
            OnKeyPress(new LogicalKeyEventArgs(KeyCode));
        }

        /// <summary>
        /// Initiates an input into the OS so simulate a key down action.
        /// </summary>
        protected void KeyDown()
        {
            keyboard.KeyDown(KeyCode);
        }

        /// <summary>
        ///  Initiates an input into the OS so simulate a key up action.
        /// </summary>
        protected void KeyUp()
        {
            keyboard.KeyUp(KeyCode);
        }

        /// <summary>
        ///  Initiates an input into the OS so simulate a key press action.
        /// </summary>
        protected void KeyPress()
        {
            keyboard.KeyPress(KeyCode);
        }

        /// <summary>
        /// Initiates the input of one or more modifier keys and an additinal virtual key code, ex: Ctrl+C
        /// </summary>
        /// <param name="modifierKeys"></param>
        protected void ModifiedKeyStrokes(IList<WindowsInput.Native.VirtualKeyCode> modifierKeys)
        {
            keyboard.ModifiedKeyStroke(modifierKeys, KeyCode);
        }

        /// <summary>
        /// Initiates the input of one or more characters
        /// </summary>
        /// <param name="text"></param>
        protected void OutputText(string text)
        {
            keyboard.TextEntry(text);
        }

        /// <summary>
        /// Determines if a keyboard key is in the down state
        /// </summary>
        /// <returns></returns>
        protected bool IsHardwareKeyDown()
        {
            return inputDeviceState.IsHardwareKeyDown(KeyCode);
        }

        /// <summary>
        /// Determines if a toggling key, such as the caps lock key, is in effect.
        /// </summary>
        /// <returns></returns>
        protected bool IsTogglingKeyInEffect()
        {
            return inputDeviceState.IsTogglingKeyInEffect(KeyCode);
        }

        /// <summary>
        /// Raises the KeyPressed event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnKeyPress(LogicalKeyEventArgs args)
        {
            // Get the event handler containing the registered listeners.
            EventHandler<LogicalKeyEventArgs> handler = KeyPressed;

            // If there are no listeners,
            if (handler == null)
            {
                // Nothing to do.
                return;
            }

            // Extract the list of registered listeners.
            Delegate[] listeners = handler.GetInvocationList();
            EventHandler<LogicalKeyEventArgs> stateChangeListener;

            // For each registered listener in the list,
            foreach (Delegate listener in listeners)
            {
                try
                {
                    // Get the state change listener.
                    stateChangeListener = listener as EventHandler<LogicalKeyEventArgs>;

                    // Broadcast the event with the arguments provided.
                    stateChangeListener?.Invoke(this, args);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    System.Diagnostics.Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "KeyPressed listener threw exception: {0}", ex));
                    // Don't rethrow the exception .
                }
            }
        }
    }
}
