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

using System;
using System.Collections.Generic;

namespace PhasmoTracker
{
	/// <summary>
	/// Data about a single game ghost
	/// </summary>
	internal class Ghost : IEquatable<Ghost>
	{
		/// <summary>
		/// The name of the ghost
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The possible evidences provided by the ghost
		/// </summary>
		public Evidences Evidences { get; }

		/// <summary>
		/// The evidence guaranteed by the ghost if any evidence is available
		/// </summary>
		public Evidences ForcedEvidence { get; }

		/// <summary>
		/// An evidence that is presented but not counted as evidence
		/// </summary>
		public Evidences FakeEvidence { get; }

		/// <summary>
		/// Whether the ghost hunts at the standard ghost speed (true), a different speed (false), or has a more complex or variable speed (null)
		/// </summary>
		public bool? IsNormalSpeed { get; }

		public Ghost(string name, Evidences evidences, bool? isNormalSpeed = true, Evidences forcedEvidence = Evidences.None, Evidences fakeEvidence = Evidences.None)
		{
			Name = name;
			Evidences = evidences;
			ForcedEvidence = forcedEvidence;
			FakeEvidence = fakeEvidence;
			IsNormalSpeed = isNormalSpeed;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public bool Equals(Ghost? other)
		{
			return other is not null && Name.Equals(other.Name);
		}

		public override bool Equals(object? obj)
		{
			return obj is Ghost other && Equals(other);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// A list of all possible game ghosts
	/// </summary>
	internal static class Ghosts
	{
		public static readonly Ghost Spirit = new(nameof(Spirit), Evidences.Emf | Evidences.Box | Evidences.Writing);
		public static readonly Ghost Wraith = new(nameof(Wraith), Evidences.Emf | Evidences.Box | Evidences.Dots);
		public static readonly Ghost Phantom = new(nameof(Phantom), Evidences.Ultraviolet | Evidences.Box | Evidences.Dots);
		public static readonly Ghost Poltergeist = new(nameof(Poltergeist), Evidences.Ultraviolet | Evidences.Box | Evidences.Writing);
		public static readonly Ghost Banshee = new(nameof(Banshee), Evidences.Ultraviolet | Evidences.Orbs| Evidences.Dots);
		public static readonly Ghost Jinn = new(nameof(Jinn), Evidences.Emf | Evidences.Ultraviolet | Evidences.Freezing);
		public static readonly Ghost Mare = new(nameof(Mare), Evidences.Orbs | Evidences.Box | Evidences.Writing);
		public static readonly Ghost Revenant = new(nameof(Revenant), Evidences.Orbs | Evidences.Freezing | Evidences.Writing, false);
		public static readonly Ghost Shade = new(nameof(Shade), Evidences.Emf | Evidences.Freezing | Evidences.Writing);
		public static readonly Ghost Demon = new(nameof(Demon), Evidences.Ultraviolet | Evidences.Freezing | Evidences.Writing);
		public static readonly Ghost Yurei = new(nameof(Yurei), Evidences.Orbs | Evidences.Freezing | Evidences.Dots);
		public static readonly Ghost Oni = new(nameof(Oni), Evidences.Emf | Evidences.Freezing | Evidences.Dots);
		public static readonly Ghost Yokai = new(nameof(Yokai), Evidences.Orbs | Evidences.Box | Evidences.Dots);
		public static readonly Ghost Hantu = new(nameof(Hantu), Evidences.Ultraviolet | Evidences.Orbs | Evidences.Freezing, false, Evidences.Freezing);
		public static readonly Ghost Goryo = new(nameof(Goryo), Evidences.Emf | Evidences.Ultraviolet | Evidences.Dots, true, Evidences.Dots);
		public static readonly Ghost Myling = new(nameof(Myling), Evidences.Emf | Evidences.Ultraviolet | Evidences.Writing);
		public static readonly Ghost Onryo = new(nameof(Onryo), Evidences.Orbs | Evidences.Box | Evidences.Freezing);
		public static readonly Ghost Twins = new($"The {nameof(Twins)}", Evidences.Emf | Evidences.Box | Evidences.Freezing, null);
		public static readonly Ghost Raiju = new(nameof(Raiju), Evidences.Emf | Evidences.Orbs | Evidences.Dots, null);
		public static readonly Ghost Obake = new(nameof(Obake), Evidences.Emf | Evidences.Ultraviolet | Evidences.Orbs, true, Evidences.Ultraviolet);
		public static readonly Ghost Mimic = new($"The {nameof(Mimic)}", Evidences.Ultraviolet | Evidences.Box | Evidences.Freezing, null, Evidences.None, Evidences.Orbs);
		public static readonly Ghost Moroi = new(nameof(Moroi), Evidences.Freezing | Evidences.Box | Evidences.Writing, false, Evidences.Box);
		public static readonly Ghost Deogen = new(nameof(Deogen), Evidences.Dots | Evidences.Box | Evidences.Writing, false, Evidences.Box);
		public static readonly Ghost Thaye = new(nameof(Thaye), Evidences.Orbs | Evidences.Dots | Evidences.Writing, false);

		public static IReadOnlyList<Ghost> All { get; }

		static Ghosts()
		{
			All = new Ghost[]
			{
				Spirit, Wraith, Phantom,
				Poltergeist, Banshee, Jinn,
				Mare, Revenant, Shade,
				Demon, Yurei, Oni,
				Yokai, Hantu, Goryo,
				Myling, Onryo, Twins,
				Raiju, Obake, Mimic,
				Moroi, Deogen, Thaye
			};
		}
	}
}
