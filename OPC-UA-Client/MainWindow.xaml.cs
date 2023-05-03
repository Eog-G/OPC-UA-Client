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

        private void Window_Closed(object sender, EventArgs e)
        {
            opcServer.Disconnect();
        }

        private void LeftNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Transitioner.SelectedIndex != 0) { Transitioner.SelectedIndex -= 1; }
            if (Transitioner.SelectedIndex == 0) 
            { 
                LeftNavigationButton.Visibility = Visibility.Hidden;
                RightNavigationButton.Visibility = Visibility.Visible;
            }
            else 
            {
                LeftNavigationButton.Visibility = Visibility.Visible;
                RightNavigationButton.Visibility = Visibility.Visible;
            }
        }

        private void RightNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Transitioner.SelectedIndex != 2) { Transitioner.SelectedIndex += 1; }
            if (Transitioner.SelectedIndex == 2) 
            { 
                RightNavigationButton.Visibility = Visibility.Hidden;
                LeftNavigationButton.Visibility = Visibility.Visible;
            }
            else 
            { 
                RightNavigationButton.Visibility = Visibility.Visible;
                LeftNavigationButton.Visibility = Visibility.Visible;
            }
        }
    }
}
