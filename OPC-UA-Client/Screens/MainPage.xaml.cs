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
        private ConcurrentQueue<short> _queue = new ConcurrentQueue<short>();
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
            liveTextBox.Text = opcServer.ReadTag99(1);
        }

        private async void writeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcServer.connected)
            {
                snackbarPopup("No Server Connected");
                return;
            }

            TextBox textBox = (TextBox)this.FindName("writeTextBox");

            if (short.TryParse(textBox.Text, out short result))
            {

                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _queue.Enqueue(result);
                    });
                });
            }
            else
            {
                snackbarPopup("Invalid Int16 value");
            }
            textBox.Text = null;
        }

        private void readButton_Click(object sender, RoutedEventArgs e)
        {
            if (!opcServer.connected)
            {
                snackbarPopup("No Server Connected");
                return;
            } 
            readTextBox.Text = opcServer.testValue;
        }

        private async void ProcessQueueAsync()
        {
            while (true)
            {
                if (_queue.TryDequeue(out short value))
                {
                    await Task.Run(() =>
                    {
                        opcServer.WriteValue("2:Tag1", Convert.ToInt16(value));
                        Dispatcher.Invoke(() =>
                        {
                            snackbarPopup($"{value} written to {opcServer.testValue}");
                            
                        });
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
                liveButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2AB53F"));
                liveButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2AB53F"));
            }
            else
            {
                _timer.Start();
                liveButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                liveButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDD2C00"));
                liveButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDD2C00"));
            }
        }
    }
}
