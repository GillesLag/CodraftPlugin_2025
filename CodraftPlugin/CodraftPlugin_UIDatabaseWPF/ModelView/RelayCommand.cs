using CodraftPlugin_UIDatabaseWPF.Model;
using System;
using System.Windows.Input;

namespace CodraftPlugin_UIDatabaseWPF
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _Action;
        private Action<FittingModel> _oke;

        public RelayCommand(Action action)
        {
            _Action = action;
        }

        public RelayCommand(Action<FittingModel> oke)
        {
            _oke = oke;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _Action?.Invoke();

            _oke?.Invoke((FittingModel)parameter);
        }
    }
}
