using System;
using System.Windows.Input;

namespace DefectViewProgram
{
    public class RelayCommand<T> : ICommand
    {
        Action<T> _execute;
        Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null) { }
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute.Invoke((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
