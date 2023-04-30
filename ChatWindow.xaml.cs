using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Messanger
{
    public partial class ChatWindow : Window
    {
        private TcpClient tcpClient = new TcpClient();

        public ChatWindow()
        {
            InitializeComponent();

            tcpClient.UserName = ConnectWindow.login;
            tcpClient.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.server.ConnectAsync(ConnectWindow.ip_text, 8888);

            HostWindow.users.Add(tcpClient.UserName);
            tcpClient.GetMessage(ListMesseges, UsersList);

            UsersList.ItemsSource = null;
            UsersList.ItemsSource = HostWindow.users;
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessegaBox.Text.Equals("/disconnect"))
            {
                Disconnect();
            }
            else
            {
                DateTime time = DateTime.Now;
                tcpClient.SendMessage($"[{time.ToShortTimeString()}] {tcpClient.UserName}: {MessegaBox.Text}");
                MessegaBox.Text = "";
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void Disconnect() 
        {
            tcpClient.DisconnectAsync().Wait();
            HostWindow.users.Remove(tcpClient.UserName);
            ConnectWindow connectWindow = new ConnectWindow();
            connectWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Disconnect();
        }
    }

    public class TcpClient
    {
        public Socket server;
        public string UserName;

        public async Task GetMessage(ListBox ListMesseges, ListBox UsersList)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await server.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                ListMesseges.Items.Add($"{message}");
                if (HostWindow.isClose)
                {
                    DisconnectAsync().Wait();
                }

                UsersList.ItemsSource = null;
                UsersList.ItemsSource = HostWindow.users;
            }
        }

        public async Task SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await server.SendAsync(bytes, SocketFlags.None);
        }

        public async Task DisconnectAsync()
        {
            server.Disconnect(false);
        }
    }
}
