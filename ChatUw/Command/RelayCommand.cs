using System;
using System.Windows.Input;

namespace ChatUw.Command
{
    public class RelayCommand : RelayCommand<object>, ICommand
    {
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            :base(p=>execute(), canExecute == null? (Func<object, bool>)null : p=>canExecute())
        {
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
