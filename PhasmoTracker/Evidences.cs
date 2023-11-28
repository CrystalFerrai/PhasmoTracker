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

namespace PhasmoTracker
{
	/// <summary>
	/// Game evidences
	/// </summary>
	[Flags]
	internal enum Evidences
	{
		None = 0x00,
		Emf = 0x01,
		Ultraviolet = 0x02,
		Writing = 0x04,
		Freezing = 0x08,
		Dots = 0x10,
		Orbs = 0x20,
		Box = 0x40
	}

	/// <summary>
	/// Extensions for the Evidences enum
	/// </summary>
	internal static class EvidencesExtensions
	{
		/// <summary>
		/// All Evidences values
		/// </summary>
		public static readonly Evidences[] EnumValues;

		static EvidencesExtensions()
		{
			// Enum.GetValues is not cheap so we cache the result
			EnumValues = Enum.GetValues<Evidences>();
		}
	}
}
