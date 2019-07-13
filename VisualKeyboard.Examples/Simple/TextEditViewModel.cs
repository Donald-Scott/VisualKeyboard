using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualKeyboard.Examples
{
    internal class TextEditViewModel : ViewModelBase
    {
        internal TextEditViewModel()
        {
            TabName = "Text Edit Keyboard";
        }

        public string TabName
        {
            get { return Get(() => TabName); }
            private set { Set(() => TabName, value); }
        }
    }
}
