using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace UdpMulticastChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> messages = new ObservableCollection<string>();
        private readonly string nameAsNumber = new Random().Next(100, 1000).ToString();

        //private UdpClient client;
        //private readonly int port = new Random().Next(8000, 65300);

        public MainWindow()
        {
            InitializeComponent();

            chatBox.ItemsSource = messages;

            //PrepareUdpClient();
        }

        //private void PrepareUdpClient()
        //{
        //    client = new UdpClient(8001);
        //}

        private async void OnReady(object sender, RoutedEventArgs e)
        {
            var recieverUdpClient = new UdpClient(8001);

            try
            {
                recieverUdpClient.JoinMulticastGroup(IPAddress.Parse("225.1.10.8"), 10);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            while (true)
            {
                var result = await recieverUdpClient.ReceiveAsync();
                messages.Add(Encoding.UTF8.GetString(result.Buffer));
            }

            //var message = Encoding.UTF8.GetString(result.Buffer);

            //messages.Add(message);
        }

        private async void SendMessage(object sender, RoutedEventArgs e)
        {
            var senderUdpClient = new UdpClient();

            if (!string.IsNullOrEmpty(messageTextBox.Text))
            {
                var datagrams = Encoding.UTF8.GetBytes($"{nameAsNumber} говорит: {messageTextBox.Text}");
                await senderUdpClient.SendAsync(datagrams, datagrams.Length, new IPEndPoint(IPAddress.Parse("225.1.10.8"), 8001));
            }

            senderUdpClient.Close();
        }
    }
}
