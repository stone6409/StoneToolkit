using System;
using System.Windows.Input;

namespace StoneToolkit.WpfCommon.Input
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeWithParameter;
        private readonly Action _executeWithoutParameter;
        private readonly Func<object, bool> _canExecuteWithParameter;
        private readonly Func<bool> _canExecuteWithoutParameter;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _executeWithoutParameter = execute;
            _canExecuteWithoutParameter = canExecute;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _executeWithParameter = execute;
            _canExecuteWithParameter = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecuteWithParameter != null || _canExecuteWithoutParameter != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecuteWithParameter != null || _canExecuteWithoutParameter != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteWithParameter != null)
            {
                return _canExecuteWithParameter(parameter);
            }
            else if (_canExecuteWithoutParameter != null)
            {
                return _canExecuteWithoutParameter();
            }

            return true; 
        }

        public void Execute(object parameter)
        {
            if (_executeWithParameter != null)
            {
                _executeWithParameter(parameter);
            }
            else if (_executeWithoutParameter != null)
            {
                _executeWithoutParameter();
            }
        }
    }
}
