using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CYBERSECURITY_AWARENESS.Services;

namespace CYBERSECURITY_AWARENESS
{
    public partial class MainWindow : Window
    {

		public MainWindow()
		{
			InitializeComponent();
			InitializeServices();
			Loaded += MainWindow_Loaded;
		}

		private void InitializeServices()
		{
			_memory = new Memory();
			_sentiment = new SentimentAnalyser();
		
			

			private string GetAsciiLogo()
			{
				return @"
__  _    ___  ____ ___  ___  ________    
/ /`\ \_/| |_)| |_ | |_)| |_)/ / \| |     
\_\_,|_| |_|_)|_|__|_| \|_|_)\_\_/|_|";
			}
		
		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// Play voice greeting
			await _audio.SpeakAsync("Welcome to CyberBot!");

		
			AddBotMessage("Hello! I'm CyberBot, your cybersecurity awareness chatbot.");
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

			AddUserMessage(userInput);
			var sentiment = _sentiment.DetectSentiment(userInput);
			UpdateMoodDisplay(sentiment);

			string botResponse = await _chatbot.GetResponseAsync(userInput);
			AddBotMessage(botResponse);
			UpdateMemoryDisplay();
			ScrollToBottom();

			SendButton.IsEnabled = true;
			InputTextBox.IsEnabled = true;
			InputTextBox.Focus();
		}
		private void AddUserMessage(string message)
		{
			var border = new Border
			{
				Style = (Style)FindResource("UserMessageStyle")
			};

			var textBlock = new TextBlock
			{
				Text = message,
				Foreground = Brushes.DarkGray,
				TextWrapping = TextWrapping.Wrap,
				FontSize = 13
			};
			border.Child = textBlock;
			ChatMessagesPanel.Children.Add(border);
		}

		private void AddBotMessage(string message)
		{
			var border = new Border
			{
				Style = (Style)FindResource("BotMessageStyle")
			};

			// Handle multi-line messages
			var stackPanel = new StackPanel();
			string[] lines = message.Split('\n');
			foreach (string line in lines)
			{
				var textBlock = new TextBlock
				{
					Text = line.Trim(),
					Foreground = Brushes.DarkGray,
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
				UserMoodText.Foreground = (Brush)FindResource("PrimaryColor");
			else if (sentiment.IsPositive)
				UserMoodText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#90BE6D");
			else
				UserMoodText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE066");
		}

		private void UpdateMemoryDisplay()
		{
			UserNameText.Text = _memory.GetUserName() ?? "Guest";
			UserInterestText.Text = _memory.GetUserInterest() ?? "None";
		}
	}
}