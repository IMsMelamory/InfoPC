using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;



namespace InfoPC.Behaviors
{
    public class MouseEnterBehaviors: Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseEnter += OnClick;
            AssociatedObject.MouseLeave += OnClick1;
            base.OnAttached();

        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeave -= OnClick1;
            AssociatedObject.MouseEnter -= OnClick;
            base.OnDetaching();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {

            if (Mouse.GetPosition(Application.Current.MainWindow).X > 0)
            {
                Application.Current.MainWindow.Opacity = 0;
            }

            
        }
        private void OnClick1(object sender, RoutedEventArgs e)
        {

            if (Mouse.GetPosition(Application.Current.MainWindow).X < 0)
            {
                Application.Current.MainWindow.Opacity = 1;
            }


        }

    }
}
