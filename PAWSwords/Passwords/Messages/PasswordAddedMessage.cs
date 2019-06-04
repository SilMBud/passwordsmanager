using PAWSwords.Core.Passwords;

namespace PAWSwords.Passwords.Messages
{
	public sealed class PasswordAddedMessage
	{
		public PasswordAddedMessage(PasswordEntry addedPassword)
		{
			AddedPassword = addedPassword;
		}

		public PasswordEntry AddedPassword { get; }
	}
}