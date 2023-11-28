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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace PhasmoTracker
{
	/// <summary>
	/// View model for all available game evidences
	/// </summary>
	internal class EvidencesVM : ViewModelBase
	{
		private readonly EvidenceVM[] mEvidences;

		/// <summary>
		/// View models for all game evidences
		/// </summary>
		public IReadOnlyList<EvidenceVM> Evidences => mEvidences;

		/// <summary>
		/// Fires when the state of one or more game evidences has changed
		/// </summary>
		public event EventHandler<EvidenceSummaryArgs>? EvidencesUpdated;

		public EvidencesVM()
		{
			mEvidences = new EvidenceVM[]
			{
				new EvidenceVM(PhasmoTracker.Evidences.Emf),
				new EvidenceVM(PhasmoTracker.Evidences.Dots),
				new EvidenceVM(PhasmoTracker.Evidences.Ultraviolet),
				new EvidenceVM(PhasmoTracker.Evidences.Orbs),
				new EvidenceVM(PhasmoTracker.Evidences.Writing),
				new EvidenceVM(PhasmoTracker.Evidences.Box),
				new EvidenceVM(PhasmoTracker.Evidences.Freezing)
			};

			foreach (EvidenceVM evidence in mEvidences)
			{
				evidence.PropertyChanged += Evidence_PropertyChanged;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				EvidencesUpdated = null;
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Reset the state of all game evidences
		/// </summary>
		public void ResetData()
		{
			foreach (EvidenceVM evidence in mEvidences)
			{
				evidence.Reset();
			}
		}

		/// <summary>
		/// Returns information about the state of all game evidences
		/// </summary>
		public EvidenceSummaryArgs GatherCurrentEvidence()
		{
			Evidences selected = PhasmoTracker.Evidences.None;
			Evidences rejected = PhasmoTracker.Evidences.None;
			int selectedCount = 0;
			foreach (EvidenceVM evidence in mEvidences)
			{
				if (evidence.IsSelected == true)
				{
					selected |= evidence.Evidence;
					++selectedCount;
				}
				if (evidence.IsSelected == false)
				{
					rejected |= evidence.Evidence;
				}
			}

			return new EvidenceSummaryArgs(selected, rejected, selectedCount);
		}

		/// <summary>
		/// Updates the availability status of all evidences
		/// </summary>
		/// <param name="availableEvidences">The currently available evidences</param>
		public void UpdateAvailableEvidences(Evidences availableEvidences)
		{
			foreach (EvidenceVM evidence in mEvidences)
			{
				evidence.IsAvailable = (evidence.Evidence & availableEvidences) != PhasmoTracker.Evidences.None;
			}
		}

		/// <summary>
		/// Highlights evidences associated with a specific ghost
		/// </summary>
		/// <param name="ghost">The ghost to highlight evidence for</param>
		public void HighlightGhostEvidences(GhostVM? ghost)
		{
			if (ghost is null)
			{
				foreach (EvidenceVM evidence in Evidences)
				{
					evidence.IsHightlighted = false;
				}
			}
			else
			{
				foreach (EvidenceVM evidence in Evidences)
				{
					evidence.IsHightlighted =
						(ghost.Ghost.Evidences & evidence.Evidence) != PhasmoTracker.Evidences.None ||
						(ghost.Ghost.FakeEvidence & evidence.Evidence) != PhasmoTracker.Evidences.None;
				}
			}
		}

		/// <summary>
		/// Gather evidence inputs and add them to an input manager
		/// </summary>
		/// <param name="inputManager">The manager to add the inputs to</param>
		public void AddInputs(InputManager inputManager)
		{
			Key[] keys =
			{
				Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0
			};

			for (int e = 0; e < 7; ++e)
			{
				EvidenceVM evidence = Evidences[e];
				inputManager.AddInput(new InputAction($"Select {evidence.Name}", new CustomKeyBinding(evidence.ToggleSelectedCommand, keys[e], ModifierKeys.None)));
				inputManager.AddInput(new InputAction($"Reject {evidence.Name}", new CustomKeyBinding(evidence.ToggleRejectedCommand, keys[e], ModifierKeys.Shift)));
			}
		}

		private void Evidence_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EvidenceVM.IsSelected))
			{
				OnEvidenceUpdated();
			}
		}

		private void OnEvidenceUpdated()
		{
			var updatedEvent = EvidencesUpdated;
			if (updatedEvent is not null)
			{
				updatedEvent.Invoke(this, GatherCurrentEvidence());
			}
		}
	}

	/// <summary>
	/// Data about the current state of all evidences
	/// </summary>
	internal class EvidenceSummaryArgs : EventArgs
	{
		/// <summary>
		/// The currently selected evidences
		/// </summary>
		public Evidences SelectedEvidences { get; }

		/// <summary>
		/// The currently rejected evidences
		/// </summary>
		public Evidences RejectedEvidences { get; }

		/// <summary>
		/// The count of currently selected evidences
		/// </summary>
		public int SelectedEvidenceCount { get; }

		public EvidenceSummaryArgs(Evidences selectedEvidences, Evidences rejectedEvidences, int selectedEvidenceCount)
		{
			SelectedEvidences = selectedEvidences;
			RejectedEvidences = rejectedEvidences;
			SelectedEvidenceCount = selectedEvidenceCount;
		}
	}
}
