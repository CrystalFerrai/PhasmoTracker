using System.Windows;

namespace PhasmoTracker
{
	internal partial class GameSettingsDialog : Window
	{
		public GameSettingsDialog(GameSettings settings)
		{
			DataContext = settings;
			Resources.Add("MapSizeConverter", settings.MapSizeConverter);

			InitializeComponent();
		}
	}
}
