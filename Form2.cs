using admin_panel;
using Amazon.S3;
using Amazon.S3.Model;
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
        List<Category> categories;
        public Form2()
        {
            InitializeComponent();

            populateCategories();
               map = new Dictionary<string, List<SongInfo>>();
               //treeAllocation();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private  void button4_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text != "")
                deleteSong(map[comboBox2.Text][comboBox3.SelectedIndex]);
            /*if (treeView1.SelectedNode.Parent != null)
            {
                int index = treeView1.SelectedNode.Index;
       
                string parent = treeView1.SelectedNode.Parent.Text;
                 deleteSong(map[treeView1.SelectedNode.Parent.Text][treeView1.SelectedNode.Index]);
           
            }
            else
            {
                deleteAlbum(treeView1.SelectedNode.Text);
            }*/
            
        }
        public async void populateAlbums(String name)
        {
            List<AlbumInfo> s = await getAlbumsAsync(name);
            comboBox2.Text = "";
            foreach (var t in s)
            {
                comboBox2.Items.Add(t.albumName);
            }
            if (comboBox2.Items.Count != 0)
            {
                comboBox2.SelectedIndex = 0;
            }
        }
        public async void populateSongs(String name)
        {
           
            comboBox3.Text = "";
            comboBox3.Items.Clear();
            foreach (var t in map[name])
            {

                comboBox3.Items.Add(t.songName.Replace(".mp3",""));
            }
            if (comboBox3.Items.Count != 0)
            {
                comboBox3.SelectedIndex = 0;
            }
        }
        public async void populateCategories()
        {
            comboBox1.Text = "Loading...";
            categories = await getCategories();
            comboBox1.Text = "";
            foreach (var t in categories)
            {

                comboBox1.Items.Add(t.categoryTitle);
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        public async Task<List<Category>> getCategories()
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/category");
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Category>));
            List<Category> feed = (List<Category>)ser.ReadObject(respStream);

            return feed;
        }
        public async Task<List<AlbumInfo>> getAlbumsAsync(String name)
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/albums/category/" + name);
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<AlbumInfo>));
            List<AlbumInfo> feed = (List<AlbumInfo>)ser.ReadObject(respStream);
            map = new Dictionary<string, List<SongInfo>>();
            foreach (AlbumInfo s in feed)
            {
                map.Add(s.albumName, await getAlbumSongs(s.albumName));
            }
            return feed;
        }

      /*  public async void updateDatabase(SongInfo songInfo)
        {
            using (var client = new HttpClient())
            {
               
                client.BaseAddress = new Uri("Base Address/URL Address");

                var json = JsonConvert.SerializeObject(songInfo, Formatting.Indented);

                // serialize your json using newtonsoft json serializer then add it to the StringContent
                var content = new StringContent(json, Encoding.UTF8, "application/json");

    // method address would be like api/callUber:SomePort for example
                var result = await client.PostAsync("Method Address", content);
                string resultContent = await result.Content.ReadAsStringAsync();
            }
        }*/
        public async Task<List<SongInfo>> getAlbumSongs(string album)
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/songs/" + album);
    
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<SongInfo>));
            List<SongInfo> feed = (List<SongInfo>)ser.ReadObject(respStream);
        
            return feed;
        }
     
       


        private async void  button3_ClickAsync(object sender, EventArgs e)
        {
            
            
        }
        /* private async void treeAllocation()
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
         }*/
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "Loading...";
            if (comboBox1.Text != "")
            {
                populateAlbums(comboBox1.Text);
            }
        }

        private async void deleteSong(SongInfo songInfo)
        {
            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(songInfo, Formatting.Indented);
            MessageBox.Show(json);
            HttpRequestMessage request = new HttpRequestMessage
            {
               
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://kdechurch.herokuapp.com/api/songs/delete")
            };
            await client.SendAsync(request);
            await deleteFile("public/" + songInfo.albumName + "/" + songInfo.songName);
            comboBox3.Items.Clear();
            comboBox3.Text = "Loading...";
            if (comboBox2.Text != "")
            {
                populateSongs(comboBox2.Text);
            }

            if (comboBox3.Items.Count != 0)
            {
                comboBox3.SelectedIndex = 0;
            }

        }
        public async Task deleteFile(String keyName)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);
        
            try
            {

                DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                {
                    BucketName = "churchappb6ce25bec93d49ac8ba023a0f6d7fb6b114009-dev",
                    Key = keyName,
                };

                DeleteObjectResponse response = await client.DeleteObjectAsync(deleteRequest);

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }
        /*private async void deleteAlbum(string albumInfo)
        {
            var client = new HttpClient();
           
            HttpRequestMessage request = new HttpRequestMessage
            {

                Content = new StringContent(albumInfo, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://kdechurch.herokuapp.com/api/album/delete")
            };
           *//* await client.SendAsync(request).ContinueWith((s) => treeAllocation());*//*
           

           
        }*/
        /*   public delegate void AddNodeToTreeViewDelegate(TreeNode value);
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


           }*/


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Text = "Loading...";
            if (comboBox2.Text != "")
            {
                populateSongs(comboBox2.Text);
            }
     
            if (comboBox3.Items.Count != 0)
            {
                comboBox3.SelectedIndex = 0;
            }
        }
    }
}
