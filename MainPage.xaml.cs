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
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace TcpChat1
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public event EventHandler Submitted;

        
        protected virtual void OnSubmitted()
        {
            if (this.Submitted != null)
                this.Submitted(this, EventArgs.Empty);
        }


        public MainPage()
        {
            InitializeComponent();

            this.KeyDown += delegate (object sender, KeyEventArgs e)
            {
                // When the enter key is pressed, Submit the data
                if (e.Key == Key.Enter)
                    this.submitButton_Click(sender, new RoutedEventArgs());
            };
        }

        /// <summary>
        /// Makes sure that the details given are valid. 
        /// Prompts a message if one of the fields is invalid
        /// </summary>
        /// <returns>Whether the given details are valid or not</returns>
        private bool ValidateFields()
        {
            // Validate username
            if (this.username.Text == string.Empty)
            {
                MessageBox.Show("Username cannot be empty!");
                return false;
            }
            if (this.username.Text.ToUpper() == "YOU")
            {
                MessageBox.Show("Username cannot be 'YOU'!");
                return false;
            }
            if (!Regex.IsMatch(this.username.Text, @"\w*"))
            {
                MessageBox.Show("Username can contain letters, numbers and underscore ONLY!");
                return false;
            }

            // Validate IP
            var regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            if (!regex.Match(this.friendIP.Text).Success)
            {
                MessageBox.Show("The ip must be in this format: 'number.number.number.number'");
                return false;
            }
            foreach (string byteString in this.friendIP.Text.Split('.'))
                if (int.Parse(byteString) > 255)
                {
                    MessageBox.Show("Invalid Ip given!");
                    return false;
                }

            // Validate port
            const int MaxPort = UInt16.MaxValue - 1;
            const int MinPort = 1024;
            // Port validation function
            Func<string, bool> ValidatePort = delegate (string portString)
            {
                int port = 0;
                if (!int.TryParse(portString, out port))
                {
                    MessageBox.Show("The port must be numeric only!");
                    return false;
                }
                if (port > MaxPort || port < MinPort)
                {
                    MessageBox.Show("The port must be between 1024 and 65535!");
                    return false;
                }

                return true;
            };
            if (!ValidatePort(this.friendPort.Text)) return false;  // Validate friend's port
            if (!ValidatePort(this.myPort.Text)) return false;      // Validate my port
            // Make sure that the given port is open
            if (!IsPortOpen(int.Parse(this.myPort.Text)))
            {
                MessageBox.Show("The port you've given is taken by another process. choose another port!");
                return false;
            }

            // In case passed all validation
            return true;
        }


        /// <summary>
        /// Determines whether the port is open or not
        /// </summary>
        /// <param name="port"></param>
        /// <returns>Whether the port is open or not</returns>
        private static bool IsPortOpen(int port)
        {
            var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            try
            {
                tcpListener.Start();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            finally
            {
                tcpListener.Stop();
            }
        }


        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidateFields())
            {
                // In case all Fields are valid
                var user = new User(
                    username: this.username.Text,
                    friendIP: this.friendIP.Text,
                    port: int.Parse(this.myPort.Text),
                    friendPort: int.Parse(this.friendPort.Text)
                    );

                GlobalData.user = user;
                this.OnSubmitted();
            }           
        }
    }
}
