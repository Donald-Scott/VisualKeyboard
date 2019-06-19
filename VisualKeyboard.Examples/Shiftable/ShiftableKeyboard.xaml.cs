using System.Windows;
using System.Windows.Controls;

namespace VisualKeyboard.Examples.Shiftable
{
    /// <summary>
    /// Interaction logic for ShiftableKeyboard.xaml
    /// </summary>
    public partial class ShiftableKeyboard : UserControl
    {
        private bool isCapitilaized;
        private bool isShifted;
        public ShiftableKeyboard()
        {
            InitializeComponent();

            isCapitilaized = false;
            isShifted = false;

            lowerCaseKeyboard.Visibility = Visibility.Visible;
            shiftedKeyboard.Visibility = Visibility.Collapsed;
            capsKeyboard.Visibility = Visibility.Collapsed;
        }

        private void ModifierChanged(object sender, Control.ModifierChangedEventArgs e)
        {
            if (e.virtualKeyCode == Control.VirtualKeyCode.Shift)
            {
                isShifted = e.IsInEffect;
            }
            else if (e.virtualKeyCode == Control.VirtualKeyCode.Capital)
            {
                isCapitilaized = e.IsInEffect;
            }

            if (isShifted && isCapitilaized) //show lower case keyboard
            {
                lowerCaseKeyboard.Visibility = Visibility.Visible;
                shiftedKeyboard.Visibility = Visibility.Collapsed;
                capsKeyboard.Visibility = Visibility.Collapsed;
            }
            else if (isShifted) //show shifted keyboard
            {
                lowerCaseKeyboard.Visibility = Visibility.Collapsed;
                shiftedKeyboard.Visibility = Visibility.Visible;
                capsKeyboard.Visibility = Visibility.Collapsed;
            }
            else if (isCapitilaized) // show caps keyboar
            {
                lowerCaseKeyboard.Visibility = Visibility.Collapsed;
                shiftedKeyboard.Visibility = Visibility.Collapsed;
                capsKeyboard.Visibility = Visibility.Visible;
            }
            else // show lowercase keyboard
            {
                lowerCaseKeyboard.Visibility = Visibility.Visible;
                shiftedKeyboard.Visibility = Visibility.Collapsed;
                capsKeyboard.Visibility = Visibility.Collapsed;
            }

            lowerCaseKeyboard.SynchroniseModifierKeyState();
            shiftedKeyboard.SynchroniseModifierKeyState();
            capsKeyboard.SynchroniseModifierKeyState();
        }

    }
}
