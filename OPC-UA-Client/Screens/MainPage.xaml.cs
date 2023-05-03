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
        private OPCServer opcServer = OPCServer.Instance;
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private ObservableString snackbarMessage = new ObservableString();
        private DispatcherTimer _timer;

        public MainPage()
        {
            InitializeComponent();

            ProcessQueueAsync();
            snackbar.DataContext = snackbarMessage;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            liveTextBox.Text = opcServer.ReadTag("2:Tag99");
        }

        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcServer.connected)
            {
                snackbarPopup("No Server Connected");
                return;
            }

            if (!string.IsNullOrEmpty(writeTextBox.Text))
            {
                _queue.Enqueue(writeTextBox.Text);
            }
            else
            {
                snackbarPopup("Invalid Value");
            }
            
            writeTextBox.Text = null;
        }

        private void readButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcServer.connected)
            {
                snackbarPopup("No Server Connected");
                return;
            }

            readTextBox.Text = opcServer.ReadTag(opcServer.RWTag);
        }

        private async void ProcessQueueAsync()
        {
            while (true)
            {
                if (_queue.TryDequeue(out string value))
                {
                    await Task.Run(() =>
                    {
                        if(opcServer.RWTag != null)
                        {
                            opcServer.WriteValue(value);
                            Dispatcher.Invoke(() =>
                            {
                                snackbarPopup($"{value} written to {opcServer.RWTag}");

                            });
                        }
                        else
                        {
                            
                        }
                        
                    });
                }
                await Task.Delay(100);
            }
        }

        private async void snackbarPopup(string message)
        {
            snackbarMessage.Value = message;

            snackbar.IsActive = true;
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
            });
            snackbar.IsActive = false;
        }

        private void liveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcServer.connected)
            {
                snackbarPopup("No Server Connected");
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
