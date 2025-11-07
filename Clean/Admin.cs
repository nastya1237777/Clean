using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using Clean;

namespace CleanPlanetApp.Forms
{
    public partial class AdminUsersForm : Form
    {
        public AdminUsersForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query = @"
                        SELECT u.user_id, u.username, u.full_name, r.role_name, u.is_active
                        FROM users u
                        INNER JOIN roles r ON u.role_id = r.role_id
                        ORDER BY u.username";

                    var adapter = new SqlDataAdapter(query, connection);
                    var table = new DataTable();
                    adapter.Fill(table);

                    dataGridView1.DataSource = table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки пользователей: " + ex.Message);
            }
        }

        
        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;TrustServerCertificate=True";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UserAddForm userAdd = new UserAddForm();
            userAdd.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int userId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["user_id"].Value);
                string username = dataGridView1.CurrentRow.Cells["username"].Value.ToString();

                if (MessageBox.Show($"Удалить пользователя '{username}'?", "Подтверждение",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqlConnection(GetConnectionString()))
                        {
                            connection.Open();
                            string query = "DELETE FROM users WHERE user_id = @UserId";
                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.ExecuteNonQuery();
                        }
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            this.Hide();
            form1.ShowDialog();
        }
    }
}