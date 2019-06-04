using System.Windows;

namespace PAWSwords
{
	public partial class ShellView : Window
	{
		public ShellView()
		{
			InitializeComponent();
		}

		private void CloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
