using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        List<UserModel> users;
        public UsersForm()
        {
            InitializeComponent();
            listView1.Scrollable = true;
            listView1.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Text = "Names";
          
            header.Name = "col1";
            header.Width = 30;
            listView1.Columns.Add(header);
            listView1.MultiSelect = false;
       

        }
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (StringFormat sf = new StringFormat())
            {
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                // Draw the standard header background.
                e.DrawBackground();

                // Draw the header text.
                using (Font headerFont = new Font("Helvetica", 25, FontStyle.Bold)) //Font size!!!!
                {
                    e.Graphics.DrawString(e.Header.Text, headerFont, Brushes.Black, e.Bounds, sf);
                }
            }
            return;
            
        }


        /*  private async Task InitializeAsync()
          {
              List<UserModel> users =  await getMessages();
              foreach(var user in users)
              {
                  ListViewItem listViewItem = new ListViewItem();
                  Debug.WriteLine(user.name);
                  listViewItem.Text = user.name;
                  listView1.Items.Add(listViewItem);
              }
          }

          public static async Task<UsersForm> Create()
          {
              var ret = new UsersForm();
              await ret.InitializeAsync();
              return ret;
          }*/
        /*    public UsersForm()
            {
                InitializeComponent();
                listView1.Scrollable = true;
                listView1.View = View.Details;
                ColumnHeader header = new ColumnHeader();
                header.Text = "";
                header.Name = "col1";
                header.Width = listView1.Width;
               // getUsersAsync();

                for (int i = 0; i < 20; i++)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Text = "hi";




                    // attach event handler for Click event 
                    // (assuming ButtonClickHandler is an existing method in the class)

                    listView1.Items.Add(listViewItem);

                }

            }*/

        public void btn_msg(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
           
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            foreach (string s in users[listView1.SelectedIndices[0]].messages)
            {
                richTextBox1.AppendText(s);
                richTextBox1.AppendText("\n\n-----------------------------\n\n");
               
            }
            
            
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public async Task<List<UserModel>> getMessages()
        {
            var client = new HttpClient();
            var uri = new Uri("https://kdechurch.herokuapp.com/api/users/messages");

            Stream respStream = await client.GetStreamAsync(uri);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<UserModel>));
            List<UserModel> feed = (List<UserModel>)ser.ReadObject(respStream);

            return feed;
        }

        private async void UsersForm_Load_1(object sender, EventArgs e)
        {
            users = await getMessages();
            foreach (var user in users)
            {
                ListViewItem listViewItem = new ListViewItem();

                listViewItem.Text = user.name;
                Debug.WriteLine(listViewItem.Text);
                listView1.Items.Add(listViewItem);
              

            }

        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
