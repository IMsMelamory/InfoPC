using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
namespace InfoPC
{
    public class ServiceControllerVM
    {

        public ServiceController ServiceController { get; set; }

        public ServiceControllerStatus Status => ServiceController.Status;

        public bool IsStopped => Status == ServiceControllerStatus.Stopped;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); ;
        }


    }
}
