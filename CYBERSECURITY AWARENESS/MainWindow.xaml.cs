using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CYBERSECURITY_AWARENESS.Services;

namespace CYBERSECURITY_AWARENESS
{
	public partial class MainWindow : Window
	{
		
		private ChatBot? _chatbot;
		private Audio? _audio;
		private Memory? _memory;
		private SentimentAnalyser? _sentiment;
		private Database? _database;
		private TaskAssist? _taskAssist;
		private Quiz? _quiz;
		private LogService? _logService;
		private NLPSimulation? _nlp;

		public MainWindow()
		{
			try
			{
				InitializeComponent();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Fatal error loading window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Application.Current.Shutdown();
				return;
			}

			// Initialise services 
			try
			{
				InitializeServices();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error initializing services: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				
			}

			Loaded += MainWindow_Loaded;
		}

		private void InitializeServices()
		{
			
			_logService = new LogService();

			
			try
			{
				_database = new Database();
					if (!_database.TestConnection())
					{
						MessageBox.Show(
							"MySQL database is not available.\n" +
							"Task features will be disabled.",
							"Database Warning",
							MessageBoxButton.OK,
							MessageBoxImage.Warning);
					}
				}
			catch (Exception ex)
			{
				MessageBox.Show($"Database connection error: {ex.Message}\nTask features disabled.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				_database = null;
			}

			_taskAssist = new TaskAssist(_database, _logService);

			
			_quiz = new Quiz(_logService);

			//  NLP
			_nlp = new NLPSimulation();

			// MEMORY & SENTIMENT 
			_memory = new Memory();
			_sentiment = new SentimentAnalyser();

			//  CHATBOT
			_chatbot = new ChatBot(
				_memory,
				_sentiment,
				_taskAssist,
				_quiz,
				_logService,
				_nlp
			);

			// 8. AUDIO
			_audio = new Audio();

			
		}
		private string GetAsciiLogo()
		{
			return @"       .---..-.  .-..----. .----..----. .----.  .----.  .---. 
                           /  ___}\ \/ / | {}  }| {_  | {}  }| {}  }/  {}  \{_   _}
                           \     } }  {  | {}  }| {__ | .-. \| {}  }\      /  | |  
                            `---'  `--'  `----' `----'`-' `-'`----'  `----'   `-'  
";
		}

		
		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (_audio != null)
				await _audio.SpeakAsync("Welcome to Cyber Bot!");
			else
				AddBotMessage("Voice service not available.");

			AddBotMessage("Hello! I'm Cyber Bot, your cybersecurity awareness assistant.");
			AddBotMessage("What's your name?");
			UpdateStatus("Awaiting user name...");
		}

		
		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			await ProcessUserInput();
		}

		private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift))
			{
				e.Handled = true;
				await ProcessUserInput();
			}
		}

		
		private async Task ProcessUserInput()
		{
			string userInput = InputTextBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(userInput))
				return;

			InputTextBox.Clear();
			SendButton.IsEnabled = false;
			InputTextBox.IsEnabled = false;
			UpdateStatus("Processing...");

			AddUserMessage(userInput);

			
			if (_sentiment == null)
				_sentiment = new SentimentAnalyser();

			var sentiment = _sentiment.DetectSentiment(userInput);
			UpdateMoodDisplay(sentiment);

			
			if (_chatbot == null)
			{
				AddBotMessage("Sorry, the chatbot service is not available. Please restart the application.");
				ReEnableInput();
				return;
			}

			string botResponse = await _chatbot.GetResponseAsync(userInput);

			
			if (_audio != null && botResponse.Length < 100 && !botResponse.Contains("\n"))
				await _audio.SpeakAsync(botResponse);

			AddBotMessage(botResponse);
			UpdateMemoryDisplay();
			ScrollToBottom();

			ReEnableInput();
		}

		private void ReEnableInput()
		{
			SendButton.IsEnabled = true;
			InputTextBox.IsEnabled = true;
			InputTextBox.Focus();
			UpdateStatus("Ready");
		}

		private void AddUserMessage(string message)
		{
			var border = new Border { Style = (Style)FindResource("UserMessageStyle") };
			var textBlock = new TextBlock
			{
				Text = message,
				Foreground = Brushes.White,
				TextWrapping = TextWrapping.Wrap,
				FontSize = 13
			};
			border.Child = textBlock;
			ChatMessagesPanel.Children.Add(border);
		}

		private void AddBotMessage(string message)
		{
			var border = new Border { Style = (Style)FindResource("BotMessageStyle") };
			var stackPanel = new StackPanel();
			string[] lines = message.Split('\n');
			foreach (string line in lines)
			{
				var textBlock = new TextBlock
				{
					Text = line.Trim(),
					Foreground = Brushes.Black,
					TextWrapping = TextWrapping.Wrap,
					FontSize = 13,
					Margin = new Thickness(0, 2, 0, 2)
				};
				stackPanel.Children.Add(textBlock);
			}
			border.Child = stackPanel;
			ChatMessagesPanel.Children.Add(border);
		}

		private void ScrollToBottom()
		{
			ChatScrollViewer.ScrollToBottom();
		}

		private void UpdateStatus(string message)
		{
			StatusText.Text = message;
		}

		private void UpdateMoodDisplay(SentimentResult sentiment)
		{
			UserMoodText.Text = sentiment.Mood;
			if (sentiment.IsNegative)
				UserMoodText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B6B");
			else if (sentiment.IsPositive)
				UserMoodText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#90BE6D");
			else
				UserMoodText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE066");
		}

		private void UpdateMemoryDisplay()
		{
			UserNameText.Text = _memory?.GetUserName() ?? "Guest";
			UserInterestText.Text = _memory?.GetUserInterest() ?? "None";
		}

		private void AddSampleTask_Click(object sender, RoutedEventArgs e)
		{
			InputTextBox.Text = "Add task - Enable two-factor authentication";
			_ = ProcessUserInput();
		}

		private void ShowTasks_Click(object sender, RoutedEventArgs e)
		{
			InputTextBox.Text = "Show tasks";
			_ = ProcessUserInput();
		}

		private void StartQuiz_Click(object sender, RoutedEventArgs e)
		{
			InputTextBox.Text = "Start quiz";
			_ = ProcessUserInput();
		}

		private void ShowActivityLog_Click(object sender, RoutedEventArgs e)
		{
			InputTextBox.Text = "Activity log";
			_ = ProcessUserInput();
		}

		private void QuitButton_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to quit Cyber Bot?", "Exit",
										  MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				Application.Current.Shutdown();
			}
		}
	}
}