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
	/// View model for a single game ghost
	/// </summary>
	internal class GhostVM : ObservableObject
	{
		private DelegateCommand mToggleSelectedCommand;

		/// <summary>
		/// The ghost
		/// </summary>
		public Ghost Ghost { get; }

		/// <summary>
		/// Whether the ghost is currently selected (true), rejected (false), or unset (null)
		/// </summary>
		public bool? IsSelected
		{
			get => _isSelected;
			set => Set(ref _isSelected, value);
		}
		private bool? _isSelected;

		/// <summary>
		/// Whether the ghost is currently available/possible
		/// </summary>
		public bool IsAvailable
		{
			get => _isAvailable;
			private set => Set(ref _isAvailable, value);
		}
		private bool _isAvailable = true;

		/// <summary>
		/// Whether the ghost is currently being highlighted
		/// </summary>
		public bool IsHightlighted
		{
			get => _isHighlighted;
			set => Set(ref _isHighlighted, value);
		}
		private bool _isHighlighted;

		/// <summary>
		/// Toggles between rejected and unset states
		/// </summary>
		public ICommand ToggleSelectedCommand => mToggleSelectedCommand;

		public GhostVM(Ghost ghost)
		{
			mToggleSelectedCommand = new(() => IsSelected = IsSelected.HasValue ? null : false);
			Ghost = ghost;
		}

		/// <summary>
		/// Resets the state of the ghost
		/// </summary>
		public void Reset()
		{
			IsSelected = null;
			IsAvailable = true;
		}

		/// <summary>
		/// Updates the state of the ghost based on the current state of evidences
		/// </summary>
		/// <param name="evidenceAvailableCount">The number of evidence available in the current game difficulty</param>
		/// <param name="args">Information about the current state of evidences</param>
		/// <param name="isNormalSpeed">Whether the current ghost hunts at normal speed</param>
		public void ApplyEvidences(int evidenceAvailableCount, EvidenceSummaryArgs args, bool? isNormalSpeed)
		{
			if (Ghost.IsNormalSpeed.HasValue && isNormalSpeed.HasValue)
			{
				if (Ghost.IsNormalSpeed.Value != isNormalSpeed.Value)
				{
					IsAvailable = false;
					return;
				}
			}

			Evidences selected = args.SelectedEvidences;
			Evidences rejected = args.RejectedEvidences;
			int selectedCount = args.SelectedEvidenceCount;

			if ((selected & Ghost.FakeEvidence) != 0)
			{
				selected &= ~Ghost.FakeEvidence;
				--selectedCount;
			}

			if (selectedCount > evidenceAvailableCount)
			{
				IsAvailable = false;
				return;
			}

			if (evidenceAvailableCount > 0 && Ghost.ForcedEvidence != Evidences.None && selectedCount >= evidenceAvailableCount)
			{
				if ((selected & Ghost.ForcedEvidence) == Evidences.None)
				{
					IsAvailable = false;
					return;
				}
			}

			if (((selected ^ Ghost.Evidences) & selected) != Evidences.None)
			{
				IsAvailable = false;
				return;
			}

			if (evidenceAvailableCount > 0 && (rejected & Ghost.ForcedEvidence) != Evidences.None)
			{
				IsAvailable = false;
				return;
			}

			Evidences rejectedDiff = (rejected ^ Ghost.Evidences) & Ghost.Evidences;
			int rejectedDiffCount = CountEvidences(rejectedDiff);

			if (rejectedDiffCount < evidenceAvailableCount || (rejected & Ghost.FakeEvidence) != Evidences.None)
			{
				IsAvailable = false;
				return;
			}

			IsAvailable = true;
		}

		private static int CountEvidences(Evidences evidences)
		{
			int i = (int)evidences;
			i = i - ((i >> 1) & 0x55555555);
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
		}
	}
}
