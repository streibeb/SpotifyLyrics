using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using SpotifyAPI.SpotifyLocalAPI;
using SpotifyLyrics.Models;
using SpotifyLyrics.ViewModels;
using SpotifyEventHandler = SpotifyAPI.SpotifyLocalAPI.SpotifyEventHandler;

namespace SpotifyLyrics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel _mvm;

        public MainWindow()
        {
            InitializeComponent();

            _mvm = (MainViewModel) this.DataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _mvm.LoadData();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                RestoreButton.Content = "2";
                WindowState = WindowState.Maximized;
            }
            else if (WindowState == WindowState.Maximized)
            {
                RestoreButton.Content = "1";
                WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _mvm.Close();
        }
    }
}
