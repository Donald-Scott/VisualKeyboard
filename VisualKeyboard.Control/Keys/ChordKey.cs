using System.Collections.Generic;
using WindowsInput;

namespace VisualKeyboard.Control
{
    internal class ChordKey : LogicalKey
    {
        private readonly List<WindowsInput.Native.VirtualKeyCode> modifierKeys;

        public ChordKey(IInputSimulator inputSimulator, VirtualKeyCode key, VirtualKeyCollection modifierKeys) : base(inputSimulator, key)
        {
            this.modifierKeys = new List<WindowsInput.Native.VirtualKeyCode>();
            foreach (VirtualKeyCode keyCode in modifierKeys)
            {
                this.modifierKeys.Add((WindowsInput.Native.VirtualKeyCode)keyCode);
            }
        }

        internal override void ScreenKeyPress()
        {
            ModifiedKeyStrokes(modifierKeys);

            List<WindowsInput.Native.VirtualKeyCode> pressedKeys = new List<WindowsInput.Native.VirtualKeyCode>
            {
                KeyCode
            };
            pressedKeys.AddRange(modifierKeys);

            LogicalKeyEventArgs args = new LogicalKeyEventArgs(pressedKeys);
            OnKeyPress(args);
        }
    }
}
