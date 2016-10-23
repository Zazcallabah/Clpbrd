using System.Windows;

namespace Clpbrd
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly Controller _controller;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = _controller = new Controller( MyNotifyIcon );
		}

		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			MyNotifyIcon.Dispose();
			_controller.Dispose();
			base.OnClosing( e );
		}

		void Exit( object sender, RoutedEventArgs e )
		{
			Application.Current.Shutdown();
		}
	}
}
