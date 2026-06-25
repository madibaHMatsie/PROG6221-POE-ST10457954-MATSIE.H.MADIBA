using System;
using System.Collections.Generic;
using CYBERSECURITY_AWARENESS.Models;

namespace CYBERSECURITY_AWARENESS.Services
{
	public class Quiz
	{
		private List<QQuestion> _questions;
		private int _currentIndex;
		private int _score;
		private LogService _log;

		public bool IsQuizActive { get; private set; }
		public QQuestion CurrentQuestion => IsQuizActive ? _questions[_currentIndex] : null;

		public Quiz(LogService log)
		{
			_log = log;
			InitializeQuestions();
			IsQuizActive = false;
		}

		private void InitializeQuestions()
		{
			_questions = new List<QQuestion>
			{
				new QQuestion
				{
					Question = "What should you do if you receive an email asking for your password?",
					Options = new List<string> { "Reply with your password", "Delete the email", "Report it as phishing", "Forward it to friends" },
					CorrectAnswerIndex = 2,
					Explanation = "Reporting phishing emails helps prevent scams. Legitimate companies never ask for passwords via email.",
					Topic = "Phishing"
				},
				new QQuestion
				{
					Question = "True or False: Using the same password for multiple accounts is safe.",
					Options = new List<string> { "True", "False" },
					CorrectAnswerIndex = 1,
					Explanation = "Using the same password puts all your accounts at risk. Use unique passwords for each account.",
					Topic = "Passwords"
				},
				new QQuestion
				{
					Question = "What does 'https://' indicate in a website URL?",
					Options = new List<string> { "The site is free", "The site is secure", "The site is fast", "The site is popular" },
					CorrectAnswerIndex = 1,
					Explanation = "HTTPS means the connection is encrypted, protecting your data from being intercepted.",
					Topic = "Safe Browsing"
				},
				new QQuestion
				{
					Question = "What is two-factor authentication (2FA)?",
					Options = new List<string> { "A second password", "A fingerprint scan only", "A second verification method", "An antivirus program" },
					CorrectAnswerIndex = 2,
					Explanation = "2FA adds an extra layer of security by requiring a second form of verification (like a code from your phone).",
					Topic = "Authentication"
				},
				new QQuestion
				{
					Question = "True or False: Public Wi-Fi is always safe to use for banking.",
					Options = new List<string> { "True", "False" },
					CorrectAnswerIndex = 1,
					Explanation = "Public Wi-Fi can be insecure. Avoid banking on public networks unless using a VPN.",
					Topic = "Safe Browsing"
				},
				new QQuestion
				{
					Question = "What is a common sign of a phishing email?",
					Options = new List<string> { "Perfect spelling", "Urgent requests", "Professional design", "Known sender" },
					CorrectAnswerIndex = 1,
					Explanation = "Phishing emails often create urgency to pressure you into acting without thinking.",
					Topic = "Phishing"
				},
				new QQuestion
				{
					Question = "How often should you update your passwords?",
					Options = new List<string> { "Never", "Every 10 years", "Every 3-6 months", "Only when hacked" },
					CorrectAnswerIndex = 2,
					Explanation = "Regular password changes reduce the risk of unauthorized access.",
					Topic = "Passwords"
				},
				new QQuestion
				{
					Question = "True or False: Sharing your OTP (One-Time Pin) with a 'bank official' is safe.",
					Options = new List<string> { "True", "False" },
					CorrectAnswerIndex = 1,
					Explanation = "Never share your OTP with anyone. Banks will never ask for it.",
					Topic = "Social Engineering"
				},
				new QQuestion
				{
					Question = "What should you do before downloading a file from the internet?",
					Options = new List<string> { "Download immediately", "Scan with antivirus", "Ignore warnings", "Share the link" },
					CorrectAnswerIndex = 1,
					Explanation = "Always scan downloads with antivirus software to prevent malware infections.",
					Topic = "Safe Browsing"
				},
				new QQuestion
				{
					Question = "What is a password manager?",
					Options = new List<string> { "A person who remembers passwords", "A tool that stores passwords securely", "A hacker tool", "A type of antivirus" },
					CorrectAnswerIndex = 1,
					Explanation = "Password managers securely store and generate strong passwords for all your accounts.",
					Topic = "Passwords"
				},
				new QQuestion
				{
					Question = "True or False: You should click on pop-up ads that say 'You won a prize'.",
					Options = new List<string> { "True", "False" },
					CorrectAnswerIndex = 1,
					Explanation = "Pop-up ads claiming prizes are often scams designed to steal your information.",
					Topic = "Scams"
				}
			};
		}

		public string StartQuiz()
		{
			_currentIndex = 0;
			_score = 0;
			IsQuizActive = true;
			_log.AddEntry("Quiz Started", "User started the cybersecurity quiz");
			return GetNextQuestion();
		}

		public string GetNextQuestion()
		{
			if (_currentIndex >= _questions.Count)
			{
				return EndQuiz();
			}
			var q = _questions[_currentIndex];
			string optionsText = "";
			for (int i = 0; i < q.Options.Count; i++)
			{
				optionsText += $"\n{(char)('A' + i)}) {q.Options[i]}";
			}
			return $"Question {_currentIndex + 1}/{_questions.Count}: {q.Question}{optionsText}\n\nType your answer (A, B, C, or D):";
		}

		public string SubmitAnswer(string answer)
		{
			if (!IsQuizActive)
				return "No quiz in progress. Type 'start quiz' to begin!";

			var q = _questions[_currentIndex];
			int selectedIndex = -1;

			answer = answer.ToUpper().Trim();
			if (answer.Length == 1 && answer[0] >= 'A' && answer[0] <= 'D')
			{
				selectedIndex = answer[0] - 'A';
			}
			else if (int.TryParse(answer, out int num) && num >= 1 && num <= q.Options.Count)
			{
				selectedIndex = num - 1;
			}

			if (selectedIndex >= 0 && selectedIndex < q.Options.Count)
			{
				bool isCorrect = (selectedIndex == q.CorrectAnswerIndex);
				if (isCorrect)
				{
					_score++;
					_currentIndex++;
					return $"Correct! {q.Explanation}\n\n{GetNextQuestion()}";
				}
				else
				{
					_currentIndex++;
					return $"Incorrect. The correct answer was {(char)('A' + q.CorrectAnswerIndex)}. {q.Explanation}\n\n{GetNextQuestion()}";
				}
			}
			return "Invalid answer. Please type A, B, C, or D.";
		}

		private string EndQuiz()
		{
			IsQuizActive = false;
			int percentage = (_score * 100) / _questions.Count;
			string feedback = percentage >= 80 ? "Excellent! You're a cybersecurity pro!" :
							  percentage >= 60 ? "Good job! Keep learning to stay safe online!" :
							  "Keep studying cybersecurity - it's important for staying safe online!";

			_log.AddEntry("Quiz Completed", $"Score: {_score}/{_questions.Count} ({percentage}%)");
			return $"Quiz completed! Your score: {_score}/{_questions.Count} ({percentage}%)\n\n{feedback}\n\nType 'start quiz' to play again or ask me cybersecurity questions!";
		}
	}
}