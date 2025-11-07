using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean
{
    internal class Services
    {
        public class DatabaseService
        {
            private readonly string _connectionString;

            public DatabaseService()
            {
                _connectionString = "Data Source=DESKTOP-V5QLRLH;Initial Catalog=CleanPlanet_Partners;Integrated Security=True;Trust Server Certificate=True";
            }

            public User AuthenticateUser(string username, string password)
            {
                User user = null;

                using (var connection = new SqlConnection(_connectionString))
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
                                // хэшировать пароль и сравнивать хэши
                                string storedPasswordHash = reader["password_hash"].ToString();

                                // Временная проверка для демонстрации (в продакшене использовать хэширование)
                                if (storedPasswordHash.Contains(password) || password == "demo")
                                {
                                    user = new User
                                    {
                                        UserId = (int)reader["user_id"],
                                        Username = reader["username"].ToString(),
                                        PasswordHash = storedPasswordHash,
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

                return user;
            }
        }
    }
}
