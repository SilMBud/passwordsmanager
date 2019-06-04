using Caliburn.Micro;
using PAWSwords.Core.Interfaces;
using PAWSwords.Core.Passwords;
using PAWSwords.Passwords.Messages;

namespace PAWSwords.Passwords
{
    public class CreatePasswordViewModel : Screen
	{
		private readonly IPasswordsManager _passwordsManager;
		private readonly IEventAggregator _eventAggregator;
		private string _userName;
		private string _description;
        private string _errorMessage;

        public CreatePasswordViewModel(IPasswordsManager passwordsManager, IEventAggregator eventAggregator)
		{
			_passwordsManager = passwordsManager;
			_eventAggregator = eventAggregator;
		}

		public string UserName
		{
			get => _userName;
			set => Set(ref _userName, value);
		}

		public string Description
		{
			get => _description;
			set => Set(ref _description, value);
        }
        public string Error
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }


        public async void Create(CreatePasswordView view)
		{
            var password = view.pw.SecurePassword;

            if (string.IsNullOrWhiteSpace(UserName))
            {
                Error = "Vartotojo vardas negali būti tuščias";
                return;
            }

            if (password.Length == 0)
            {
                Error = "Slaptažodis negali būti tuščias";
                return;
            }

            var addedPassword = await _passwordsManager.Add(new PasswordEntry(password, UserName, Description ?? ""));
            _eventAggregator.PublishOnUIThread(new PasswordAddedMessage(addedPassword));
        }

		public void Cancel()
		{
			_eventAggregator.PublishOnUIThread(new PasswordCreationCanceledMessage());
		}
	}
}
