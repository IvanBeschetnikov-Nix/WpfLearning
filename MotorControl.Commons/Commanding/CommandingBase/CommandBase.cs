using System;
using System.Windows.Input;

namespace MotorControl.Commons.Commanding.CommandingBase
{
    public abstract class CommandBase : ICommand
    {
        protected abstract bool CanExecute(object parameter);
        protected abstract void Execute(object parameter);
    
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }
    
        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter);
        }
    
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
    
    public abstract class CommandBase<T> : ICommand
    {
        protected abstract bool CanExecute(T parameter);
        protected abstract void Execute(T parameter);
    
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute((T) parameter);
        }
    
        void ICommand.Execute(object parameter)
        {
            this.Execute((T) parameter);
        }
    
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}