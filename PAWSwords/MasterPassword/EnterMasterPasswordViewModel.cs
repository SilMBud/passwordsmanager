using Caliburn.Micro;
using PAWSwords.Core.Interfaces;
using System;

namespace PAWSwords.MasterPassword
{
    public class EnterMasterPasswordViewModel : Screen
	{
		private readonly IMasterKeyManager _masterKeyManager;
		private readonly IEventAggregator _eventAggregator;
        private string _errorMessage;

        public EnterMasterPasswordViewModel(IMasterKeyManager masterKeyManager, IEventAggregator eventAggregator)
		{
			_masterKeyManager = masterKeyManager;
			_eventAggregator = eventAggregator;
		}

        public string Error
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        public async void Continue(EnterMasterPasswordView view)
		{
			using (var password = view.pw.SecurePassword)
			{
                if (password.Length < 6)
                {
                    Error = "Pagrindinis slaptažodis turi būti mažiausiai 6 simbolių ilgio";
                    return;
                }

                try
                {
                    await _masterKeyManager.Load(password);
                }
                catch (Exception)
                {
                    Error = "Įvestas neteisingas pagrindinis slaptažodis";
                    return;
                }

                _eventAggregator.PublishOnUIThread(new MasterKeyLoadedMessage());
			}
		}
	}
}
