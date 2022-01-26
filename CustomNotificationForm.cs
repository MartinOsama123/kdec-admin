using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace admin_panel
{
    public partial class CustomNotificationForm : Form
    {
        public CustomNotificationForm()
        {
            InitializeComponent();
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            sendNotification(textBox1.Text, textBox2.Text, "Global");
        }
        private void sendNotification(string title,string body, string channel)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://kdechurch.herokuapp.com/api/send-notification");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            Note note = new Note(title, body, channel);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(note, Formatting.Indented);

                System.Diagnostics.Debug.WriteLine(json);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }


        }
    }
}
