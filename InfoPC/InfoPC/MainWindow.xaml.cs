using System;
using System.Windows;
using System.Windows.Interop;

namespace InfoPC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new PCViewModel();
            Left = SystemParameters.WorkArea.Width - Width;
            
    }
        
    }
    }

