using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CYBERSECURITY_AWARENESS.Models;
using TaskModel = CYBERSECURITY_AWARENESS.Models.Task;

namespace CYBERSECURITY_AWARENESS.Services
{
	/// <summary>
	/// MySQL database operations for task storage.
	
	public class Database
	{
		
		private string connectionString = "Server=localhost;Database=cyberbot;Uid=root;Pwd=Madiba21!@Hunadi;";
		private bool _isConnected = false;

		public Database()
		{
			try
			{
				InitializeDatabase();
				_isConnected = true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Database init failed: {ex.Message}");
				_isConnected = false;
			
			}
		}

		private void InitializeDatabase()
		{
			using (var connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				string createTable = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Title VARCHAR(255) NOT NULL,
                        Description TEXT,
                        ReminderDate VARCHAR(50),
                        IsCompleted BOOLEAN DEFAULT FALSE,
                        CreatedAt DATETIME NOT NULL
                    )";
				using (var command = new MySqlCommand(createTable, connection))
				{
					command.ExecuteNonQuery();
				}
			}
		}


		public bool IsAvailable() => _isConnected;

	
		public bool TestConnection() => _isConnected;


		public void AddTask(string title, string description, string reminderDate = null)
		{
			if (!_isConnected) throw new Exception("Database not available.");
			using (var connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				string sql = @"INSERT INTO Tasks (Title, Description, ReminderDate, CreatedAt) 
                              VALUES (@title, @desc, @reminder, @created)";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@title", title);
					command.Parameters.AddWithValue("@desc", description ?? "");
					command.Parameters.AddWithValue("@reminder", reminderDate ?? "");
					command.Parameters.AddWithValue("@created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					command.ExecuteNonQuery();
				}
			}
		}

		public List<TaskModel> GetTasks(bool includeCompleted = false)
		{
			var tasks = new List<TaskModel>();
			if (!_isConnected) return tasks; // Return empty if DB not available

			using (var connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				string sql = includeCompleted
					? "SELECT * FROM Tasks ORDER BY IsCompleted, CreatedAt DESC"
					: "SELECT * FROM Tasks WHERE IsCompleted = FALSE ORDER BY CreatedAt DESC";
				using (var command = new MySqlCommand(sql, connection))
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						tasks.Add(new TaskModel
						{
							Id = reader.GetInt32("Id"),
							Title = reader.GetString("Title"),
							Description = reader.IsDBNull(reader.GetOrdinal("Description"))
								? "" : reader.GetString("Description"),
							ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate"))
								? null : reader.GetString("ReminderDate"),
							IsCompleted = reader.GetBoolean("IsCompleted"),
							CreatedAt = reader.GetString("CreatedAt")
						});
					}
				}
			}
			return tasks;
		}

		public void UpdateTaskStatus(int taskId, bool isCompleted)
		{
			if (!_isConnected) throw new Exception("Database not available.");
			using (var connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				string sql = "UPDATE Tasks SET IsCompleted = @completed WHERE Id = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@completed", isCompleted);
					command.Parameters.AddWithValue("@id", taskId);
					command.ExecuteNonQuery();
				}
			}
		}

		public void DeleteTask(int taskId)
		{
			if (!_isConnected) throw new Exception("Database not available.");
			using (var connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				string sql = "DELETE FROM Tasks WHERE Id = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", taskId);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}