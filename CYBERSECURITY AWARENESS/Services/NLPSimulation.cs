using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CYBERSECURITY_AWARENESS.Services
{
	public class NLPSimulation
	{
		private Dictionary<string, string[]> _intentPatterns;

		public NLPSimulation()
		{
			InitializeIntentPatterns();
		}

		private void InitializeIntentPatterns()
		{
			_intentPatterns = new Dictionary<string, string[]>
			{
				["add_task"] = new[] { @"add task", @"create task", @"new task", @"add a task" },
				["complete_task"] = new[] { @"complete task", @"finish task", @"mark task as done", @"task done" },
				["delete_task"] = new[] { @"delete task", @"remove task", @"erase task" },
				["show_tasks"] = new[] { @"show tasks", @"list tasks", @"my tasks", @"pending tasks" },
				["set_reminder"] = new[] { @"remind me", @"set reminder", @"reminder for" },
				["start_quiz"] = new[] { @"start quiz", @"begin quiz", @"take quiz", @"play quiz" },
				["show_log"] = new[] { @"activity log", @"show log", @"what have you done", @"recent actions" },
				["update_interest"] = new[] { @"update interest", @"new interest", @"change interest" }
			};
		}

		public string DetectIntent(string input)
		{
			string lowerInput = input.ToLower();
			foreach (var intent in _intentPatterns)
			{
				foreach (string pattern in intent.Value)
				{
					if (Regex.IsMatch(lowerInput, pattern, RegexOptions.IgnoreCase))
					{
						return intent.Key;
					}
				}
			}
			return "unknown";
		}

		public string ExtractTaskTitle(string input)
		{
			string[] prefixes = { "add task", "create task", "new task", "add a task" };
			foreach (string prefix in prefixes)
			{
				if (input.ToLower().StartsWith(prefix))
				{
					string title = input.Substring(prefix.Length).Trim();
					if (title.ToLower().Contains("remind me"))
					{
						int remindIndex = title.ToLower().IndexOf("remind me");
						title = title.Substring(0, remindIndex).Trim();
					}
					return title;
				}
			}
			return input;
		}

		public string ExtractReminderDays(string input)
		{
			var match = Regex.Match(input, @"remind me in (\d+)\s*days?", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
			return null;
		}
	}
}