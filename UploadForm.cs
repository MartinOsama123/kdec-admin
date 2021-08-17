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
            if (dialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = dialog1.FileName;
            }
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = String.Join(" ", dialog.FileNames);
            }
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dialog.FileNames.Length; i++)
            {
                byte[] buffer = File.ReadAllBytes(dialog.FileNames[i]);
                UploadMultipartMP3(buffer, dialog.SafeFileNames[i], "form-data", "https://kdechurch.herokuapp.com/church/upload");
                uploadDatabase(new SongInfo(comboBox2.Text, comboBox1.Text, dialog.SafeFileNames[i]));
            }
            byte[] buffer1 = File.ReadAllBytes(dialog1.FileName);
            UploadMultipartImage(buffer1, dialog1.SafeFileName, "form-data", "https://kdechurch.herokuapp.com/api/upload/img/" + comboBox2.Text);
            MessageBox.Show("File Uploaded Successfully.");
            textBox1.Clear();
            pictureBox1.Image = pictureBox1.InitialImage;
        }
        public void UploadMultipartMP3(byte[] file, string filename, string contentType, string url)
        {
            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(file);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"files\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData(url, "POST", nfile);
        }
        public void UploadMultipartImage(byte[] file, string filename, string contentType, string url)
        {
            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(file);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"image\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData(url, "POST", nfile);
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
    }
}
