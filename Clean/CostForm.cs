using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using Clean;

namespace CleanPlanetApp.Forms
{
    public partial class ServiceCostForm : Form
    {
        public ServiceCostForm()
        {
            InitializeComponent();
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
        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу для расчета");
                return;
            }

            dynamic selectedService = comboBox1.SelectedItem;
            int serviceId = Convert.ToInt32(selectedService.Id);
            CalculateServiceCost(serviceId);
        }


        private void CalculateServiceCost(int serviceId)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    // Получаем трудозатраты услуги
                    string laborQuery = "SELECT labor_cost_hours FROM services WHERE service_id = @ServiceId";
                    decimal laborHours = 0;
                    using (var cmd = new SqlCommand(laborQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        laborHours = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    // Расчет стоимости материалов
                    string materialsQuery = @"
                        SELECT SUM(sm.material_consumption_per_unit * m.cost_per_unit * (1 + sm.waste_percentage/100))
                        FROM service_materials sm
                        INNER JOIN materials m ON sm.material_id = m.material_id
                        WHERE sm.service_id = @ServiceId";

                    decimal materialCost = 0;
                    using (var cmd = new SqlCommand(materialsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        var result = cmd.ExecuteScalar();
                        if (result != DBNull.Value)
                            materialCost = Convert.ToDecimal(result);
                    }

                    // Расчет трудозатрат (предположим ставка 500 руб/час)
                    decimal hourlyRate = 500;
                    decimal laborCost = laborHours * hourlyRate;

                    // Итоговая себестоимость
                    decimal totalCost = materialCost + laborCost;

                    // Обновляем Labels
                    label3.Text = $"{laborCost:0.00} руб.";
                    label4.Text = $"{materialCost:0.00} руб.";
                    label5.Text = $"{totalCost:0.00} руб.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка расчета: " + ex.Message);
            }
        }

        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;TrustServerCertificate=True";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Managers managers = new Managers();
            this.Close();
            managers.ShowDialog();
        }
    }
}