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
    /// Interaction logic for ConnectPage.xaml
    /// </summary>
    public partial class ConnectPage : Page
    {
        public event EventHandler Connected;


        protected virtual void OnConnected()
        {
            if (this.Connected != null)
                this.Connected(this, EventArgs.Empty);
        }


        public ConnectPage(User user)
        {
            InitializeComponent();

            this.Loaded += async delegate (object sender, RoutedEventArgs e)
            {
                user.Connected += (eventSender, eventArgs) => this.OnConnected();
                await user.ConnectAsync();  // Connect to the other client asynchronously
                this.NavigationService.Navigate((new ChatPage(user)));
            };

        }

    }
}
