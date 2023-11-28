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

namespace PhasmoTracker.Utils
{
	/// <summary>
	/// Extension methods for generic enumerables
	/// </summary>
	internal static class EnumerableExtensions
	{
		/// <summary>
		/// Returns the index of the first matching item
		/// </summary>
		/// <typeparam name="T">The item type</typeparam>
		/// <param name="collection">The collection to search</param>
		/// <param name="item">The item to locate</param>
		/// <returns>The index of the item if found, else -1</returns>
		/// <exception cref="ArgumentNullException">A passed in parameter is null</exception>
		public static int IndexOf<T>(this IReadOnlyList<T> collection, T item)
		{
			if (collection is null) throw new ArgumentNullException(nameof(collection));
			if (item is null) throw new ArgumentNullException(nameof(item));

			for (int i = 0; i < collection.Count; ++i)
			{
				if (item.Equals(collection[i])) return i;
			}

			return -1;
		}
	}
}
