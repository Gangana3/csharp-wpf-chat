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

            //User user = null;
            //ConnectPage connectPage = null;
            //ChatPage chatPage = null;

            //mainPage.Submitted += delegate (object sender, EventArgs e)
            //{
            //    // When user submitted data successfully
            //    this.mainFrame.NavigationService.Navigate(connectPage);
            //    user = mainPage.User;
            //};

            //connectPage = new ConnectPage(user);
            //chatPage = new ChatPage(user);

            //connectPage.Connected += delegate (object sender, EventArgs e)
            //{
            //// When user connected to another user
            //this.Dispatcher.Invoke(() => this.mainFrame.NavigationService.Navigate(chatPage));
            //};

            

            //chatPage.ConnectionClosed += delegate (object sender, EventArgs e)
            //{
            //// After the connection is closed
            //this.Dispatcher.Invoke(() => this.mainFrame.Navigate(mainPage));
            //};



            this.mainFrame.NavigationService.Navigate(mainPage);
        }
    }
}
