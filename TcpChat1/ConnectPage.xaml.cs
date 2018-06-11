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
using System.Threading;


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


        public ConnectPage()
        {
            InitializeComponent();

            this.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                GlobalData.user.Connected += (eventSender, eventArgs) => this.OnConnected();
                var connectThread = new Thread(GlobalData.user.Connect);
                connectThread.IsBackground = true;
                connectThread.Start();
            };
        }

    }
}
