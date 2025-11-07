using Clean;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CleanPlanetApp.Forms
{
    public partial class PartnerForm : Form
    {
        private DataGridView dataGridViewPartner;
        private string directorName;

        public PartnerForm(string userFullName)
        {
            directorName = userFullName;
            InitializeComponent();
            LoadPartnerData();
            LoadServices();
        }

        private void LoadServices()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query = "SELECT service_id, service_name FROM services WHERE is_active = 1";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(new
                            {
                                Id = reader["service_id"],
                                Name = reader["service_name"].ToString()
                            });
                        }
                    }
                    comboBox1.DisplayMember = "Name";
                    comboBox1.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки услуг: " + ex.Message);
            }
        }

        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;TrustServerCertificate=True";
        }

        private void LoadPartnerData()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    // Ищем партнера по director_full_name
                    string query = @"
                        SELECT 
                            p.name as 'Название',
                            pt.type_name as 'Тип партнера', 
                            p.director_full_name as 'Директор',
                            p.address as 'Адрес',
                            p.phone as 'Телефон',
                            p.email as 'Email',
                            p.inn as 'ИНН',
                            p.rating as 'Рейтинг',
                            CASE WHEN p.is_active = 1 THEN 'Да' ELSE 'Нет' END as 'Активен'
                        FROM partners p
                        INNER JOIN partner_types pt ON p.partner_type_id = pt.partner_type_id
                        WHERE p.director_full_name = @DirectorName";

                    var adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@DirectorName", directorName);

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
            Form1 form1 = new Form1();
            this.Close();
            form1.ShowDialog();
        }
    }
}