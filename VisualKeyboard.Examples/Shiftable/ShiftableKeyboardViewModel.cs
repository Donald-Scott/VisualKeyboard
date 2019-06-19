using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmDialogs;

namespace VisualKeyboard.Examples.Shiftable
{
    internal class ShiftableKeyboardViewModel : ViewModelBase, IModalDialogViewModel
    {
        private bool? dialogResult;

        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            set
            {
                if (value == dialogResult)
                {
                    return;
                }

                dialogResult = value;
                OnPropertyChanged(() => DialogResult);
            }
        }
    }
}
