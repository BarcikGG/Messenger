using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Messanger
{
    public partial class HostWindow : Window
    {
        private TcpServer tcpServer = new();
        private TcpClient tcpClient = new();

        public static List<UserInfo> logs = new();
        public static List<string> users = new();
        public static List<string> logsprint = new();

        public static bool isClose = false;
        public static bool isConnected;
        private bool isLogActive = false;

        public HostWindow()
        {
            InitializeComponent();
            isConnected = true;

            tcpClient.UserName = ConnectWindow.login;
            IPEndPoint IP_point = new IPEndPoint(IPAddress.Any, 8888);
            tcpServer.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServer.socket.Bind(IP_point);
            tcpServer.socket.Listen(1000);

            tcpClient.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.server.ConnectAsync("127.0.0.1", 8888);
            users.Add(tcpClient.UserName);

            ListMesseges.Items.Add("\t\t" + DateTime.Now.ToLongDateString());
            tcpServer.ListenClients(ListMesseges, UsersList);
        }



        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime time = DateTime.Now;
            tcpClient.SendMessage($"[{time.ToShortTimeString()}] {tcpClient.UserName}: {MessegaBox.Text}");
            MessegaBox.Text = "";
        }

        private void SwitchUsersBtn_Click(object sender, RoutedEventArgs e)
        {
            isLogActive = !isLogActive;

            if (isLogActive)
            {
                UsersLog.Visibility = Visibility.Visible;
                UsersLog.ItemsSource = null;
                logsprint.Clear();
                foreach (var item in logs)
                {
                    logsprint.Add(item.ConnectionTime.ToString()+ "\n"+ item.Name);
                }
                UsersLog.ItemsSource = logsprint;

                UsersList.Visibility = Visibility.Hidden;
            }
            else 
            {
                UsersLog.Visibility = Visibility.Hidden;
                UsersList.Visibility = Visibility.Visible;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect() 
        {
            var clientsCopy = tcpServer.clients.ToList();
            tcpClient.SendMessage($"/close/");
            isClose = true;
            foreach (var client in clientsCopy)
            {
                tcpServer.DisconnectClient(client);
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(ChatWindow))
                {
                    window.Close();
                }
            }

            tcpServer.socket.Close();

            users.Clear();
            isConnected = false;
            ConnectWindow connectWindow = new ConnectWindow();
            connectWindow.Show();
            this.Close();
        }
    }

    public class UserInfo
    {
        public DateTime ConnectionTime { get; set; }
        public string Name { get; set; }

        public UserInfo(DateTime connectTime, string name) 
        {
            ConnectionTime = connectTime;
            Name = $"new user: [{name}]";
        }
    }

    class TcpServer
    {
        public Socket socket;
        public List<Socket> clients = new List<Socket>();
        private TcpClient tcpClient = new();

        public async Task ListenClients(ListBox ListMesseges, ListBox UsersList)
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                clients.Add(client);
                HostWindow.logs.Add(new UserInfo(DateTime.Now, HostWindow.users.Last()));

                GetMessage(client, ListMesseges, UsersList);

                UsersList.ItemsSource = null;
                UsersList.ItemsSource = HostWindow.users;
            }
        }

        private async Task GetMessage(Socket client, ListBox ListMesseges, ListBox UsersList)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                int bytesRecieved = await client.ReceiveAsync(bytes, SocketFlags.None);

                if (bytesRecieved == 0)
                {
                    clients.Remove(client);
                    HostWindow.users.Remove(tcpClient.UserName);
                    
                    ListMesseges.Items.Add($"Client {client.RemoteEndPoint} ({tcpClient.UserName}) disconnected.");

                    UsersList.ItemsSource = null;
                    UsersList.ItemsSource = HostWindow.users;
                    break;
                }

                string message = Encoding.UTF8.GetString(bytes, 0, bytesRecieved);
                ListMesseges.Items.Add($"[{client.RemoteEndPoint}]: \n{message}");

                foreach (var item in clients)
                {
                    if (item != client)
                    {
                        SendMessage(item, message);
                    }
                }
            }
        }

        private async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }

        public void DisconnectClient(Socket client)
        {
            if (clients.Contains(client))
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                clients.Remove(client);      

                foreach (var item in clients)
                {
                    SendMessage(item, $"User '{tcpClient.UserName}' has left the chat.");
                }

                HostWindow.users.Remove(tcpClient.UserName);
            }
        }

    }
}
