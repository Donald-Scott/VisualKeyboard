using WindowsInput;

namespace VisualKeyboard.Control
{
    internal abstract class ModifierKeyBase : VirtualKey
    {
        public ModifierKeyBase(IInputSimulator inputSimulator, VirtualKeyCode key) : base(inputSimulator, key)
        {

        }

        internal bool IsInEffect { get; set; }

        internal abstract void SynchroniseKeyState();
    }
}
