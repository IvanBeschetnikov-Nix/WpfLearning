using System.Threading.Tasks;

namespace MotorControl.Commons.Commanding.CommandingBase
{
    public abstract class CommandBaseAsync : CommandBase
    {
        protected abstract Task ExecuteAsync(object parameter);
    
        protected override void Execute(object parameter)
        {
            this.ExecuteAsync(parameter);
        }
    }
    
    public abstract class CommandBaseAsync<T> : CommandBase<T>
    {
        protected abstract Task ExecuteAsync(T parameter);
    
        protected override void Execute(T parameter)
        {
            this.ExecuteAsync(parameter);
        }
    }
}