using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admin_panel
{
    public partial class UsersForm : Form
    {
        public UsersForm()
        {
            InitializeComponent();
            getUsersAsync();
        }
        public async Task<List<UserModel>> getUsersAsync()
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/albums");
            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<UserModel>));
            List<UserModel> feed = (List<UserModel>)ser.ReadObject(respStream);

            return feed;
        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
