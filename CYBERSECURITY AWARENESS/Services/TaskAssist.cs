using System;
using CYBERSECURITY_AWARENESS.Models;

namespace CYBERSECURITY_AWARENESS.Services
{
	public class TaskAssist
	{
		private Database? _db;
		private LogService _log;
		private bool _dbAvailable;

		public TaskAssist(Database db, LogService log)
		{
			_db = db;
			_log = log;
			_dbAvailable = db != null && db.IsAvailable();
		}

		public string AddTask(string title, string description, string reminderDays = null)
		{
			if (!_dbAvailable)
				return "Database is not available. Cannot add tasks. Please check your MySQL connection.";

			string reminderDate = null;
			if (!string.IsNullOrEmpty(reminderDays) && int.TryParse(reminderDays, out int days))
			{
				reminderDate = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
			}

			
			_db!.AddTask(title, description, reminderDate);
			_log.AddEntry("Task Added", $"Added task: {title}");

			
			return reminderDate != null
				? $"Task '{title}' added successfully! I'll remind you on {reminderDate}."
				: $"Task '{title}' added successfully!";
		}

		public string GetTasksList()
		{
			if (!_dbAvailable)
				return "Database is not available. Cannot retrieve tasks.";

			var tasks = _db.GetTasks(false);
			if (tasks.Count == 0)
				return "You have no pending tasks. Add a task like 'Add task - Enable 2FA'";

			string result = "Your pending tasks:\n";
			foreach (var task in tasks)
			{
				result += $"• {task.Title}";
				if (!string.IsNullOrEmpty(task.ReminderDate))
					result += $" (Reminder: {task.ReminderDate})";
				result += "\n";
			}
			return result;
		}

		public string CompleteTask(string taskTitle)
		{
			if (!_dbAvailable)
				return "Database is not available. Cannot complete tasks.";

			var tasks = _db.GetTasks(false);
			foreach (var task in tasks)
			{
				if (task.Title.ToLower().Contains(taskTitle.ToLower()))
				{
					_db.UpdateTaskStatus(task.Id, true);
					_log.AddEntry("Task Completed", $"Completed task: {task.Title}");
					return $"Marked '{task.Title}' as completed! Great job staying on top of your cybersecurity!";
				}
			}
			return $"Could not find task containing '{taskTitle}'. Type 'show tasks' to see your pending tasks.";
		}

		public string DeleteTask(string taskTitle)
		{
			if (!_dbAvailable)
				return "Database is not available. Cannot delete tasks.";

			var tasks = _db.GetTasks(true);
			foreach (var task in tasks)
			{
				if (task.Title.ToLower().Contains(taskTitle.ToLower()))
				{
					_db.DeleteTask(task.Id);
					_log.AddEntry("Task Deleted", $"Deleted task: {task.Title}");
					return $"Deleted task '{task.Title}'.";
				}
			}
			return $"Could not find task containing '{taskTitle}'.";
		}
	}
}