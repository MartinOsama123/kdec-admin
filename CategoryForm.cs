using System;
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
using Newtonsoft.Json;

namespace admin_panel
{
    public partial class CategoryForm : Form
    {
        List<Category> cat;
        public CategoryForm()
        {

            InitializeComponent();
            populateCategories();
        }
        public async void createCategory(Category category)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://kdechurch.herokuapp.com");
                var json = JsonConvert.SerializeObject(category, Formatting.Indented);


                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("/api/category/add", content);
                string resultContent = await result.Content.ReadAsStringAsync();
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
        public async void populateCategories()
        {

            label1.Visible = false; textBox1.Visible = false; button1.Visible = false;
            cat = await getCategories();
            label1.Visible = true; textBox1.Visible = true; button1.Visible = true; 


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string value = textBox1.Text.ToString().Trim();
            var found = cat.Where(item => item.categoryTitle.ToLower() == value.ToLower()).FirstOrDefault();
          
            if (found == null && value != "")
            {
                createCategory(new Category(value));
                MessageBox.Show("Category Added Successfully");
            }
            else MessageBox.Show("Category Already Exists");
        }
    }
}
