using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayerUI
{
    public partial class Form2 : Form
    {
        Dictionary<string, List<SongInfo>> map;
        public Form2()
        {
            InitializeComponent();
            map = new Dictionary<string, List<SongInfo>>();
            treeAllocation();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Parent != null)
                deleteSong(map[treeView1.SelectedNode.Parent.Text][treeView1.SelectedNode.Index]);
            else
            {
                deleteAlbum(treeView1.SelectedNode.Text);
            }
            
        }
        public void UploadMultipart(byte[] file, string filename, string contentType, string url)
        {
            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(file);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"files\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData(url, "POST", nfile);
        }
        public async void updateDatabase(SongInfo songInfo)
        {
            using (var client = new HttpClient())
            {
                // This would be the like http://www.uber.com
                client.BaseAddress = new Uri("Base Address/URL Address");

                var json = JsonConvert.SerializeObject(songInfo, Formatting.Indented);

                // serialize your json using newtonsoft json serializer then add it to the StringContent
                var content = new StringContent(json, Encoding.UTF8, "application/json");

    // method address would be like api/callUber:SomePort for example
                var result = await client.PostAsync("Method Address", content);
                string resultContent = await result.Content.ReadAsStringAsync();
            }
        }
        public async Task<List<SongInfo>> getAlbumSongs(string album)
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/songs/" + album);
    
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<SongInfo>));
            List<SongInfo> feed = (List<SongInfo>)ser.ReadObject(respStream);
        
            return feed;
        }
        public async Task<List<AlbumInfo>> getAlbumsAsync()
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/albums");
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<AlbumInfo>));
            List<AlbumInfo> feed = (List<AlbumInfo>)ser.ReadObject(respStream);
         
            return feed;
        }
       


        private async void  button3_ClickAsync(object sender, EventArgs e)
        {
            
            
        }
        private async void treeAllocation()
        {
            deleteNode();
            List<AlbumInfo> albumList = await getAlbumsAsync();
          map = new Dictionary<string, List<SongInfo>>();
            foreach (AlbumInfo s in albumList)
            {
                map.Add(s.albumName, await getAlbumSongs(s.albumName));
                TreeNode root = new TreeNode(s.albumName);
                foreach (SongInfo info in map[s.albumName])
                    root.Nodes.Add(info.songName);
                AddNodeToTreeView(root);
              
    
            }
        }
        private async void deleteSong(SongInfo songInfo)
        {
            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(songInfo, Formatting.Indented);
            HttpRequestMessage request = new HttpRequestMessage
            {
               
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("http://localhost:8080/api/songs/delete")
            };
            await client.SendAsync(request).ContinueWith((s) => treeAllocation());
           

        }
        private async void deleteAlbum(string albumInfo)
        {
            var client = new HttpClient();
           
            HttpRequestMessage request = new HttpRequestMessage
            {

                Content = new StringContent(albumInfo, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("http://localhost:8080/api/album/delete")
            };
            await client.SendAsync(request).ContinueWith((s) => treeAllocation());
           

           
        }
        public delegate void AddNodeToTreeViewDelegate(TreeNode value);
        private void AddNodeToTreeView(TreeNode value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddNodeToTreeViewDelegate(AddNodeToTreeView), value);
            }
            else
            {
                treeView1.Nodes.Add(value);
            }


        }
        public delegate void deleteNodeDelgate();
        private void deleteNode()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new deleteNodeDelgate(deleteNode));
            }
            else
            {
                treeView1.Nodes.Clear();
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
