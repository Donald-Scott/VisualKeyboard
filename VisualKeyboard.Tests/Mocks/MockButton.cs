using System.Windows.Controls;

namespace VisualKeyboard.Tests
{
    internal class MockButton : Button
    {
        public void ClickButton()
        {
            OnClick();
        }
    }
}
