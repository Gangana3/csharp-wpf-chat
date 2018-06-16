using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TcpChat1
{
    [Serializable]
    public class Message
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

        public override string ToString()
        {
            return this.Content;
        }
    }
}
