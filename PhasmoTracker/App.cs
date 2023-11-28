// Copyright 2023 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PhasmoTracker.Utils;
using System;
using System.Windows;

namespace PhasmoTracker
{
	/// <summary>
	/// The main application class
	/// </summary>
	internal class App : Application
	{
		private MainWindowVM? mMainWindowVM;

		private StreamOverlayWindow? mStreamOverlayWindow;

		/// <summary>
		/// The entry point of the application
		/// </summary>
		/// <param name="args">Command line parameters passed to the application</param>
		/// <returns>The application exit code</returns>
		[STAThread]
		public static int Main(string[] args)
		{
			App app = new App();
			return app.Run();
		}

		/// <summary>
		/// Creates a new instance of the App class
		/// </summary>
		public App()
		{
			ShutdownMode = ShutdownMode.OnExplicitShutdown;
			Resources.MergedDictionaries.Add(LoadResourceDictionary("Resources.xaml"));
		}

		/// <summary>
		/// Called when the application is starting up
		/// </summary>
		protected override void OnStartup(StartupEventArgs e)
		{
			mMainWindowVM = new MainWindowVM();

			mStreamOverlayWindow = new StreamOverlayWindow(mMainWindowVM);
			mStreamOverlayWindow.Show();
			mStreamOverlayWindow.Hide();

			mMainWindowVM.SetOverlayWindow(mStreamOverlayWindow);

			MainWindow = new MainWindow(mMainWindowVM);
			MainWindow.Closed += MainWindow_Closed;
			MainWindow.Show();

			// These might be good as a user options at some point

			// This keeps the overlay window on top of the main window, which may not be desired.
			//mStreamOverlayWindow.Owner = MainWindow;

			// This keeps the overlay on top of all non-topmost windows system wide
			//mStreamOverlayWindow.Topmost = true;

			base.OnStartup(e);
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			mStreamOverlayWindow?.Close();
			Shutdown();
		}

		/// <summary>
		/// Called when the application is shutting down
		/// </summary>
		protected override void OnExit(ExitEventArgs e)
		{
			mMainWindowVM?.OnApplicationExiting();
			mMainWindowVM?.Dispose();

			base.OnExit(e);
		}

		/// <summary>
		/// Helper method for loading resource dictionaries that are embedded as resources in the application
		/// </summary>
		/// <param name="path">The relative path fromt he project root to the xaml file containing the resource dictionary</param>
		/// <returns>The loaded resource dictionary</returns>
		private static ResourceDictionary LoadResourceDictionary(string path)
		{
			return (ResourceDictionary)LoadComponent(ResourceHelper.GetResourceUri(path, true));
		}
	}
}
