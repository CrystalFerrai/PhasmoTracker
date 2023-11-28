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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PhasmoTracker
{
	/// <summary>
	/// The view model for the main application window
	/// </summary>
	internal class MainWindowVM : ViewModelBase
	{
		private readonly InputManager mInputManager;

		private readonly DelegateCommand mResetDataCommand;

		private readonly DelegateCommand mRestartTimerCommand;
		private readonly DelegateCommand mStopTimerCommand;

		private readonly DelegateCommand mToggleSanityDrainCommand;
		private readonly DelegateCommand mResetSanityDrainCommand;
		private readonly DelegateCommand mLose5SanityCommand;
		private readonly DelegateCommand mUseSanityMedicationCommand;

		private readonly DelegateCommand mToggleNormalSpeedCommand;
		private readonly DelegateCommand mToggleAbnormalSpeedCommand;

		private readonly DispatcherTimer mTimer;
		private readonly DispatcherTimer mSanityTimer;

		private readonly GameSettings mGameSettings;
		private readonly GameSettingsDialog mSettingsDialog;

		private DateTime mTimerStartTime;
		private DateTime mSanityTimerUpdateTime;

		private Window? mOverlayWindow;

		private bool mIsSettingsDialogOpen;

		private bool? mIsTimerRunning;
		private bool? mIsSanityDraining;

		/// <summary>
		/// View models for all evidences
		/// </summary>
		public EvidencesVM Evidences { get; }

		/// <summary>
		/// View models for all ghosts
		/// </summary>
		public GhostsVM Ghosts { get; }

		/// <summary>
		/// Current game settings
		/// </summary>
		public GameSettings GameSettings => mGameSettings;

		/// <summary>
		/// Whether the game settings dialog is currently open
		/// </summary>
		public bool IsSettingsDialogOpen
		{
			get => mIsSettingsDialogOpen;
			set
			{
				if (Set(ref mIsSettingsDialogOpen, value))
				{
					OpenSettingsDialog(value);
				}
			}
		}

		/// <summary>
		/// Whether the current ghost is normal speed
		/// </summary>
		public bool? IsNormalSpeed
		{
			get => _isNormalSpeed;
			set
			{
				if (Set(ref _isNormalSpeed, value))
				{
					Ghosts.ApplyEvidences(mGameSettings.EvidenceCount, Evidences.GatherCurrentEvidence(), value);
				}
			}
		}
		private bool? _isNormalSpeed;

		/// <summary>
		/// Whether the stream overlay window is currently visible
		/// </summary>
		public bool? IsOverlayWindowVisible
		{
			get
			{
				if (mOverlayWindow is null) return null;
				return mOverlayWindow.IsVisible;
			}
			set
			{
				if (mOverlayWindow is null || !value.HasValue) return;
				if (mOverlayWindow.IsVisible != value.Value)
				{
					if (value.Value) mOverlayWindow.Show();
					else mOverlayWindow.Hide();
				}
			}
		}

		/// <summary>
		/// Whether shortcut keys are currently enabled
		/// </summary>
		public bool AreGlobalShortcutsEnabled
		{
			get => _areGlobalShortcutsEnabled;
			set
			{
				if (Set(ref _areGlobalShortcutsEnabled, value))
				{
					mInputManager.EnableInput(value);
				}
			}
		}
		private bool _areGlobalShortcutsEnabled;

		/// <summary>
		/// Whether current sanity is visible in the stream overlay window
		/// </summary>
		public bool IsSanityVisible
		{
			get => _isSanityVisible;
			set => Set(ref _isSanityVisible, value);
		}
		private bool _isSanityVisible = true;

		/// <summary>
		/// The current elapsed time displayed in the general purpose timer
		/// </summary>
		public TimeSpan TimerElapsedTime
		{
			get => _timerElapsedTime;
			set => Set(ref _timerElapsedTime, value);
		}
		private TimeSpan _timerElapsedTime;

		/// <summary>
		/// Whether the general purpose timer is currently running
		/// </summary>
		public bool? IsTimerRunning
		{
			get => mIsTimerRunning;
			set
			{
				if (Set(ref mIsTimerRunning, value))
				{
					if (value.HasValue && value.Value) RestartTimer();
					else StopTimer();
				}
			}
		}

		/// <summary>
		/// The current sanity
		/// </summary>
		public double CurrentSanity
		{
			get => _currentSanity;
			set => Set(ref _currentSanity, value);
		}
		private double _currentSanity = 1.0;

		/// <summary>
		/// Whether sanity is currently passively draining
		/// </summary>
		public bool? IsSanityDraining
		{
			get => mIsSanityDraining;
			set
			{
				if (Set(ref mIsSanityDraining, value))
				{
					if (value.HasValue && value.Value) StartSanityDrain();
					else StopSanityDrain();
				}
			}
		}

		/// <summary>
		/// Resets all transient state in the application
		/// </summary>
		public ICommand ResetDataCommand => mResetDataCommand;

		/// <summary>
		/// Sets the general purpose tiemr to 0 and starts it
		/// </summary>
		public ICommand RestartTimerCommand => mRestartTimerCommand;

		/// <summary>
		/// Stops the general purpose timer and sets it to 0
		/// </summary>
		public ICommand StopTimerCommand => mStopTimerCommand;

		/// <summary>
		/// Starts or stops passive sanity drain
		/// </summary>
		public ICommand ToggleSanityDrainCommand => mToggleSanityDrainCommand;

		/// <summary>
		/// Resets current sanity to starting sanity and stops passive sanity drain
		/// </summary>
		public ICommand ResetSanityDrainCommand => mResetSanityDrainCommand;

		/// <summary>
		/// Instantly remove 5 sanity
		/// </summary>
		public ICommand Lose5SanityCommand => mLose5SanityCommand;

		/// <summary>
		/// Add to current sanity an amount equal to the current sanity medication value
		/// </summary>
		public ICommand UseSanityMedicationCommand => mUseSanityMedicationCommand;

		/// <summary>
		/// Toggle ghost hunting speed between normal and unset
		/// </summary>
		public ICommand ToggleNormalSpeedCommand => mToggleNormalSpeedCommand;

		/// <summary>
		/// Toggle ghost hunting speed between abnormal and unset
		/// </summary>
		public ICommand ToggleAbnormalSpeedCommand => mToggleAbnormalSpeedCommand;

		/// <summary>
		/// Creates a new instance of the MainWindowVM class
		/// </summary>
		public MainWindowVM()
		{
			mInputManager = new();

			mGameSettings = new();
			mGameSettings.PropertyChanged += GameSettings_PropertyChanged;

			mTimer = new(TimeSpan.FromMilliseconds(100.0), DispatcherPriority.Normal, Timer_Tick, Dispatcher.CurrentDispatcher)
			{
				IsEnabled = false
			};
			mSanityTimer = new(TimeSpan.FromMilliseconds(100.0), DispatcherPriority.Normal, SanityTimer_Tick, Dispatcher.CurrentDispatcher)
			{
				IsEnabled = false
			};

			mResetDataCommand = new(ResetData);

			mToggleNormalSpeedCommand = new(() => IsNormalSpeed = IsNormalSpeed.HasValue && IsNormalSpeed.Value ? null : true);
			mToggleAbnormalSpeedCommand = new(() => IsNormalSpeed = IsNormalSpeed.HasValue && !IsNormalSpeed.Value ? null : false);

			mRestartTimerCommand = new(() =>
			{
				mIsTimerRunning = true;
				NotifyPropertyChanged(nameof(IsTimerRunning));
				RestartTimer();
			});

			mStopTimerCommand = new(() =>
			{
				mIsTimerRunning = null;
				NotifyPropertyChanged(nameof(IsTimerRunning));
				StopTimer();
			});

			mToggleSanityDrainCommand = new(() =>
			{
				if (mIsSanityDraining.HasValue && mIsSanityDraining.Value)
				{
					mIsSanityDraining = null;
					StopSanityDrain();
				}
				else
				{
					mIsSanityDraining = true;
					StartSanityDrain();
				}
				NotifyPropertyChanged(nameof(IsSanityDraining));
			});
			mResetSanityDrainCommand = new(() =>
			{
				mIsSanityDraining = null;
				ResetSanityDrain();
				NotifyPropertyChanged(nameof(IsSanityDraining));
			});
			mLose5SanityCommand = new(() =>
			{
				CurrentSanity = Math.Max(0.0, CurrentSanity - 0.05);
			});
			mUseSanityMedicationCommand = new(() =>
			{
				CurrentSanity = Math.Min(1.0, CurrentSanity + mGameSettings.SanityMedicationAmount);
			});

			mSettingsDialog = new(mGameSettings);
			mSettingsDialog.Closing += SettingsDialog_Closing;

			Evidences = new();
			Evidences.EvidencesUpdated += Evidences_Updated;

			Ghosts = new();
			Ghosts.PropertyChanged += Ghosts_PropertyChanged;

			BuildInputMap();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				mInputManager.Dispose();
				Evidences.Dispose();
				Ghosts.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Called when the main window has loaded
		/// </summary>
		public void OnMainWindowLoaded()
		{
		}

		/// <summary>
		/// Called when the application is about to exit
		/// </summary>
		public void OnApplicationExiting()
		{
			mSettingsDialog.Closing -= SettingsDialog_Closing;
			mSettingsDialog.Close();
		}

		/// <summary>
		/// Set a reference to the stream overlay window
		/// </summary>
		/// <param name="overlayWindow">the overlay window</param>
		public void SetOverlayWindow(Window? overlayWindow)
		{
			mOverlayWindow = overlayWindow;
		}

		/// <summary>
		/// Reset all transient state data
		/// </summary>
		public void ResetData()
		{
			Evidences.ResetData();
			Ghosts.ResetData();
			IsNormalSpeed = null;
			IsTimerRunning = null;
			IsSanityDraining = null;
			ResetSanityDrain();
		}

		private void BuildInputMap()
		{
			mInputManager.AddInput(new InputAction("Reset Data", new CustomKeyBinding(ResetDataCommand, Key.R, ModifierKeys.Alt)));

			mInputManager.AddInput(new InputAction("Toggle Normal Speed", new CustomKeyBinding(ToggleNormalSpeedCommand, Key.Back, ModifierKeys.None)));
			mInputManager.AddInput(new InputAction("Toggle Abnormal Speed", new CustomKeyBinding(ToggleAbnormalSpeedCommand, Key.Back, ModifierKeys.Shift)));

			mInputManager.AddInput(new InputAction("Restart Timer", new CustomKeyBinding(RestartTimerCommand, Key.D1, ModifierKeys.None)));
			mInputManager.AddInput(new InputAction("Stop Timer", new CustomKeyBinding(StopTimerCommand, Key.D1, ModifierKeys.Shift)));

			mInputManager.AddInput(new InputAction("Toggle Sanity Drain", new CustomKeyBinding(ToggleSanityDrainCommand, Key.D2, ModifierKeys.None)));
			mInputManager.AddInput(new InputAction("Reset Sanity Drain", new CustomKeyBinding(ResetSanityDrainCommand, Key.D2, ModifierKeys.Shift)));
			mInputManager.AddInput(new InputAction("Lose 5 Sanity", new CustomKeyBinding(Lose5SanityCommand, Key.D3, ModifierKeys.None)));
			mInputManager.AddInput(new InputAction("Use Sanity Medication", new CustomKeyBinding(UseSanityMedicationCommand, Key.D3, ModifierKeys.Shift)));

			Ghosts.AddInputs(mInputManager);
			Evidences.AddInputs(mInputManager);
		}

		private void OpenSettingsDialog(bool open)
		{
			if (open)
			{
				if (!mSettingsDialog.IsVisible)
				{
					mSettingsDialog.Show();
					mSettingsDialog.WindowState = WindowState.Normal;
				}
			}
			else
			{
				if (mSettingsDialog.IsVisible)
				{
					mSettingsDialog.Hide();
				}
			}
		}

		private void RestartTimer()
		{
			mTimerStartTime = DateTime.Now;
			mTimer.Start();
			TimerElapsedTime = TimeSpan.Zero;
		}

		private void StopTimer()
		{
			mTimer.Stop();
			TimerElapsedTime = TimeSpan.Zero;
		}

		private void StartSanityDrain()
		{
			mSanityTimerUpdateTime = DateTime.Now;
			mSanityTimer.Start();
		}

		private void StopSanityDrain()
		{
			if (mSanityTimer.IsEnabled)
			{
				mSanityTimer.Stop();
				OnSanityDrainInterval(DateTime.Now - mSanityTimerUpdateTime);
				mSanityTimerUpdateTime = DateTime.Now;
			}
		}

		private void ResetSanityDrain()
		{
			mSanityTimer.Stop();
			mSanityTimerUpdateTime = DateTime.Now;
			CurrentSanity = mGameSettings.StartingSanity;
		}

		private void Evidences_Updated(object? sender, EvidenceSummaryArgs e)
		{
			OnEvidenceUpdated(e);
		}

		private void SettingsDialog_Closing(object? sender, CancelEventArgs e)
		{
			e.Cancel = true;
			mSettingsDialog.Hide();
			mIsSettingsDialogOpen = false;
			NotifyPropertyChanged(nameof(IsSettingsDialogOpen));
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			TimerElapsedTime = DateTime.Now - mTimerStartTime;
		}

		private void SanityTimer_Tick(object? sender, EventArgs e)
		{
			OnSanityDrainInterval(DateTime.Now - mSanityTimerUpdateTime);
			mSanityTimerUpdateTime = DateTime.Now;
		}

		private void GameSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(GameSettings.EvidenceCount):
					OnEvidenceUpdated(Evidences.GatherCurrentEvidence());
					break;
				case nameof(GameSettings.StartingSanity):
					if (!mIsSanityDraining.HasValue || !mIsSanityDraining.Value)
					{
						CurrentSanity = mGameSettings.StartingSanity;
					}
					break;
			}
		}

		private void OnSanityDrainInterval(TimeSpan elapsed)
		{
			double rate = mGameSettings.MapSize switch
			{
				MapSize.Small => 0.0012,
				MapSize.Medium => 0.0008,
				MapSize.Large => 0.0005,
				_ => 0.0012
			};

			rate *= mGameSettings.SanityDrainMultiplier;
			if (mGameSettings.IsMultiplayer)
			{
				rate *= 2.0;
			}

			double amount = elapsed.TotalSeconds * rate;

			if (CurrentSanity >= amount)
			{
				CurrentSanity -= amount;
			}
			else
			{
				CurrentSanity = 0.0;
			}
		}

		private void OnEvidenceUpdated(EvidenceSummaryArgs args)
		{
			Ghosts.ApplyEvidences(mGameSettings.EvidenceCount, args, IsNormalSpeed);

			Evidences possibleEvidences = args.SelectedEvidences;

			if (args.SelectedEvidenceCount >= mGameSettings.EvidenceCount)
			{
				foreach (GhostVM ghost in Ghosts.Ghosts)
				{
					if (ghost.IsAvailable)
					{
						possibleEvidences |= ghost.Ghost.FakeEvidence;
						if (ghost.Ghost.FakeEvidence != PhasmoTracker.Evidences.None)
						{
							int selectedCount = args.SelectedEvidenceCount;
							if ((ghost.Ghost.FakeEvidence & args.SelectedEvidences) != PhasmoTracker.Evidences.None)
							{
								--selectedCount;
							}
							if (selectedCount < mGameSettings.EvidenceCount)
							{
								possibleEvidences |= ghost.Ghost.Evidences;
							}
						}
					}
				}
			}
			else
			{
				foreach (GhostVM ghost in Ghosts.Ghosts)
				{
					if (ghost.IsAvailable)
					{
						possibleEvidences |= ghost.Ghost.Evidences;
						possibleEvidences |= ghost.Ghost.FakeEvidence;
					}
				}
			}

			Evidences.UpdateAvailableEvidences(possibleEvidences);
		}

		private void Ghosts_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			GhostsVM instance = (GhostsVM)sender!;
			if (e.PropertyName == nameof(GhostsVM.HighlightedGhost))
			{
				Evidences.HighlightGhostEvidences(instance.HighlightedGhost);
			}
		}
	}
}
