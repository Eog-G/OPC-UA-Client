using Microsoft.Xaml.Behaviors.Layout;
using Opc.Ua;
using OPC_UA_Client.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OPC_UA_Client.Screens
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        private OPCClient opcClient = OPCClient.Instance;
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private DispatcherTimer _timer;
        private MainWindow mainWindow;

        public MainPage()
        {
            InitializeComponent();

            ProcessQueueAsync();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get a reference to the parent window
            mainWindow = Window.GetWindow(this) as MainWindow;

            // Unsubscribe from the Loaded event
            Loaded -= UserControl_Loaded;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            liveTextBox.Text = opcClient.ReadTag("2:Tag99");
        }

        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcClient.connected)
            {
                mainWindow.snackbarPopup("No Server Connected");
                return;
            }
            if (opcClient.RWTagType == "null")
            {
                mainWindow.snackbarPopup("Invalid Tag ID");
                return;
            }

            if (!string.IsNullOrEmpty(writeTextBox.Text))
            {
                _queue.Enqueue(writeTextBox.Text);
            }
            else
            {
                mainWindow.snackbarPopup("Error: Empty Value");
            }
            
            writeTextBox.Text = null;
        }

        private void readButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcClient.connected)
            {
                mainWindow.snackbarPopup("No Server Connected");
                return;
            }

            readTextBox.Text = opcClient.ReadTag(opcClient.RWTag);
        }

        private async void ProcessQueueAsync()
        {
            while (true)
            {
                if (_queue.TryDequeue(out string value))
                {
                    await Task.Run(() =>
                    {
                        OPCReturnCode returnCode = opcClient.WriteValue(value);
                        Dispatcher.Invoke(() =>
                        {
                            if (returnCode.Code.Item1 == 0)
                            {
                                mainWindow.snackbarPopup($"{value} Written to {opcClient.RWTag}");
                            }
                            else
                            {
                                mainWindow.snackbarPopup($"Invalid {returnCode.correctType} Value");
                            }
                           
                            
                        });
                    });
                }
                await Task.Delay(100);
            }
        }

        private void liveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcClient.connected)
            {
                mainWindow.snackbarPopup("No Server Connected");
                return;
            }

            ToggleButton toggleButton = sender as ToggleButton;

            if ((bool)toggleButton.IsChecked)
            {
                _timer.Stop();
                liveButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                liveButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7BC74D"));
                liveButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7BC74D"));
            }
            else
            {
                _timer.Start();
                liveButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                liveButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5F"));
                liveButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5F"));
            }
        }
    }
}
