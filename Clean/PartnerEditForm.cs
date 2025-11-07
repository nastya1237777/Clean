using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace CleanPlanetApp.Forms
{
    public partial class PartnerEditForm : Form
    {
        private int? partnerId;
        private TextBox txtName;
        private TextBox txtDirector;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtINN;
        private NumericUpDown numRating;
        private Button btnSave;
        private Button btnCancel;

        public PartnerEditForm(int? id = null)
        {
            partnerId = id;
            this.Size = new System.Drawing.Size(400, 350);
            this.Text = partnerId.HasValue ? "Редактирование партнера" : "Добавление партнера";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            int y = 20;
            txtName = new TextBox() { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(250, 25) };
            this.Controls.Add(new Label() { Text = "Название:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(80, 25) });
            this.Controls.Add(txtName);
            y += 40;

            txtDirector = new TextBox() { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(250, 25) };
            this.Controls.Add(new Label() { Text = "Директор:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(80, 25) });
            this.Controls.Add(txtDirector);
            y += 40;

            txtPhone = new TextBox() { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(250, 25) };
            this.Controls.Add(new Label() { Text = "Телефон:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(80, 25) });
            this.Controls.Add(txtPhone);
            y += 40;

            txtEmail = new TextBox() { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(250, 25) };
            this.Controls.Add(new Label() { Text = "Email:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(80, 25) });
            this.Controls.Add(txtEmail);
            y += 40;

            txtINN = new TextBox() { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(250, 25) };
            this.Controls.Add(new Label() { Text = "ИНН:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(80, 25) });
            this.Controls.Add(txtINN);
            y += 40;

            numRating = new NumericUpDown()
            {
                Location = new System.Drawing.Point(100, y),
                Size = new System.Drawing.Size(100, 25),
                Minimum = 0,
                Maximum = 5,
                DecimalPlaces = 2
            };
            this.Controls.Add(new Label() { Text = "Рейтинг:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(80, 25) });
            this.Controls.Add(numRating);
            y += 50;

            btnSave = new Button() { Text = "Сохранить", Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(100, 35) };
            btnCancel = new Button() { Text = "Отмена", Location = new System.Drawing.Point(220, y), Size = new System.Drawing.Size(100, 35) };

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            InitializeComponent();
            if (partnerId.HasValue)
                LoadPartnerData();
        }

        private void LoadPartnerData()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query = "SELECT name, director_full_name, phone, email, inn, rating FROM partners WHERE partner_id = @PartnerId";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PartnerId", partnerId.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["name"].ToString();
                            txtDirector.Text = reader["director_full_name"].ToString();
                            txtPhone.Text = reader["phone"].ToString();
                            txtEmail.Text = reader["email"].ToString();
                            txtINN.Text = reader["inn"].ToString();
                            numRating.Value = Convert.ToDecimal(reader["rating"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtINN.Text))
            {
                MessageBox.Show("Заполните обязательные поля");
                return;
            }

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    string query;
                    SqlCommand command;

                    if (partnerId.HasValue)
                    {
                        query = @"UPDATE partners SET name=@Name, director_full_name=@Director, 
                                 phone=@Phone, email=@Email, inn=@INN, rating=@Rating 
                                 WHERE partner_id=@PartnerId";
                    }
                    else
                    {
                        query = @"INSERT INTO partners (partner_type_id, name, director_full_name, 
                                 phone, email, inn, rating, is_active) 
                                 VALUES (1, @Name, @Director, @Phone, @Email, @INN, @Rating, 1)";
                    }

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Director", txtDirector.Text ?? "");
                    command.Parameters.AddWithValue("@Phone", txtPhone.Text ?? "");
                    command.Parameters.AddWithValue("@Email", txtEmail.Text ?? "");
                    command.Parameters.AddWithValue("@INN", txtINN.Text);
                    command.Parameters.AddWithValue("@Rating", numRating.Value);

                    if (partnerId.HasValue)
                        command.Parameters.AddWithValue("@PartnerId", partnerId.Value);

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