using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;

namespace CleanPlanetApp.Forms
{
    public partial class UserAddForm : Form
    {
        private TextBox textBoxUsername;
        private TextBox textBoxFullName;
        private ComboBox comboBoxRoles;
        private Button btnSave;
        private Button btnCancel;

        public UserAddForm()
        {
            InitializeComponent();
            this.Size = new Size(400, 300);
            this.Text = "Добавление пользователя";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;

            // Логин
            textBoxUsername = new TextBox()
            {
                Location = new Point(120, y),
                Size = new Size(200, 25)
            };
            this.Controls.Add(new Label()
            {
                Text = "Логин:",
                Location = new Point(20, y),
                Size = new Size(80, 25)
            });
            this.Controls.Add(textBoxUsername);
            y += 40;

            // ФИО
            textBoxFullName = new TextBox()
            {
                Location = new Point(120, y),
                Size = new Size(200, 25)
            };
            this.Controls.Add(new Label()
            {
                Text = "ФИО:",
                Location = new Point(20, y),
                Size = new Size(80, 25)
            });
            this.Controls.Add(textBoxFullName);
            y += 40;

            // Роль
            comboBoxRoles = new ComboBox()
            {
                Location = new Point(120, y),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(new Label()
            {
                Text = "Роль:",
                Location = new Point(20, y),
                Size = new Size(80, 25)
            });
            this.Controls.Add(comboBoxRoles);
            y += 60;

            // Кнопки
            btnSave = new Button()
            {
                Text = "Сохранить",
                Location = new Point(120, y),
                Size = new Size(100, 35)
            };
            btnCancel = new Button()
            {
                Text = "Отмена",
                Location = new Point(230, y),
                Size = new Size(100, 35)
            };

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            btnSave.Click += btnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            LoadRoles();
        }


        private void LoadRoles()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query = "SELECT role_id, role_name FROM roles ORDER BY role_name";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBoxRoles.Items.Add(new
                            {
                                Id = reader["role_id"],
                                Name = reader["role_name"].ToString()
                            });
                        }
                    }
                    comboBoxRoles.DisplayMember = "Name";
                    comboBoxRoles.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки ролей: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxFullName.Text))
            {
                MessageBox.Show("Заполните обязательные поля");
                return;
            }

            if (comboBoxRoles.SelectedValue == null)
            {
                MessageBox.Show("Выберите роль");
                return;
            }

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string query = @"INSERT INTO users (username, password_hash, full_name, 
                                 role_id, is_active) 
                                 VALUES (@Username, @PasswordHash, @FullName, @RoleId, 1)";

                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", textBoxUsername.Text);
                    command.Parameters.AddWithValue("@FullName", textBoxFullName.Text);
                    command.Parameters.AddWithValue("@RoleId", comboBoxRoles.SelectedValue);
                    command.Parameters.AddWithValue("@PasswordHash", "demo");

                    command.ExecuteNonQuery();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;TrustServerCertificate=True";
        }
    }
}