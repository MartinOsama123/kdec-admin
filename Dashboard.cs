using PlayerUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admin_panel
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            listView1.Scrollable = true;
            listView1.View = View.Details;
            listView2.Scrollable = true;
            listView2.View = View.Details;
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            string count = await getUsersCount();
            label2.Text = count;
            List<SongInfo> songList = await getAllSongs();
            foreach(SongInfo s in songList)
            {
                listView2.Items.Add(s.songName).SubItems.Add(s.views.ToString());
            }
            songList = songList.OrderByDescending(x => x.views).ToList();
            for (int i = 0;i<songList.Count;i++)
            {
                if (i == 10) break;
                listView1.Items.Add(songList[i].songName).SubItems.Add(songList[i].views.ToString());
            }
        }
        public async Task<string> getUsersCount()
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/users/count");
            Stream respStream = await client.GetStreamAsync(uri);
            Debug.WriteLine("repStream");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(string));
            string feed = (string)ser.ReadObject(respStream);

            return feed;
        }
      
        public async Task<List<SongInfo>> getAllSongs()
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/songs/all");

            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<SongInfo>));
            List<SongInfo> feed = (List<SongInfo>)ser.ReadObject(respStream);

            return feed;
        }
    }
}
