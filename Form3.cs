using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayerUI
{
    public partial class Form3 : Form
    {
        OpenFileDialog dialog1;
   

        public static Label numLabel;
        public static int count = 0;
        OpenFileDialog dialog;
        public Form3()
        {
            InitializeComponent();
           
        }
        private  void Post(Session session1)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://kdechurch.herokuapp.com/api/channels/create");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(session1, Formatting.Indented);

                System.Diagnostics.Debug.WriteLine(json);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private  void button9_Click(object sender, EventArgs e)
        {
            label1.Enabled = false; label2.Enabled = false;  label4.Enabled = false;   richTextBox1.Enabled = false; button9.Enabled = false; hostName.Enabled = false; title.Enabled = false;  button9.Enabled = false; pictureBox1.Enabled = false; 
            label1.Visible = false; label2.Visible = false;  label4.Visible = false;  richTextBox1.Visible = false; button9.Visible = false; hostName.Visible = false; title.Visible = false;  button9.Visible = false; pictureBox1.Visible = false; 

            
            Post(new Session(title.Text,richTextBox1.Text,hostName.Text,"English", "https://www.radioking.com/play/my-radio97", dialog1.SafeFileName));
            var task = UploadFile("public/" + title.Text + "/" + dialog1.SafeFileName, dialog1.FileName);
            MessageBox.Show(@"You are now Live, Start talking.", @"Message", MessageBoxButtons.OK);
            endButton.Enabled = true; endButton.Visible = true;

           
        }
        private void endButton_Click(object sender, EventArgs e)
        {
            label1.Enabled = true; label2.Enabled = true;  label4.Enabled = true;   richTextBox1.Enabled = true;  button9.Enabled = true; hostName.Enabled = true; title.Enabled = true; button9.Enabled = true; pictureBox1.Enabled = true; endButton.Enabled = false; pictureBox1.Enabled = true;
            label1.Visible = true; label2.Visible = true;  label4.Visible = true;  richTextBox1.Visible = true;  button9.Visible = true; hostName.Visible = true; title.Visible = true; button9.Visible = true;  pictureBox1.Visible = false; endButton.Visible = false; pictureBox1.Visible = true;
            deleteSong(new Session(title.Text, richTextBox1.Text, hostName.Text, "English", "https://www.radioking.com/play/my-radio97", dialog1.SafeFileName));
         
        }
        public async Task UploadFile(String keyName, String filePath)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);

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
        private async void deleteSong(Session session)
        {
            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(session, Formatting.Indented);
            HttpRequestMessage request = new HttpRequestMessage
            {

                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://kdechurch.herokuapp.com/api/channels/delete")
            };
            await client.SendAsync(request);
        }
        public async void UploadMultipartImageAsync(byte[] file, string filename, string contentType, string url)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new ByteArrayContent(file, 0, file.Length), "image", filename);
            HttpResponseMessage response = await httpClient.PostAsync(url, form);
            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
        }
        private bool CheckChannelName()
        {
            var channelNameChar = title.Text.ToCharArray();
            return !(from nameChar in channelNameChar
                     where nameChar < 'a' || nameChar > 'z'
                     where nameChar < 'A' || nameChar > 'Z'
                     where nameChar < '0' || nameChar > '9'
                     let temp = new[]
                     {
                    '!', '#', '$', '%', '&', '(', ')', '+', '-', ':', ';', '<', '=', '.', '>', '?', '@', '[', ']', '^',
                    '_', '{', '}', '|', '~', ',', (char) 32
                }
                     where Array.IndexOf(temp, nameChar) < 0
                     select nameChar).Any();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            dialog1 = new OpenFileDialog();
            if (dialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = dialog1.FileName;
            }
        }
        class Session
        {

           public String channelName;
            public String description;
            public String hostName;
            public String lang;
            public String token;
            public String imgPath;

            public Session()
            {
            }

            public Session(string channelName, string description, string hostName, string lang, string token,string imgPath)
            {
                this.channelName = channelName;
                this.description = description;
                this.hostName = hostName;
                this.lang = lang;
                this.token = token;
                this.imgPath = imgPath;
            }
        }
       

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

      

        
    }
}
