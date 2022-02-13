using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;



namespace InfoPC
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services : Window
    {
        

        public Services()
        {

            InitializeComponent();
            DataContext = new ServiceControllerVM();
        }

        
    }
}
