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
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        private static readonly object lockObj = new object();


        public event EventHandler ConnectionClosed;   // After connection is closed


        protected virtual void OnConnectionClosed()
        {
            if (this.ConnectionClosed != null)
                this.ConnectionClosed(this, EventArgs.Empty);
        }


        public ChatPage()
        {
            InitializeComponent();


            RoutedEventHandler ReceiveMessages = delegate (object sender, RoutedEventArgs e)
            {
                // Clear the chat box from previous conversations
                this.chatTextBox.Text = "";

                // Change the header
                this.header.Content = string.Format("Connected to {0}", GlobalData.user.FriendIP);

                // Receive messages in a different thread
                var receiveMessagesThread = new Thread(
                    delegate ()
                    {
                        var messageSound = new System.Media.SoundPlayer(@"Resources/message.wav");
                        try
                        { 
                            while (true)
                            {
                                Message message = GlobalData.user.Receive();  // Receive message
                                
                                // Print the received message and play a message sound
                                this.AppendChatTextBox(string.Format("[{0}] {1}: {2}\n", message.TimeFormed, message.Sender.Username, message.Content));
                                messageSound.Play();
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("The other user has left the chatroom!");
                                GlobalData.user.CloseConnection();
                                this.OnConnectionClosed();

                            });
                        }
                    });
                receiveMessagesThread.IsBackground = true;
                receiveMessagesThread.Start();
            };

            this.Loaded += ReceiveMessages;     // When the page is loaded, start receiving messages

            // When a key is down
            this.KeyDown += delegate (object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    // When the enter key is down, press the send button
                    this.sendButton_Click(sender, new RoutedEventArgs());
                }
            };
        }


        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.messageTextBox.Text))
            {
                MessageBox.Show("Message cannot be empty!");
            }
            else
            {
                // Send the message
                GlobalData.user.Send(this.messageTextBox.Text);

                // Print the message
                var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
                int minute = time.Minute;
                int hour = time.Hour;
                string minuteString, hourString;
                if (minute < 10)
                    minuteString = "0" + minute.ToString();
                else
                    minuteString = minute.ToString();
                if (hour < 10)
                    hourString = "0" + minute.ToString();
                else
                    hourString = hour.ToString();
                this.AppendChatTextBox(string.Format("[{0}:{1}] YOU: {2}\n", time.Hour, time.Minute, this.messageTextBox.Text));

                // Empty the text box
                this.messageTextBox.Text = string.Empty;
            }
        }


        /// <summary>
        /// Appends the Chat Text block on the page with the given message
        /// </summary>
        /// <param name="rawMessage">Message to print to the text block</param>
        private void AppendChatTextBox(string rawMessage)
        {            
            this.Dispatcher.Invoke(() => { this.chatTextBox.Text += rawMessage; this.scrollBar.ScrollToEnd();  });            
        }
    }
}
