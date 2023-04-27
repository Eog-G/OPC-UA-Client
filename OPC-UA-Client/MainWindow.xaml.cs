using OPC_UA_Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Opc.UaFx.Client;
using System.Threading;
using System.Collections.Concurrent;
using Opc.Ua;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using OPC_UA_Client.Screens;

namespace OPC_UA_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OPCServer opcServer = OPCServer.Instance;
        private int SelectedPageIndex = 1;
        private ObservableString snackbarMessage = new ObservableString();

        public MainWindow()
        {
            InitializeComponent();
            snackbar.DataContext = snackbarMessage;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            opcServer.Disconnect();
        }

        private void LeftNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if(Transitioner.SelectedIndex != 0) { Transitioner.SelectedIndex -= 1; }
        }

        private void RightNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if(Transitioner.SelectedIndex != 2) { Transitioner.SelectedIndex += 1; }
        }

        public async void snackbarPopup(string message)
        {
            snackbarMessage.Value = message;

            snackbar.IsActive = true;
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
            });
            snackbar.IsActive = false;
        }
    }
}
