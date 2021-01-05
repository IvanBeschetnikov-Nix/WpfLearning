using System;
using MotorControl.Commons.Commanding.CommandingBase;

namespace MotorControl.Commons.Commanding.RelayCommand
{
    public class RelayCommand : CommandBase
    {
        private Action _execute;
        private Func<bool> _canExecute;

        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        protected override void Execute(object parameter)
        {
            _execute();
        }
    }
    
    public class RelayCommand<T> : CommandBase<T>
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(T parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        protected override void Execute(T parameter)
        {
            _execute(parameter);
        }
    }
}