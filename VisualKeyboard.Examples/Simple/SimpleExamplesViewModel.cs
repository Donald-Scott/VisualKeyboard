using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmDialogs;

namespace VisualKeyboard.Examples
{
    internal class SimpleExamplesViewModel : ViewModelBase, IModalDialogViewModel
    {
        private bool? dialogResult;

        internal SimpleExamplesViewModel()
        {
            Items = new ObservableCollection<ViewModelBase>();

            Items.Add(new QwertyViewModel());
            Items.Add(new TextEditViewModel());
            Items.Add(new EmailViewModel());

            SelectedTab = Items[0];
        }

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

        /// <summary>
        /// Gets a collection of view models that are selectible in the tab control
        /// </summary>
        public ObservableCollection<ViewModelBase> Items
        {
            get { return Get(() => Items); }
            private set { Set(() => Items, value); }
        }

        /// <summary>
        /// Gets or sets the current view model that is displayed in the tab control.
        /// </summary>
        public ViewModelBase SelectedTab
        {
            get { return Get(() => SelectedTab); }
            set { Set(() => SelectedTab, value); }
        }
    }
}
