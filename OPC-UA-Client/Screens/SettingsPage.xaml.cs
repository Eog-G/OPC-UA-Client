using MaterialDesignThemes.Wpf;
using OPC_UA_Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace OPC_UA_Client.Screens
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private OPCServer opcServer = OPCServer.Instance;
        private MainWindow mainWindow;

        public SettingsPage()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;

            if(opcServer.connected)
            {
                connectionStatusIcon.Foreground = Brushes.Green;
                connectButton.IsEnabled = false;
                disconnectButton.IsEnabled = true;
            }
            else
            {
                connectionStatusIcon.Foreground= Brushes.Red;
                connectButton.IsEnabled = true;
                disconnectButton.IsEnabled = false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get a reference to the parent window
            mainWindow = Window.GetWindow(this) as MainWindow;

            // Unsubscribe from the loaded event
            Loaded -= UserControl_Loaded;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if(endpointTextBox.Text.Length > 0)
            {
                opcServer.EndpointURL = endpointTextBox.Text;
                    try
                    {
                        opcServer.Connect();
                        connectionStatusIcon.Foreground = Brushes.Green;
                        connectButton.IsEnabled = false;
                        disconnectButton.IsEnabled = true;
                    }
                    catch
                    {
                        mainWindow.snackbarPopup("Failed to connect to OPC UA server");
                    }
            }
            else
            {
                mainWindow.snackbarPopup("Please enter an OPC UA server endpoint URL");
            }
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            opcServer.Disconnect();
            connectionStatusIcon.Foreground = Brushes.Red;
            connectButton.IsEnabled = true;
            disconnectButton.IsEnabled = false;
        }

        private void writeTagTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            opcServer.RWTag = writeTagTextBox.Text;
            writeTagDataTypeTextBox.Text = opcServer.RWTagType;
        }
    }
}
