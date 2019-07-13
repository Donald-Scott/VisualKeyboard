using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualKeyboard.Examples.Basic;
using VisualKeyboard.Examples.Shiftable;

namespace VisualKeyboard.Examples
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService dlgService;

        internal MainWindowViewModel()
        {
            dlgService = new DialogService();
        }

        public ICommand ExitCommand { get { return new RelayCommand(ExitApp); } }

        public ICommand ShowBasicExampleCommand { get { return new RelayCommand(ShowBasicExample); } }

        public ICommand ShowShiftableExampleCommand { get { return new RelayCommand(ShowShiftableExample); } }

        public ICommand ShowSimpleExamplesCommand { get { return new RelayCommand(ShowSimpleExample); } }

        private void ExitApp()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }

        private void ShowBasicExample()
        {
            BasicKeyboardViewModel vm = new BasicKeyboardViewModel();
            dlgService.ShowDialog<BasicKeyboardView>(this, vm);
        }

        private void ShowShiftableExample()
        {
            ShiftableKeyboardViewModel vm = new ShiftableKeyboardViewModel();
            dlgService.ShowDialog<ShiftableKeyboardView>(this, vm);
        }

        private void ShowSimpleExample()
        {
            SimpleExamplesViewModel vm = new SimpleExamplesViewModel();
            dlgService.ShowDialog<SimpleExamplesView>(this, vm);
        }
    }
}
