using System;
using System.Collections.Generic;
using System.Text;

namespace admin_panel
{
    public class Note
    {
        public string title;
        public string message;
        public string topic;
        public string token;

        public Note(string title, string message, string topic)
        {
            this.title = title;
            this.message = message;
            this.topic = topic;
        }

        public Note(string title, string message, string topic, string token) : this(title, message, topic)
        {
            this.token = token;
        }
    }
}
