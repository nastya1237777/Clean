using Clean;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace CleanPlanetApp.Services
{
    public class PartnerService
    {
        private readonly string _connectionString;

        public PartnerService()
        {
            _connectionString = "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;Trust Server Certificate=True";
        }

        public List<Partners> GetAllPartners()
        {
            var partners = new List<Partners>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT p.partner_id, p.partner_type_id, pt.type_name as partner_type_name,
                           p.name, p.director_full_name, p.address, p.phone, p.email,
                           p.inn, p.rating, p.is_active
                    FROM partners p
                    INNER JOIN partner_types pt ON p.partner_type_id = pt.partner_type_id
                    ORDER BY p.name";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        partners.Add(new Partners
                        {
                            PartnerId = (int)reader["partner_id"],
                            PartnerTypeId = (int)reader["partner_type_id"],
                            Name = reader["name"].ToString(),
                            DirectorFullName = reader["director_full_name"].ToString(),
                            Address = reader["address"].ToString(),
                            Phone = reader["phone"].ToString(),
                            Email = reader["email"].ToString(),
                            Inn = reader["inn"].ToString(),
                            Rating = (decimal)reader["rating"],
                            IsActive = (bool)reader["is_active"]
                        });
                    }
                }
            }

            return partners;
        }

        public List<PartnerType> GetPartnerTypes()
        {
            var types = new List<PartnerType>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT partner_type_id, type_name FROM partner_types ORDER BY type_name";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(new PartnerType
                        {
                            PartnerTypeId = (int)reader["partner_type_id"],
                            TypeName = reader["type_name"].ToString()
                        });
                    }
                }
            }

            return types;
        }

        public bool AddPartner(Partners partner)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO partners (partner_type_id, name, director_full_name, address, 
                                        phone, email, inn, rating, is_active)
                    VALUES (@PartnerTypeId, @Name, @DirectorFullName, @Address, 
                           @Phone, @Email, @Inn, @Rating, @IsActive)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PartnerTypeId", partner.PartnerTypeId);
                    command.Parameters.AddWithValue("@Name", partner.Name);
                    command.Parameters.AddWithValue("@DirectorFullName", partner.DirectorFullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", partner.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", partner.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", partner.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Inn", partner.Inn);
                    command.Parameters.AddWithValue("@Rating", partner.Rating);
                    command.Parameters.AddWithValue("@IsActive", partner.IsActive);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdatePartner(Partners partner)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE partners 
                    SET partner_type_id = @PartnerTypeId,
                        name = @Name,
                        director_full_name = @DirectorFullName,
                        address = @Address,
                        phone = @Phone,
                        email = @Email,
                        inn = @Inn,
                        rating = @Rating,
                        is_active = @IsActive
                    WHERE partner_id = @PartnerId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PartnerId", partner.PartnerId);
                    command.Parameters.AddWithValue("@PartnerTypeId", partner.PartnerTypeId);
                    command.Parameters.AddWithValue("@Name", partner.Name);
                    command.Parameters.AddWithValue("@DirectorFullName", partner.DirectorFullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", partner.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", partner.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", partner.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Inn", partner.Inn);
                    command.Parameters.AddWithValue("@Rating", partner.Rating);
                    command.Parameters.AddWithValue("@IsActive", partner.IsActive);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeletePartner(int partnerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Проверяем, есть ли связанные заказы
                string checkOrdersQuery = "SELECT COUNT(*) FROM orders WHERE partner_id = @PartnerId";
                using (var checkCommand = new SqlCommand(checkOrdersQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@PartnerId", partnerId);
                    int orderCount = (int)checkCommand.ExecuteScalar();

                    if (orderCount > 0)
                    {
                        throw new InvalidOperationException("Невозможно удалить партнера, так как с ним связаны заказы.");
                    }
                }

                // Удаляем партнера
                string deleteQuery = "DELETE FROM partners WHERE partner_id = @PartnerId";
                using (var command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@PartnerId", partnerId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }

    public class PartnerType
    {
        public int PartnerTypeId { get; set; }
        public string TypeName { get; set; }
    }
}
