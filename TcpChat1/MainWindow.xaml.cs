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

namespace TcpChat1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Lock the window, Make it not resizeable
            this.ResizeMode = ResizeMode.NoResize;

            var mainPage = new MainPage();
            var connectPage = new ConnectPage();
            var chatPage = new ChatPage();

            connectPage.Connected += delegate (object sender, EventArgs e)
            {
            // When user connected to another user
            this.Dispatcher.Invoke(() => this.mainFrame.NavigationService.Navigate(chatPage));
            };

            

            chatPage.ConnectionClosed += delegate (object sender, EventArgs e)
            {
            // After the connection is closed
            this.Dispatcher.Invoke(() => this.mainFrame.Navigate(mainPage));
            };

            mainPage.Submitted += delegate (object sender, EventArgs e)
            {
            // When user submitted data successfully
            this.mainFrame.NavigationService.Navigate(connectPage);
            };

            this.mainFrame.NavigationService.Navigate(mainPage);
        }
    }
}
