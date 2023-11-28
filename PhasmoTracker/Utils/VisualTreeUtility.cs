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
using System.Windows;
using System.Windows.Media;

namespace PhasmoTracker.Utils
{
	/// <summary>
	/// Functions to help gather information about a visual tree
	/// </summary>
	internal static class VisualTreeUtility
	{
		/// <summary>
		/// Finds the first visual descendant of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the visual child to search for</typeparam>
		/// <param name="ancestor">The ancestor at which to begin searching</param>
		/// <returns>The first descendant of the specified typeparam</returns>
		public static T? FindDescendant<T>(DependencyObject ancestor) where T : DependencyObject
		{
			return FindDescendant<T>(ancestor, item => true);
		}

		/// <summary>
		/// Finds the first visual descendant matching the specified predicate.
		/// The predicate is only called for discovered items of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the visual child to search for</typeparam>
		/// <param name="ancestor">The ancestor at which to begin searching</param>
		/// <param name="predicate">A predicate to determine if the child is the one being searched for</param>
		/// <returns>The first descendant matching the predicate</returns>
		public static T? FindDescendant<T>(DependencyObject ancestor, Predicate<T> predicate) where T : DependencyObject
		{
			return FindDescendant(typeof(T), ancestor, item => predicate((T)item)) as T;
		}

		/// <summary>
		/// Finds the first visual descendant of the specified type.
		/// </summary>
		/// <param name="itemType">The type of the visual child to search for</param>
		/// <param name="ancestor">The ancestor at which to begin searching</param>
		/// <returns>The first descendant of the specified Type</returns>
		public static DependencyObject? FindDescendant(Type itemType, DependencyObject ancestor)
		{
			return FindDescendant(itemType, ancestor, item => true);
		}

		/// <summary>
		/// Finds the first visual descendant matching the specified predicate.
		/// The predicate is only called for discovered items of the specified type.
		/// </summary>
		/// <param name="itemType">The type of the visual child to search for</param>
		/// <param name="ancestor">The ancestor at which to begin searching</param>
		/// <param name="predicate">A predicate to determine if the child is the one being searched for</param>
		/// <returns>The first descendant matching the predicate</returns>
		/// <exception cref="ArgumentNullException">One of the passed in parameters was null</exception>
		/// <exception cref="ArgumentException">The itemType didn't extend DependencyObject</exception>
		public static DependencyObject? FindDescendant(Type itemType, DependencyObject ancestor, Predicate<DependencyObject> predicate)
		{
			if (itemType == null) throw new ArgumentNullException("itemType");
			if (ancestor == null) throw new ArgumentNullException("ancestor");
			if (predicate == null) throw new ArgumentNullException("predicate");
			if (!typeof(DependencyObject).IsAssignableFrom(itemType)) throw new ArgumentException("itemType", "The passed in type must be or extend DependencyObject");

			//Using a queue for a breadth-first traversal rather than depth-first
			Queue<DependencyObject> queue = new Queue<DependencyObject>();
			queue.Enqueue(ancestor);

			while (queue.Count > 0)
			{
				DependencyObject currentChild = queue.Dequeue();
				if (itemType.IsAssignableFrom(currentChild.GetType()))
				{
					if (predicate.Invoke(currentChild))
					{
						return currentChild;
					}
				}

				int count = VisualTreeHelper.GetChildrenCount(currentChild);
				for (int i = 0; i < count; ++i)
				{
					queue.Enqueue(VisualTreeHelper.GetChild(currentChild, i));
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the transform of a child relative to the first parent of the specified type
		/// </summary>
		/// <typeparam name="ParentType">The type of the parent</typeparam>
		/// <param name="child">The child to get the relative tranform for</param>
		/// <exception cref="ArgumentNullException">The passed in child is null</exception>
		/// <exception cref="ArgumentException">The specified child does not have a parent of the specified type</exception>
		public static Transform GetRelativeTransform<ParentType>(Visual child)
		{
			if (child == null) throw new ArgumentNullException("child");

			TransformGroup group = new TransformGroup();
			Transform transform;
			Visual current = child;
			while (!(current is ParentType) && current != null)
			{
				transform = VisualTreeHelper.GetTransform(current);
				if (transform != null) group.Children.Add(transform);
				current = (Visual)VisualTreeHelper.GetParent(current);
			}

			if (current == null) throw new ArgumentException(string.Format("The spcified child \"{0}\" does not have a parent of type \"{1}\"", child.ToString(), typeof(ParentType).FullName));
			return group;
		}
	}
}
