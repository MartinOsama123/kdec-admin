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
using Newtonsoft.Json;
using System.Windows.Forms;
using admin_panel;

namespace PlayerUI
{
    public partial class UploadForm : Form
    {
        OpenFileDialog dialog;
        OpenFileDialog dialog1;
        public UploadForm()
        {
            InitializeComponent();
            populateAlbums();
            comboBox1.SelectedItem = comboBox1?.Items[0] ?? "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void browseBtn_Click(object sender, EventArgs e)
        {
            dialog = new OpenFileDialog();
            dialog.Filter = "(*.mp3)|*.mp3";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = String.Join(" ", dialog.FileNames);
            }
        }

        private async void confirmBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dialog.FileNames.Length; i++)
            {
                byte[] buffer = File.ReadAllBytes(dialog.FileNames[i]);
                await UploadMultipartMP3(buffer, dialog.SafeFileNames[i], "https://kdechurch.herokuapp.com/church/upload");
                uploadDatabase(new SongInfo(comboBox2.Text, comboBox1.Text, dialog.SafeFileNames[i]));
            }
            byte[] buffer1 = File.ReadAllBytes(dialog1.FileName);
           await UploadMultipartImage(buffer1, dialog1.SafeFileName, "https://kdechurch.herokuapp.com/api/upload/img/" + comboBox2.Text);
            MessageBox.Show("File Uploaded Successfully.");
            textBox1.Clear();
            pictureBox1.Image = pictureBox1.InitialImage;
            sendNotification(comboBox2.Text,"Global");
        }
        public async Task UploadMultipartMP3(byte[] file, string filename, string url)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new ByteArrayContent(file, 0, file.Length), "files", filename);
            HttpResponseMessage response = await httpClient.PostAsync(url, form);
            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
        }
        public async Task UploadMultipartImage(byte[] file, string filename, string url)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new ByteArrayContent(file, 0, file.Length), "image", filename);
            HttpResponseMessage response = await httpClient.PostAsync(url, form);
            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
        }
        public async void uploadDatabase(SongInfo songInfo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://kdechurch.herokuapp.com");
                var json = JsonConvert.SerializeObject(songInfo, Formatting.Indented);
         

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("/api/songs/create", content);
                string resultContent = await result.Content.ReadAsStringAsync();
            }
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
        public async void populateAlbums()
        {
            List<AlbumInfo> s = await getAlbumsAsync();
            foreach(var t in s)
            {
                comboBox2.Items.Add(t.albumName);
            }
        }
        private void sendNotification(string album, string channel)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://kdechurch.herokuapp.com/api/send-notification");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            Note note = new Note("New Album Release", album, channel);
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
