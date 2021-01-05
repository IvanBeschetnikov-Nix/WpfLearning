using System;
using System.Threading.Tasks;
using MotorControl.Commons.Commanding.CommandingBase;

namespace MotorControl.Commons.Commanding.RelayCommand
{
    public class RelayCommandAsync : CommandBaseAsync
    {
        private Func<Task> _execute;
        private Func<bool> _canExecute;

        public RelayCommandAsync(Func<Task> execute) : this(execute, null)
        {
        }

        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            var task = _execute();

            if (task is null)
                return;

            await task;
        }
    }
    
    public class RelayCommandAsync<T> : CommandBaseAsync<T>
    {
        private Func<T, Task> _execute;
        private Func<T, bool> _canExecute;

        public RelayCommandAsync(Func<T, Task> execute) : this(execute, null)
        {
        }

        public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(T parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        protected override async Task ExecuteAsync(T parameter)
        {
            var task = _execute(parameter);

            if (task is null)
                return;

            await task;
        }
    }
}