using System;
using System.Collections.Generic;
using CYBERSECURITY_AWARENESS.Models;

namespace CYBERSECURITY_AWARENESS.Services
{
	public class LogService
	{
		private List<LogEntry> _logs = new List<LogEntry>();
		private int _nextId = 1;

		public void AddEntry(string action, string details)
		{
			_logs.Insert(0, new LogEntry
			{
				Id = _nextId++,
				Action = action,
				Details = details,
				Timestamp = DateTime.Now
			});
		}

		public string GetLogSummary(int count = 10)
		{
			if (_logs.Count == 0)
				return "No activities recorded yet. Try adding a task or starting the quiz!";

			string result = "Here's a summary of recent actions:\n";
			int itemsToShow = Math.Min(count, _logs.Count);
			for (int i = 0; i < itemsToShow; i++)
			{
				var log = _logs[i];
				result += $"{i + 1}. {log.Action}: {log.Details} ({log.Timestamp:yyyy-MM-dd HH:mm})\n";
			}
			return result;
		}
	}
}