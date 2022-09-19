using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Carbon.Example.Desktop
{
	public partial class SessionDialog : Window, INotifyPropertyChanged
	{
		public SessionDialog()
		{
			InitializeComponent();
			Loaded += SessionDialog_Loaded;
		}

		void SessionDialog_Loaded(object sender, RoutedEventArgs e)
		{
			PassAccount.Password = Password;
			BoxAccountId.Focus();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		MainController Controller => (MainController)DataContext;

		void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		async Task InnerLogin()
		{
			if (!CanAuthenticate) return;
			FeedInfo("Authenticating...");
			await Task.Delay(500);
			if (await Controller.LoginAsync(_accountId, _password))
			{
				FeedInfo("Success");
				DialogResult = true;
				return;
			}
			FeedAlert(Controller.AppError.Message);
		}

		void FeedInfo(string message)
		{
			LabelFeedback.Foreground = SystemColors.WindowTextBrush;
			Feedback = message;
		}

		void FeedAlert(string message)
		{
			LabelFeedback.Foreground = Brushes.DarkRed;
			Feedback = message;
		}

		#region Handlers

		async void Trap_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				await InnerLogin();
			}
		}

		async void Password_KeyUp(object sender, KeyEventArgs e)
		{
			Password = PassAccount.Password;
			if (e.Key == Key.Enter)
			{
				await InnerLogin();
			}
		}

		async void Authenticate_Click(object sender, RoutedEventArgs e)
		{
			await InnerLogin();
		}

		#endregion

		#region Binding Properties

		string _accountId;
		public string AccountId
		{
			get => _accountId;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_accountId != newval)
				{
					_accountId = newval;
					OnPropertyChanged(nameof(AccountId));
					OnPropertyChanged(nameof(CanAuthenticate));
				}
			}
		}

		string _password;
		public string Password
		{
			get => _password;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_password != newval)
				{
					_password = newval;
					OnPropertyChanged(nameof(Password));
					OnPropertyChanged(nameof(CanAuthenticate));
				}
			}
		}

		string _feedback = "Ready to login";
		public string Feedback
		{
			get => _feedback;
			set
			{
				string newval = string.IsNullOrEmpty(value) ? null : value;
				if (_feedback != newval)
				{
					_feedback = newval;
					OnPropertyChanged(nameof(Feedback));
				}
			}
		}

		public bool CanAuthenticate => _accountId?.Length > 3 && _password?.Length > 3;

		#endregion
	}
}
