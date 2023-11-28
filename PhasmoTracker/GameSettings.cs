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

using PhasmoTracker.Converters;
using PhasmoTracker.Utils;
using System.Windows.Input;

namespace PhasmoTracker
{
	/// <summary>
	/// Settings from the game that are used by this application
	/// </summary>
	internal class GameSettings : ObservableObject
	{
		private static readonly double[] StartingSanityList;
		private static readonly double[] SanityDrainMultiplierList;
		private static readonly double[] SanityMedicationList;

		private readonly DelegateCommand mDecreaseStartingSanityCommand;
		private readonly DelegateCommand mIncreaseStartingSanityCommand;

		private readonly DelegateCommand mDecreaseSanityDrainCommand;
		private readonly DelegateCommand mIncreaseSanityDrainCommand;

		private readonly DelegateCommand mIncreaseSanityMedicationCommand;
		private readonly DelegateCommand mDecreaseSanityMedicationCommand;

		private int mStartingSanityIndex;
		private int mSanityDrainMultiplierIndex;
		private int mSanityMedicationIndex;

		/// <summary>
		/// The number of evidence available
		/// </summary>
		public int EvidenceCount
		{
			get => _evidenceCount;
			set => Set(ref _evidenceCount, value);
		}
		private int _evidenceCount = 3;

		/// <summary>
		/// The size of the map being played
		/// </summary>
		public MapSize MapSize
		{
			get => _mapSize;
			set => Set(ref _mapSize, value);
		}
		private MapSize _mapSize = MapSize.Small;

		/// <summary>
		/// Whether the game is being played with other players
		/// </summary>
		public bool IsMultiplayer
		{
			get => _isMultiplayer;
			set => Set(ref _isMultiplayer, value);
		}
		private bool _isMultiplayer = false;

		/// <summary>
		/// The starting player sanity
		/// </summary>
		public double StartingSanity
		{
			get => StartingSanityList[mStartingSanityIndex];
			set
			{
				int index = StartingSanityList.IndexOf(value);
				if (index >= 0)
				{
					mStartingSanityIndex = index;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// The passive sanity drain multiplier
		/// </summary>
		public double SanityDrainMultiplier
		{
			get => SanityDrainMultiplierList[mSanityDrainMultiplierIndex];
			set
			{
				int index = SanityDrainMultiplierList.IndexOf(value);
				if (index >= 0)
				{
					mSanityDrainMultiplierIndex = index;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// The amount of sanity restored by sanity medication
		/// </summary>
		public double SanityMedicationAmount
		{
			get => SanityMedicationList[mSanityMedicationIndex];
			set
			{
				int index = SanityMedicationList.IndexOf(value);
				if (index >= 0)
				{
					mSanityMedicationIndex = index;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Converts map size enum to bool
		/// </summary>
		public EnumEqualsConverter<MapSize> MapSizeConverter { get; }

		/// <summary>
		/// Decreases the starting sanity
		/// </summary>
		public ICommand DecreaseStartingSanityCommand => mDecreaseStartingSanityCommand;

		/// <summary>
		/// Increases the starting sanity
		/// </summary>
		public ICommand IncreaseStartingSanityCommand => mIncreaseStartingSanityCommand;

		/// <summary>
		/// Decreases the sanity drain
		/// </summary>
		public ICommand DecreaseSanityDrainCommand => mDecreaseSanityDrainCommand;

		/// <summary>
		/// Increases the sanity drain
		/// </summary>
		public ICommand IncreaseSanityDrainCommand => mIncreaseSanityDrainCommand;

		/// <summary>
		/// Decreases the sanity medication effectiveness
		/// </summary>
		public ICommand DecreaseSanityMedicationCommand => mDecreaseSanityMedicationCommand;

		/// <summary>
		/// Increases the sanity medication effectiveness
		/// </summary>
		public ICommand IncreaseSanityMedicationCommand => mIncreaseSanityMedicationCommand;

		static GameSettings()
		{
			// Values match custom difficulty values available in game

			StartingSanityList = new double[]
			{
				0.0,
				0.25,
				0.5,
				0.75,
				1.0
			};

			SanityDrainMultiplierList = new double[]
			{
				0.0,
				0.5,
				1.0,
				1.5,
				2.0
			};

			SanityMedicationList = new double[]
			{
				0.0,
				0.05,
				0.1,
				0.2,
				0.25,
				0.3,
				0.35,
				0.4,
				0.45,
				0.5,
				0.75,
				1.0
			};
		}

		public GameSettings()
		{
			mDecreaseStartingSanityCommand = new(DecreaseStartingSanity, CanDecreaseStartingSanity);
			mIncreaseStartingSanityCommand = new(IncreaseStartingSanity, CanIncreaseStartingSanity);

			mDecreaseSanityDrainCommand = new(DecreaseSanityDrain, CanDecreaseSanityDrain);
			mIncreaseSanityDrainCommand = new(IncreaseSanityDrain, CanIncreaseSanityDrain);

			mDecreaseSanityMedicationCommand = new(DecreaseSanityMedication, CanDecreaseSanityMedication);
			mIncreaseSanityMedicationCommand = new(IncreaseSanityMedication, CanIncreaseSanityMedication);

			// Defaults based on professional difficulty
			mStartingSanityIndex = 4;
			mSanityDrainMultiplierIndex = 4;
			mSanityMedicationIndex = 5;

			MapSizeConverter = new();
		}

		/// <summary>
		/// Decreases the starting sanity
		/// </summary>
		public void DecreaseStartingSanity()
		{
			if (CanDecreaseStartingSanity())
			{
				--mStartingSanityIndex;
				NotifyPropertyChanged(nameof(StartingSanity));
				mDecreaseStartingSanityCommand.RaiseCanExecuteChanged();
				mIncreaseStartingSanityCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Increases the starting sanity
		/// </summary>
		public void IncreaseStartingSanity()
		{
			if (CanIncreaseStartingSanity())
			{
				++mStartingSanityIndex;
				NotifyPropertyChanged(nameof(StartingSanity));
				mDecreaseStartingSanityCommand.RaiseCanExecuteChanged();
				mIncreaseStartingSanityCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Decreases the sanity drain
		/// </summary>
		public void DecreaseSanityDrain()
		{
			if (CanDecreaseSanityDrain())
			{
				--mSanityDrainMultiplierIndex;
				NotifyPropertyChanged(nameof(SanityDrainMultiplier));
				mDecreaseSanityDrainCommand.RaiseCanExecuteChanged();
				mIncreaseSanityDrainCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Increases the sanity drain
		/// </summary>
		public void IncreaseSanityDrain()
		{
			if (CanIncreaseSanityDrain())
			{
				++mSanityDrainMultiplierIndex;
				NotifyPropertyChanged(nameof(SanityDrainMultiplier));
				mDecreaseSanityDrainCommand.RaiseCanExecuteChanged();
				mIncreaseSanityDrainCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Decreases the sanity medication effectiveness
		/// </summary>
		public void DecreaseSanityMedication()
		{
			if (CanDecreaseSanityMedication())
			{
				--mSanityMedicationIndex;
				NotifyPropertyChanged(nameof(SanityMedicationAmount));
				mDecreaseSanityMedicationCommand.RaiseCanExecuteChanged();
				mIncreaseSanityMedicationCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Increases the sanity medication effectiveness
		/// </summary>
		public void IncreaseSanityMedication()
		{
			if (CanIncreaseSanityMedication())
			{
				++mSanityMedicationIndex;
				NotifyPropertyChanged(nameof(SanityMedicationAmount));
				mDecreaseSanityMedicationCommand.RaiseCanExecuteChanged();
				mIncreaseSanityMedicationCommand.RaiseCanExecuteChanged();
			}
		}

		private bool CanDecreaseStartingSanity()
		{
			return mStartingSanityIndex > 0;
		}

		private bool CanIncreaseStartingSanity()
		{
			return mStartingSanityIndex < StartingSanityList.Length - 1;
		}

		private bool CanDecreaseSanityDrain()
		{
			return mSanityDrainMultiplierIndex > 0;
		}

		private bool CanIncreaseSanityDrain()
		{
			return mSanityDrainMultiplierIndex < SanityDrainMultiplierList.Length - 1;
		}

		private bool CanDecreaseSanityMedication()
		{
			return mSanityMedicationIndex > 0;
		}

		private bool CanIncreaseSanityMedication()
		{
			return mSanityMedicationIndex < SanityMedicationList.Length - 1;
		}
	}

	internal enum MapSize
	{
		Small,
		Medium,
		Large
	}
}
