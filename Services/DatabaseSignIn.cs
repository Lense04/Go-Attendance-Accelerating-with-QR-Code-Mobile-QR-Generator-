using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace UMAttendanceSystem_Mobile.Services
{
    public class DatabaseSignIn
    {
        private readonly string _connectionString;

        public DatabaseSignIn(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<(bool isValid, string studentName, string department, string studentNumber)> CheckCredentialsAsync(string studentNumber, string password)
        {
            if (string.IsNullOrWhiteSpace(studentNumber) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Student number and password cannot be empty.");
            }

            // List of available departments
            var departments = new List<string> { "BED", "CAE", "CAFAE", "CASE", "CCE", "CCJE", "CEE", "CHE", "CHSE", "CTE", "TS", "PS" };

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    foreach (var department in departments)
                    {
                        // Construct the table name dynamically
                        string tableName = $"users.Student_List_{department}";
                        string query = $"SELECT Student_Name FROM {tableName} WHERE Student_Number = @Student_Number AND Password = @Password";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Student_Number", studentNumber);
                            command.Parameters.AddWithValue("@Password", password);

                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    string studentName = reader["Student_Name"]?.ToString();
                                    return (true, studentName, department, studentNumber);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while checking credentials.", ex);
            }

            return (false, null, null, null);
        }
    }
}