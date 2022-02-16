using System.Windows;



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
