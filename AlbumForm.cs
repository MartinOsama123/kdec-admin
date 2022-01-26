﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using PlayerUI;

namespace admin_panel
{
    public partial class AlbumForm : Form
    {
        OpenFileDialog dialog1;
        List<Category> categories;
        List<AlbumInfo> albums;
        public AlbumForm()
        {
            InitializeComponent();
            populateCategories();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
                dialog1 = new OpenFileDialog();
                dialog1.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                if (dialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.ImageLocation = dialog1.FileName;
                }
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            textBox1.Visible = false; label1.Visible = false; pictureBox1.Visible = false; button1.Visible = false;
            label3.Text = "Please wait...";
            string albumName = textBox1.Text;
            string lang = comboBox1.Text;
            AlbumInfo albumInfo = albums.Where(item => item.albumName == albumName).FirstOrDefault();
        
            if (albumInfo == null)
            {
                if (dialog1 != null && dialog1.CheckPathExists && albumName != "" && lang != "")
                {
                   
                   
                       await  UploadFile("public/" + albumName + "/" + albumName + Path.GetExtension(dialog1.FileName), dialog1.FileName);
          
              
                   await uploadAlbumImgDatabase(new AlbumInfo(albumName, lang, albumName + Path.GetExtension(dialog1.FileName)));
               
                    MessageBox.Show("Album Added Successfully");
                    pictureBox1.Image = pictureBox1.InitialImage;
                    textBox1.Text = "";
                }
                else
                {
                    MessageBox.Show("Please fill all the requirements");
                }
            }else
            {
                MessageBox.Show("Album Already Exists");
            }
            label3.Visible = false;
            textBox1.Visible = true; label1.Visible = true; pictureBox1.Visible = true; button1.Visible = true;
         
        }
        public async void populateCategories()
        {
            label3.Visible = true;
            textBox1.Visible = false; label1.Visible = false; pictureBox1.Visible = false; button1.Visible = false;
            label3.Text = "Loading...";
            categories = await getCategories();
            foreach (var t in categories)
            {

                comboBox1.Items.Add(t.categoryTitle);
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Visible = true;
            textBox1.Visible = false;  label1.Visible = false; pictureBox1.Visible = false; button1.Visible = false;
            label3.Text = "Loading...";
            if (comboBox1.Text != "")
            {
                albums = await getAlbumsAsync(comboBox1.Text);
            }
            label3.Visible = false;
            textBox1.Visible = true;  label1.Visible = true; pictureBox1.Visible = true; button1.Visible = true;
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

            return feed;
        }
        public async Task uploadAlbumImgDatabase(AlbumInfo albumInfo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://kdechurch.herokuapp.com");
                var json = JsonConvert.SerializeObject(albumInfo, Formatting.Indented);


                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine(content);
                var result = await client.PostAsync("/api/upload/img", content);
                string resultContent = await result.Content.ReadAsStringAsync();
            }
        }
        public async Task UploadFile(String keyName, String filePath)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);
            Console.WriteLine("yeaa");
       
            try
            {

                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = "churchappb6ce25bec93d49ac8ba023a0f6d7fb6b114009-dev",
                    Key = keyName,
                    FilePath = filePath,

                };

                PutObjectResponse response = await client.PutObjectAsync(putRequest);

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    MessageBox.Show("Check the provided AWS Credentials.");
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    MessageBox.Show(" amazonS3Exception.Message");
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }

     
    }
}
