using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualKeyboard.Examples
{
    internal class ImagesViewModel : ViewModelBase
    {
        internal ImagesViewModel()
        {
            TabName = "Image Keyboard";
        }

        public string TabName
        {
            get { return Get(() => TabName); }
            private set { Set(() => TabName, value); }
        }
    }
}
