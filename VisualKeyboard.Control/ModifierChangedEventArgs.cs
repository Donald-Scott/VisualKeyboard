using System.Windows;

namespace VisualKeyboard.Control
{
    public class ModifierChangedEventArgs : RoutedEventArgs
    {
        public ModifierChangedEventArgs(VirtualKeyCode keyCode, bool isInEffect)
        {
            virtualKeyCode = keyCode;
            IsInEffect = isInEffect;
        }

        public ModifierChangedEventArgs(RoutedEvent routedEvent, VirtualKeyCode keyCode, bool isInEffect) : base(routedEvent)
        {
            virtualKeyCode = keyCode;
            IsInEffect = isInEffect;
        }

        public ModifierChangedEventArgs(RoutedEvent routedEvent, object source, VirtualKeyCode keyCode, bool isInEffect) : base(routedEvent, source)
        {
            virtualKeyCode = keyCode;
            IsInEffect = isInEffect;
        }

        public VirtualKeyCode virtualKeyCode { get; }

        public bool IsInEffect { get; }
    }

    public delegate void ModifierChangedEventHandler(object sender, ModifierChangedEventArgs e);
}
