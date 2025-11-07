using CleanPlanetApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clean
{
    public partial class UprPart : Form
    {
        public UprPart()
        {
            InitializeComponent();
            LoadPartners();
        }

        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;TrustServerCertificate=True";
        }
        private void LoadPartners()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query = @"
                        SELECT p.partner_id, p.name, pt.type_name, p.director_full_name, 
                               p.phone, p.email, p.inn, p.rating, p.is_active
                        FROM partners p
                        INNER JOIN partner_types pt ON p.partner_type_id = pt.partner_type_id
                        ORDER BY p.name";

                    var adapter = new SqlDataAdapter(query, connection);
                    var table = new DataTable();
                    adapter.Fill(table);

                    dataGridView1.DataSource = table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var editForm = new PartnerEditForm();
            Visible = false;
            editForm.ShowDialog();
            Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int partnerId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["partner_id"].Value);
                var editForm = new PartnerEditForm(partnerId);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPartners();
                }
            }
            else
            {
                MessageBox.Show("Выберите партнера для редактирования");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int partnerId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["partner_id"].Value);
                string partnerName = dataGridView1.CurrentRow.Cells["name"].Value.ToString();

                if (MessageBox.Show($"Удалить партнера '{partnerName}'?", "Подтверждение",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqlConnection(GetConnectionString()))
                        {
                            connection.Open();
                            string query = "DELETE FROM partners WHERE partner_id = @PartnerId";
                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@PartnerId", partnerId);
                            command.ExecuteNonQuery();
                        }
                        LoadPartners();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите партнера для удаления");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Managers managers = new Managers();
            this.Close();
            managers.ShowDialog();
        }
    }
}
