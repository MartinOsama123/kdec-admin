using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using admin_panel;
using Amazon.S3;
using Amazon.S3.Model;

namespace PlayerUI
{
    public partial class UploadForm : Form
    {
        OpenFileDialog dialog;
        OpenFileDialog dialog1;
        string albumName, authorName, lang;
        List<Category> categories;


        public UploadForm()
        {
            InitializeComponent();
       
            progressBar1.Visible = false;
            label3.Visible = false;
             populateCategories();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "Loading...";
            if(comboBox1.Text != "")
            {
                populateAlbums(comboBox1.Text);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

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
        public async Task UploadFile(String keyName,String filePath)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);
            Console.WriteLine("yeaa");
            System.Diagnostics.Debug.WriteLine(albumName + Path.GetExtension(dialog1.FileName));
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
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }


        private async void confirmBtn_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = dialog.FileNames.Length;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            label3.Visible = true;
            
            textBox1.Visible = false;  label1.Visible = false; label2.Visible = false;  confirmBtn.Visible = false; browseBtn.Visible = false; comboBox1.Visible = false; comboBox2.Visible = false;

            albumName = comboBox2.Text;
            lang = comboBox1.Text;
            authorName = "";
             var task =  Task.Run(() => backgroundWorker1.RunWorkerAsync());
          


        }
    /*    public async Task UploadMultipartMP3(byte[] file, string filename, string url)
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
        }*/
        public async Task uploadDatabase(SongInfo songInfo)
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
       
      /*  public async Task uploadAlbumImgDatabase(AlbumInfo albumInfo)
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
        }*/
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
            var uri = new Uri("https://kdechurch.herokuapp.com/api/albums/category/"+name);
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<AlbumInfo>));
            List<AlbumInfo> feed = (List<AlbumInfo>)ser.ReadObject(respStream);

            return feed;
        }
     

        public async void populateAlbums(String name)
        {
            List<AlbumInfo> s = await getAlbumsAsync(name);
            comboBox2.Text = "";
            foreach(var t in s)
            {
                comboBox2.Items.Add(t.albumName);
            }
            if(comboBox2.Items.Count != 0)
            {
                comboBox2.SelectedIndex = 0;
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
            if(comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            this.Invoke(new Action(() => {
                textBox1.Visible = true;  label1.Visible = true; label2.Visible = true;   confirmBtn.Visible = true; browseBtn.Visible = true; comboBox1.Visible = true; comboBox2.Visible = true;
                progressBar1.Visible = false;
                label3.Visible = false;
                textBox1.Clear();
              
                sendNotification(comboBox2.Text, "Global");
            }));
           
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;
            /*if (!comboBox1.Items.Contains(lang))
            {
                createCategory(new Category(lang));
            }*/
            for (int i = 0; i < dialog.FileNames.Length; i++)
            {
                this.Invoke(new Action(() => { label3.Text = String.Format("Files Uploaded ({0} / {1}), Please wait...", i, dialog.FileNames.Length); }));
                var task1 = UploadFile("public/" + albumName + "/" + dialog.SafeFileNames[i], dialog.FileNames[i]);
                task1.Wait();
                var task2 = uploadDatabase(new SongInfo(albumName, lang, dialog.SafeFileNames[i], "", 0));
                task2.Wait();
                backgroundWorker.ReportProgress(i+1);
            }
        /*  if (dialog1 != null && dialog1.CheckPathExists)
            {
               
                var task = UploadFile("public/"+albumName + "/" + albumName+ Path.GetExtension(dialog1.FileName), dialog1.FileName);
                task.Wait();
                var task3 = uploadAlbumImgDatabase(new AlbumInfo(albumName, lang, albumName + Path.GetExtension(dialog1.FileName)));
                task3.Wait();
            }*/
          
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Invoke(new Action(() => progressBar1.Value = e.ProgressPercentage));
        }
    }
}
