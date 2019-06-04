using Caliburn.Micro;
using PAWSwords.Core;
using PAWSwords.Core.Interfaces;
using PAWSwords.Core.Passwords;
using System;

namespace PAWSwords.Passwords
{
    public class PasswordViewModel : Screen
    {
        private string _passwordString = "*********";
        private bool _isPasswordShown;

        private readonly LazyPasswordEntry _lazyPasswordEntry;
        private readonly IMasterKeyManager _masterKeyManager;
        private readonly PasswordEntry _passwordEntry;

        public PasswordViewModel(PasswordEntry password)
        {
            Id = password.Id;
            UserName = password.UserName;
            Description = password.Description;
            _isLazy = false;
            _passwordEntry = password;
        }

        public PasswordViewModel(LazyPasswordEntry password, IMasterKeyManager masterKeyManager)
        {
            Id = password.Id;
            UserName = password.UserName;
            Description = password.Description;
            _isLazy = true;
            _lazyPasswordEntry = password;
            _masterKeyManager = masterKeyManager;
        }

        public event EventHandler<RemovePasswordEventArgs> RemoveRequested;

        public Guid? Id { get; }

        public string UserName { get; }

        public string Description { get; }

        private readonly bool _isLazy;

        public string Password
        {
            get => _passwordString;
            private set
            {
                Set(ref _passwordString, value);
            }
        }

        public bool IsPasswordShown
        {
            get => _isPasswordShown;
            private set => Set(ref _isPasswordShown, value);
        }

        public async void ShowPassword()
        {
            if (_isLazy)
            {
                using (var masterKey = await _masterKeyManager.Get())
                {
                    Password = Helpers.GetStringFromSecureString(await _lazyPasswordEntry.GetPassword(masterKey.Value));
                }
            }
            else
            {
                Password = Helpers.GetStringFromSecureString(_passwordEntry.Password);
            }

            IsPasswordShown = true;
        }

        public void HidePassword()
        {
            Password = "*********";
            IsPasswordShown = false;
        }

        public void Remove()
        {
            if (Id.HasValue)
                RemoveRequested?.Invoke(this, new RemovePasswordEventArgs(Id.Value));
        }

       
    }

    public class RemovePasswordEventArgs : EventArgs
    {
        public RemovePasswordEventArgs(Guid passwordId)
        {
            PasswordId = passwordId;
        }

        public Guid PasswordId { get; }
    }
}