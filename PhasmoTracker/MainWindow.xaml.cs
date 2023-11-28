using PhasmoTracker.Utils;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhasmoTracker
{
	/// <summary>
	/// The main application window
	/// </summary>
	internal partial class MainWindow : Window
	{
		private static ImageSource sAppIcon;

		private MainWindowVM ViewModel => (MainWindowVM)DataContext;

		static MainWindow()
		{
			sAppIcon = BitmapFrame.Create(ResourceHelper.GetResourceUri("Resources/appicon.ico"));
		}

		/// <summary>
		/// Creates a new instance of the MainWindow class
		/// </summary>
		public MainWindow(MainWindowVM viewModel)
		{
			DataContext = viewModel;
			Icon = sAppIcon;

			Loaded += MainWindow_Loaded;

			InitializeComponent();
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel.OnMainWindowLoaded();
		}

		private void Sanity_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right)
			{
				ViewModel.ResetSanityDrainCommand.Execute(null);
				e.Handled = true;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.R:
					ViewModel.ResetData();
					e.Handled = true;
					break;
			}

			base.OnKeyDown(e);
		}
	}
}
