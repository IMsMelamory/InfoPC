﻿using System.Windows;
using System.Drawing;
using System;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;

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
            Top = SystemParameters.WorkArea.Height - Height;

        }
    }
    }

