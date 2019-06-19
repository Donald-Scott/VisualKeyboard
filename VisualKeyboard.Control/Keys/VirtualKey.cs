using WindowsInput;

namespace VisualKeyboard.Control
{
    /// <summary>
    /// This class represents a simple keyboard key with a single virtual key code
    /// </summary>
    internal class VirtualKey : LogicalKey
    {
        public VirtualKey(IInputSimulator inputSimulator, VirtualKeyCode key)
            : base(inputSimulator, key)
        {
        }

        /// <summary>
        /// This method invokes the base class KeyPress method to simulate the keyboard inpu
        /// of the assigned virtual key code.  It also raises an event to notify listeners
        /// of the key press event.
        /// </summary>
        internal override void ScreenKeyPress()
        {
            KeyPress();
            base.ScreenKeyPress();
        }
    }
}
