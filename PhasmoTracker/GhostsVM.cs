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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PhasmoTracker
{
	/// <summary>
	/// View model for all available game ghosts
	/// </summary>
	internal class GhostsVM : ViewModelBase
	{
		private readonly GhostVM[] mGhosts;

		/// <summary>
		/// List of all ghosts
		/// </summary>
		public IReadOnlyList<GhostVM> Ghosts => mGhosts;

		/// <summary>
		/// A ghost currently being highlighted
		/// </summary>
		public GhostVM? HighlightedGhost
		{
			get => _highlightedGhost;
			set => Set(ref _highlightedGhost, value);
		}
		private GhostVM? _highlightedGhost;

		public GhostsVM()
		{
			mGhosts = PhasmoTracker.Ghosts.All.Select(g => new GhostVM(g)).ToArray();
			foreach (GhostVM ghost in mGhosts)
			{
				ghost.PropertyChanged += Ghost_PropertyChanged;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (GhostVM ghost in mGhosts)
				{
					ghost.PropertyChanged -= Ghost_PropertyChanged;
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Reset the state of all ghosts
		/// </summary>
		public void ResetData()
		{
			foreach (GhostVM ghost in mGhosts)
			{
				ghost.Reset();
			}
		}

		/// <summary>
		/// Update the state of all ghosts based on the current state of evidences
		/// </summary>
		/// <param name="evidenceAvailableCount">The number of evidence available in the current game difficulty</param>
		/// <param name="args">Information about the current state of evidences</param>
		/// <param name="isNormalSpeed">Whether the current ghost hunts at normal speed</param>
		public void ApplyEvidences(int evidenceAvailableCount, EvidenceSummaryArgs args, bool? isNormalSpeed)
		{
			foreach (GhostVM ghost in Ghosts)
			{
				ghost.ApplyEvidences(evidenceAvailableCount, args, isNormalSpeed);
			}
		}

		/// <summary>
		/// Adds ghost selection inputs to an input manager
		/// </summary>
		/// <param name="inputManager">The manager to add inputs to</param>
		public void AddInputs(InputManager inputManager)
		{
			Key[] keys =
			{
				Key.NumPad7, Key.NumPad8, Key.NumPad9,
				Key.NumPad4, Key.NumPad5, Key.NumPad6,
				Key.NumPad1, Key.NumPad2, Key.NumPad3,
				Key.NumPad0, Key.Decimal, Key.F24
			};

			for (int group = 0; group <= 12; group += 12)
			{
				ModifierKeys modifier = group == 0 ? ModifierKeys.Shift : ModifierKeys.None;
				for (int g = 0; g < 12; ++g)
				{
					GhostVM ghost = Ghosts[group + g];
					inputManager.AddInput(new InputAction($"Select {ghost.Ghost.Name}", new CustomKeyBinding(ghost.ToggleSelectedCommand, keys[g], modifier)));
				}
			}
		}

		private void Ghost_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			GhostVM instance = (GhostVM)sender!;
			if (e.PropertyName == nameof(GhostVM.IsSelected) && instance.IsSelected == true)
			{
				foreach (GhostVM ghost in mGhosts)
				{
					if (ghost == instance) continue;
					if (ghost.IsSelected == true)
					{
						ghost.IsSelected = null;
					}
				}
			}
			else if (e.PropertyName == nameof(GhostVM.IsHightlighted))
			{
				if (instance.IsHightlighted)
				{
					HighlightedGhost = instance;
				}
				else if (!(HighlightedGhost?.IsHightlighted ?? false))
				{
					HighlightedGhost = null;
				}
			}
		}
	}
}
