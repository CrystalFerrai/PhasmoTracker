using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PhasmoTracker
{
	/// <summary>
	/// Interaction logic for StreamOverlayWindow.xaml
	/// </summary>
	internal partial class StreamOverlayWindow : Window
	{
		public StreamOverlayWindow(MainWindowVM viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (Application.Current.MainWindow?.IsVisible ?? false)
			{
				e.Cancel = true;
			}

			base.OnClosing(e);
		}
	}
}
