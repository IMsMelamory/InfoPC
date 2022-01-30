using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InfoPC.Helpers;
using Microsoft.Xaml.Behaviors;

namespace InfoPC.Behaviors
{
    public class MouseEnterBehaviors : Behavior<Grid>
    {
        private Point _mousePositionOnMainWindow;
        protected override void OnAttached()
        {
            Application.Current.MainWindow.MouseEnter += OnMouseEnter;
        }
        protected override void OnDetaching()
        {

            Application.Current.MainWindow.MouseEnter -= OnMouseEnter;
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e)
        {
            _mousePositionOnMainWindow = Application.Current.MainWindow.PointFromScreen(new Point(0, 0));
            Application.Current.MainWindow.Visibility = Visibility.Collapsed;
            Task.Run(async () => await ShowWindow());
           

        }
        private async Task ShowWindow()
        {
            while (true)
            {
                await Task.Delay(1000);
                var _mousePositionOnScreen = MouseWindowsHelper.GetMousePosition();
                if (!(_mousePositionOnScreen.X + _mousePositionOnMainWindow.X > 0))
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (Application.Current.MainWindow != null)
                        {
                            Application.Current.MainWindow.Visibility = Visibility.Visible;
                        }
                    });
                    break;
                }
            }
            
            
        }

    }
}
