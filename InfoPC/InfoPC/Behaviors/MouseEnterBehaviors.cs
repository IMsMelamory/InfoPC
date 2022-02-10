using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InfoPC.Helpers;
using Microsoft.Xaml.Behaviors;

namespace InfoPC.Behaviors
{
    public class MouseEnterBehaviors : Behavior<Grid>
    {
        private Point _mousePositionOnMainWindowX;
        private int _heightWindows;
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
            _mousePositionOnMainWindowX = Application.Current.MainWindow.PointFromScreen(new Point(0, 0));
            _heightWindows = (int)Application.Current.MainWindow.Height;
            Application.Current.MainWindow.Visibility = Visibility.Collapsed;
            Task.Run(async () => await ShowWindow());
        }
        private async Task ShowWindow()
        {
            while (true)
            {
                await Task.Delay(500);
                var _mousePositionOnScreen = MouseWindowsHelper.GetMousePosition();
                if ( _heightWindows < _mousePositionOnScreen.Y  || !(_mousePositionOnMainWindowX.X + _mousePositionOnScreen.X > 0))
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
