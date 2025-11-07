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
using static System.Collections.Specialized.BitVector32;

namespace Clean
{
    public partial class Form1 : Form
    {
        private int userId;

        public Form1()
        {
            InitializeComponent();
        }
        // Статический класс для сессии
        public static class Session
        {
            public static User CurrentUser { get; set; }
            public static bool IsLoggedIn => CurrentUser != null;
        }
        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;TrustServerCertificate=True";
        }

        private User AuthenticateUser(string username, string password)
        {
            User user = null;

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query = @"
                        SELECT u.user_id, u.username, u.password_hash, u.full_name, 
                               u.role_id, r.role_name, u.is_active
                        FROM users u
                        INNER JOIN roles r ON u.role_id = r.role_id
                        WHERE u.username = @Username AND u.is_active = 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPasswordHash = reader["password_hash"].ToString();

                                // Временная проверка пароля (в реальном приложении используй хэширование)
                                if (ValidatePassword(password, storedPasswordHash))
                                {
                                    user = new User
                                    {
                                        UserId = (int)reader["user_id"],
                                        Username = reader["username"].ToString(),
                                        FullName = reader["full_name"].ToString(),
                                        RoleId = (int)reader["role_id"],
                                        RoleName = reader["role_name"].ToString(),
                                        IsActive = (bool)reader["is_active"]
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return user;
        }

        // Временный метод проверки пароля (замени на реальное хэширование)
        private bool ValidatePassword(string inputPassword, string storedPasswordHash)
        {
            // Для демонстрации - простой вариант
            // В реальном приложении используй: BCrypt.Net.BCrypt.Verify(inputPassword, storedPasswordHash)
            return storedPasswordHash.Contains(inputPassword) ||
                   inputPassword == "demo" ||
                   storedPasswordHash == inputPassword;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;

            button1.Enabled = false;

            try
            {
                // Аутентификация пользователя
                var user = AuthenticateUser(username, password);

                if (user != null)
                {
                    // Создание сессии пользователя
                    Session.CurrentUser = user;

                    // Сообщение об успешном входе
                    MessageBox.Show($"Добро пожаловать, {user.FullName}!\nРоль: {user.RoleName}",
                                  "Успешный вход", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (user.RoleName == "Manager")
                    {
                        Form managerForm = new Managers();
                        this.Hide();
                        managerForm.Show();
                    }

                    if (user.RoleName == "Partner")
                    {
                        Form partForm = new PartnerForm(user.FullName);
                        this.Hide();
                        partForm.Show();
                    }

                    if (user.RoleName == "Admin")
                    {
                        Form adm = new AdminUsersForm();
                        this.Hide();
                        adm.Show();
                    }

                }
                else
                {
                    label3.Visible = true;
                    textBox2.Clear();
                    textBox2.Focus();
                    return;
                }
            }
            finally
            {
                // Восстановить состояние UI
                Cursor = Cursors.Default;
                button1.Enabled = true;
            }
        }

        
        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Вход по нажатию Enter в поле пароля
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                button1_Click(sender, e);
            }
        }
        
        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Вход по нажатию Enter в поле логина
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                textBox2.Focus();
            }
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            // Установить фокус на поле логина при загрузке
            textBox1.Focus();
        }  
    }
}
