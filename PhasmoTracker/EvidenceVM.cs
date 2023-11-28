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
using System.Windows.Input;

namespace PhasmoTracker
{
	/// <summary>
	/// View model for a single game evidence
	/// </summary>
	internal class EvidenceVM : ObservableObject
	{
		private DelegateCommand mToggleSelectedCommand;
		private DelegateCommand mToggleRejectedCommand;

		/// <summary>
		/// The game evidence represented by this VM
		/// </summary>
		public Evidences Evidence { get; }

		/// <summary>
		/// The name of the evidence
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Whether the evidence is currently selected (true), rejected (false), or unset (null)
		/// </summary>
		public bool? IsSelected
		{
			get => _isSelected;
			set => Set(ref _isSelected, value);
		}
		private bool? _isSelected;

		/// <summary>
		/// Whether the evidence is currently available
		/// </summary>
		public bool IsAvailable
		{
			get => _isAvailable;
			set => Set(ref _isAvailable, value);
		}
		private bool _isAvailable = true;

		/// <summary>
		/// Whether the evidence is currently highlighted
		/// </summary>
		public bool IsHightlighted
		{
			get => _isHighlighted;
			set => Set(ref _isHighlighted, value);
		}
		private bool _isHighlighted;

		/// <summary>
		/// Toggle between selected and unset states
		/// </summary>
		public ICommand ToggleSelectedCommand => mToggleSelectedCommand;

		/// <summary>
		/// Toggle between rejected and unset states
		/// </summary>
		public ICommand ToggleRejectedCommand => mToggleRejectedCommand;

		public EvidenceVM(Evidences evidence)
		{
			mToggleSelectedCommand = new(() => IsSelected = IsSelected.HasValue && IsSelected.Value ? null : true);
			mToggleRejectedCommand = new(() => IsSelected = IsSelected.HasValue && !IsSelected.Value ? null : false);

			Evidence = evidence;
			Name = evidence switch
			{
				Evidences.None => "None",
				Evidences.Emf => "EMF Level 5",
				Evidences.Ultraviolet => "Ultraviolet",
				Evidences.Writing => "Ghost Writing",
				Evidences.Freezing => "Freezing Temperatures",
				Evidences.Dots => "D.O.T.S. Projector",
				Evidences.Orbs => "Ghost Orb",
				Evidences.Box => "Spirit Box",
				_ => $"Unknown ({(int)evidence:x8})"
			};
		}

		/// <summary>
		/// Reset the state of the game evidence
		/// </summary>
		public void Reset()
		{
			IsSelected = null;
			IsAvailable = true;
		}
	}
}
