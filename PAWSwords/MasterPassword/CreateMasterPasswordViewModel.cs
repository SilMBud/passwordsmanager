using Caliburn.Micro;
using PAWSwords.Core.Interfaces;

namespace PAWSwords.MasterPassword
{
    public class CreateMasterPasswordViewModel : Screen
    {
        private readonly IMasterKeyManager _masterKeyManager;
        private readonly IEventAggregator _eventAggregator;
        private string _errorMessage;

        public CreateMasterPasswordViewModel(IMasterKeyManager masterKeyManager, IEventAggregator eventAggregator)
        {
            _masterKeyManager = masterKeyManager;
            _eventAggregator = eventAggregator;
        }

        public string Error
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        public async void Create(CreateMasterPasswordView view)
        {
            using (var password = view.pw.SecurePassword)
            {

                if (password.Length < 6)
                {
                    Error = "Pagrindinis slaptažodis turi būti mažiausiai 6 simbolių ilgio";
                    return;
                }

                await _masterKeyManager.Create(password);
                _eventAggregator.PublishOnUIThread(new MasterKeyLoadedMessage());
            }
        }
    }
}
