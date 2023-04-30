using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messanger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            ConnectWindow connectWindow = new ConnectWindow();
            connectWindow.Show();
        }
    }
}
