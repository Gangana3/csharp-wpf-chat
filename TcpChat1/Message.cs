using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TcpChat1
{
    class Message
    {
        private enum MessageParameter { username, friendIP, port, friendPort, timeFormed }; // All the parameters that are added to the message

        private string content;
        private User sender;
        private string timeFormed;    //"hour:minutes" format (according to UTC)

        public Message(string content, User sender)
        {
            this.content = content;
            this.sender = sender;
            var now = DateTime.UtcNow;

            string hour = ""; string minute = "";
            if (now.Hour < 10) hour = "0" + now.Hour.ToString();
            else hour = now.Hour.ToString();
            if (now.Minute < 10) minute = "0" + now.Minute.ToString();
            else minute = now.Minute.ToString();

            this.timeFormed = string.Format("{0}:{1}", hour, minute);
        }

        public Message(string rawMessage)
        {
            this.content = GetMessageContent(rawMessage);
            this.sender = new User(
                username: GetParameterFromMessage(rawMessage, MessageParameter.username),
                friendIP: GetParameterFromMessage(rawMessage, MessageParameter.friendIP),
                port: int.Parse(GetParameterFromMessage(rawMessage, MessageParameter.port)),
                friendPort: int.Parse(GetParameterFromMessage(rawMessage, MessageParameter.friendPort))
                );
            this.timeFormed = GetParameterFromMessage(rawMessage, MessageParameter.timeFormed);
        }


        public string Content
        {
            get { return this.content; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("message ontent cannot be empty!");
                else
                    this.content = value;
            }
        }

        public User Sender
        {
            get { return this.sender; }
            set
            {
                if (value == null)
                    throw new Exception("Message sender cannot be assigned to null");
                else
                    this.sender = value;
            }
        }


        public string TimeFormed
        {
            get
            {
                var localCurrentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
                var timeDifferences = localCurrentTime.Hour - DateTime.UtcNow.Hour;     // Time difference between UTC and Local Time Zone

                string[] split = this.timeFormed.Split(':');

                string hours = split[0];
                string minutes = split[1];

                if (int.Parse(hours) < 10) hours = "0" + hours;
                if (int.Parse(minutes) < 10) minutes = "0" + minutes;

                return string.Format("{0}:{1}", int.Parse(hours) + timeDifferences, minutes);
            }
        }


        public int Length
        {
            get
            {
                return this.ToString().Length;
            }
        }


        /// <summary>
        /// Returns a specific parameter from message parameters
        /// </summary>
        /// <param name="rawMessage">A complete message including the parameters</param>
        /// <param name="param">parameter to get</param>
        /// <returns>a specific parameter from message parameters</returns>
        private static string GetParameterFromMessage(string rawMessage, MessageParameter param)
        {
            string regexPattern = "";
            switch (param)
            {
                case MessageParameter.friendIP:
                    regexPattern = @".+friend_ip=(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})&";
                    break;

                case MessageParameter.username:
                    regexPattern = @".+username=(\w*)&";
                    break;

                case MessageParameter.port:
                    regexPattern = @".*port=(\d*)&";
                    break;

                case MessageParameter.friendPort:
                    regexPattern = @".*friend_port=(\d*)&";
                    break;

                case MessageParameter.timeFormed:
                    regexPattern = @".*time_formed=(\d{2}:\d{2})&";
                    break;
            }
            int questionMarkIndex = rawMessage.LastIndexOf('?');
            string parameters = rawMessage.Substring(questionMarkIndex);
            Match match = Regex.Match(parameters, regexPattern);
            return match.Groups[1].ToString();
        }


        /// <summary>
        /// Returns a message content
        /// </summary>
        /// <param name="rawMessage">A complete message, including the parameters</param>
        /// <returns>Message's content</returns>
        private static string GetMessageContent(string rawMessage)
        {
            int questionMarkIndex = rawMessage.LastIndexOf('?');
            return rawMessage.Substring(0, questionMarkIndex);
        }

        public override string ToString()
        {
            return string.Format("{0}?username={1}&friend_ip={2}&port={3}&friend_port={4}&time_formed={5}&", content, this.sender.Username, this.sender.FriendIP, this.sender.Port, this.sender.FriendPort, this.timeFormed);
        }
    }
}
