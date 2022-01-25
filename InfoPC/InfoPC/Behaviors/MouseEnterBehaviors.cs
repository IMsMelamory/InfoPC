using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Drawing;
using System.Runtime.InteropServices;
using System;

namespace InfoPC.Behaviors
{
    public class MouseEnterBehaviors : Behavior<Grid>
    {
        protected override void OnAttached()
        {
            Application.Current.MainWindow.MouseEnter += OnMouseEnter;
            Application.Current.MainWindow.MouseLeave += OnMouseLeave;

            base.OnAttached();
        }
        protected override void OnDetaching()
        {

            Application.Current.MainWindow.MouseEnter -= OnMouseEnter;
            Application.Current.MainWindow.MouseLeave -= OnMouseLeave;
            base.OnDetaching();
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e)
        {

            AssociatedObject.Opacity = 0;

        }
        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Opacity = 1;
        }

    }
}
