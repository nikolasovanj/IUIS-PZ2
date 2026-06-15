using System;
using System.Windows.Input;

namespace NetworkService.Helpers.Commands
{
    public class MyICommand : ICommand
    {
        Action _TargetExecuteMethod;
        Action _TargetUndoMethod;
        Func<bool> _TargetCanExecuteMethod;

        public MyICommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }
        public MyICommand(Action executeMethod, Action undoMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetUndoMethod = undoMethod;
        }

        public MyICommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
        public void Execute()
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
            }
        }
        public void Undo()
        {
            if (_TargetUndoMethod != null)
            {
                _TargetUndoMethod();
            }
        }
        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
            }
        }

    }

    public class MyICommand<T> : ICommand
    {

        Action<T> _TargetExecuteMethod;
        Action<T> _TargetUndoMethod;
        Func<T, bool> _TargetCanExecuteMethod;

        public MyICommand(Action<T> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }
        public MyICommand(Action<T> executeMethod, Action<T> undoMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetUndoMethod = undoMethod;
        }
        public MyICommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
        public void Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod((T)parameter);
            }
        }
        public void Undo(object parameter)
        {
            if (_TargetUndoMethod != null)
            {
                _TargetUndoMethod((T)parameter);
            }
        }
        #region ICommand Members

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                T tparm = (T)parameter;
                return _TargetCanExecuteMethod(tparm);
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod((T)parameter);
            }
        }

        #endregion
    }
}
