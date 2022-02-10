using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InfoPC
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> callback;
        private readonly Action<Exception> onException;
        private bool isExecuting;

        public bool IsExecuting
        {
            get => isExecuting;
            set
            {
                isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
        public event EventHandler CanExecuteChanged;

        public AsyncRelayCommand(Func<object, Task> callback, Action<Exception> onException = null)
        {
            this.callback = callback;
            this.onException = onException;
        }

        public bool CanExecute(object parameter) => !IsExecuting;

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await callback(parameter);
            }
            catch (Exception e)
            {
                onException?.Invoke(e);
            }

            IsExecuting = false;
        }
    }
}
