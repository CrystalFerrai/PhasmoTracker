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
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhasmoTracker
{
	/// <summary>
	/// Specifies image sources for all of the images included in this module
	/// </summary>
	internal static class Images
	{
		/// <summary>
		/// Action and state icons
		/// </summary>
		internal static class Icons
		{
			public static ImageSource Dialog { get; private set; }
			public static ImageSource Keyboard { get; private set; }
			public static ImageSource Sanity { get; private set; }
			public static ImageSource Settings { get; private set; }

#nullable disable warnings
			static Icons()
			{
				LoadImages(typeof(Icons));
			}
#nullable restore warnings
		}

		/// <summary>
		/// Loads images and sets the image properties for a type
		/// </summary>
		/// <param name="type">The type to set properties on</param>
		private static void LoadImages(Type type)
		{
			foreach (PropertyInfo property in type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType.IsAssignableFrom(typeof(BitmapImage))))
			{
				property.SetValue(null, new BitmapImage(ResourceHelper.GetResourceUri(string.Format("/Resources/{0}/{1}.png", type.Name, property.Name))), null);
			}
		}
	}
}
