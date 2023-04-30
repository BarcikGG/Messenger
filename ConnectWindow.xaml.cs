using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Messanger
{
    public partial class ConnectWindow : Window
    {
        private string IP = "127.0.0.1";
        public static string ip_text;
        public static string login;
        public ConnectWindow()
        {
            InitializeComponent();
            ConnectServerBtn.IsEnabled = false;
        }

        private void IPBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConnectServerBtn.IsEnabled = true;
        }

        private void CreateServerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoginBox.Text.Length > 0)
                {
                    login = LoginBox.Text;
                    HostWindow hostWindow = new HostWindow();
                    hostWindow.Show();
                    this.Close();
                }
                else MessageBox.Show("Введите никнейм");
            }
            catch 
            {
                MessageBox.Show("Ошибка, возможно сервер уже запущен");
            }
        }

        private void ConnectServerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoginBox.Text.Length > 0 & IPBox.Text == IP)
                {
                    login = LoginBox.Text;
                    ip_text = IPBox.Text;
                    ChatWindow chatWindow = new ChatWindow();
                    chatWindow.Show();
                    this.Close();
                }
                else if (LoginBox.Text.Length == 0)
                {
                    MessageBox.Show("Введите никнейм");
                }
                else
                {
                    MessageBox.Show("Неверный IP адрес");
                }
            }
            catch 
            {
                MessageBox.Show("Ошибка, возможно сервер не запущен");
            }
        }
    }
}
