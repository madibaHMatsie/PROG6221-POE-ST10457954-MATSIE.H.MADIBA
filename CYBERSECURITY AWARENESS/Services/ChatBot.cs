using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CYBERSECURITY_AWARENESS.Services
{
	public class ChatBot
	{
		private Memory _memory;
		private SentimentAnalyser _sentiment;
		private Random _random = new Random();
		private Dictionary<string, List<string>> _responseDatabase;
		private Dictionary<string, string> _followUpTopics;

		public ChatBot(Memory memory, SentimentAnalyser sentiment)
		{
			_memory = memory;
			_sentiment = sentiment;
			InitializeResponses();
			InitializeFollowUps();
		}

		private void InitializeResponses()
		{
			_responseDatabase = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
			{
				["password"] = new List<string>
				{
					"Use strong, unique passwords for each account. Avoid using personal details like birthdays.",
					"A good password has at least 12 characters with a mix of letters, numbers, and symbols.",
					"Consider using a password manager to generate and store complex passwords securely.",
					"Enable two-factor authentication whenever possible - it adds an extra layer of security."
				},

				["phish"] = new List<string>
				{
					"Never click links in unsolicited emails. Hover over links first to see the real URL.",
					"Check the sender's email address carefully. Scammers often use addresses that look similar to real ones.",
					"Be suspicious of urgent requests for personal information. Legitimate companies don't do this.",
					"Look for spelling and grammar errors - these are common signs of phishing attempts."
				},

				["privacy"] = new List<string>
				{
					"Review your social media privacy settings regularly. Limit who can see your posts.",
					"Be careful what personal information you share online - once posted, it's hard to remove.",
					"Use privacy-focused browsers and search engines for better online privacy.",
					"Consider using a VPN to encrypt your internet connection, especially on public Wi-Fi."
				},

				["scam"] = new List<string>
				{
					"If something sounds too good to be true, it probably is. Be skeptical of unexpected prizes.",
					"Never send money or gift cards to someone you haven't met in person.",
					"Verify investment opportunities with registered financial advisors before committing.",
					"Report scams to the South African Banking Risk Information Centre (SABRIC)."
				},

				["help"] = new List<string>
				{
					"I can help you with:\n- Password safety\n- Phishing scams\n- Privacy protection\n- Scam recognition\n- Safe browsing",
					"Try asking me about passwords, phishing, privacy, or online scams. I'll give you helpful tips!"
				}
			};
		}

		private void InitializeFollowUps()
		{
			_followUpTopics = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["another tip"] = "password",
				["tell me more"] = "phish",
				["explain more"] = "privacy",
				["more information"] = "scam",
				["more tips"] = "password"
			};
		}

		public async Task<string> GetResponseAsync(string userInput)
		{
			await Task.Delay(10); 

			
			if (string.IsNullOrEmpty(_memory.GetUserName()) && !userInput.Contains("my name is"))
			{
				if (userInput.Length < 30 && !userInput.Contains("?"))
				{
					_memory.SetUserName(userInput);
					return $"Nice to meet you, {userInput}! I'll remember your name. What cybersecurity topic interests you?";
				}
			}

			if (userInput.ToLower().Contains("my name is"))
			{
				string name = ExtractName(userInput);
				_memory.SetUserName(name);
				return $"Hello {name}! I'll remember that. What would you like to learn about cybersecurity?";
			}

			
			if (userInput.ToLower().Contains("interested in") || userInput.ToLower().Contains("like to learn about"))
			{
				string interest = ExtractInterest(userInput);
				_memory.SetUserInterest(interest);
				return $"Great! I'll remember that you're interested in {interest}. Let me share some tips about {interest}.";
			}

			// Follow-up requests
			foreach (var followUp in _followUpTopics)
			{
				if (userInput.ToLower().Contains(followUp.Key))
				{
					if (_responseDatabase.ContainsKey(followUp.Value))
					{
						var responses = _responseDatabase[followUp.Value];
						return GetRandomResponse(responses);
					}
				}
			}

			// keywords in database
			foreach (var keyword in _responseDatabase.Keys)
			{
				if (userInput.ToLower().Contains(keyword))
				{
					var responses = _responseDatabase[keyword];
					string response = GetRandomResponse(responses);

					string userName = _memory.GetUserName();
					if (!string.IsNullOrEmpty(userName) && !response.Contains(userName))
					{
						response = $"{userName}, " + response.ToLower().FirstCharToUpper();
					}

					var sentiment = _sentiment.DetectSentiment(userInput);
					if (sentiment.IsNegative)
					{
						response = "I understand your concern. " + response;
					}
					return response;
				}
			}

			// Greetings
			if (userInput.ToLower().Contains("hello") || userInput.ToLower().Contains("hi"))
			{
				string name = _memory.GetUserName();
				return name != null ? $"Hi {name}! How can I help you today?" : "Hello! What's your name?";
			}

			if (userInput.ToLower().Contains("how are you"))
				return "I'm functioning well, thank you! Ready to help you stay cyber-safe.";

			if (userInput.ToLower().Contains("purpose") || userInput.ToLower().Contains("what can you do"))
				return "I'm your Cyber Security Assistant. I help educate about online safety, password security, and recognizing scams.";

			if (userInput.ToLower().Contains("thank"))
				return "You're welcome! Stay safe online.";

			// Response 
			string userNameRecall = _memory.GetUserName();
			string interestRecall = _memory.GetUserInterest();

			if (!string.IsNullOrEmpty(interestRecall))
				return $"Since you're interested in {interestRecall}, would you like me to share more tips about that topic? Or try asking about passwords, phishing, or privacy.";

			if (!string.IsNullOrEmpty(userNameRecall))
			{
				return $"I'm not sure I understand, {userNameRecall}. Could you rephrase your question? Try asking about passwords, phishing, or privacy.";
			}

			return "I'm not sure I understand. Can you try rephrasing? You can ask me about passwords, phishing, privacy, or online scams.";
		}

		private string GetRandomResponse(List<string> responses)
		{
			return responses[_random.Next(responses.Count)];
		}

		private string ExtractName(string input)
		{
			string lower = input.ToLower();
			int index = lower.IndexOf("my name is");
			if (index >= 0)
			{
				string namePart = input.Substring(index + 10).Trim();
				string[] words = namePart.Split(' ');
				return words[0];
			}
			return input.Trim();
		}

      private string ExtractInterest(string input)
        {
          string lower = input.ToLower();
         if (lower.Contains("update interest to"))
               {
                  int index = lower.IndexOf("update interest to");
                  string interestPart = input.Substring(index + 19).Trim();
                  return interestPart.Split(' ', '.', '?')[0];
            }
          else if (lower.Contains("my new interest is"))
          {
             int index = lower.IndexOf("my new interest is");
           string interestPart = input.Substring(index + 18).Trim();
             return interestPart.Split(' ', '.', '?')[0];
           }
             else if (lower.Contains("interested in"))
          {
             int index = lower.IndexOf("interested in");
               string interestPart = input.Substring(index + 13).Trim();
             return interestPart.Split(' ', '.', '?')[0];
           }
               return "cybersecurity";
        }
	
	public static class StringExtensions
	{
		public static string FirstCharToUpper(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;
			return char.ToUpper(str[0]) + str.Substring(1);
		}
	}
}
