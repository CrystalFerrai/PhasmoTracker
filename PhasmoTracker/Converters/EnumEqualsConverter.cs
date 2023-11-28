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
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PhasmoTracker.Converters
{
	/// <summary>
	/// Converts an enum value into a bool value based on whether it matches a parameter
	/// </summary>
	/// <typeparam name="T">The type of the enum</typeparam>
	internal class EnumEqualsConverter<T> : IValueConverter where T : struct
	{
		/// <summary>
		/// Converts an enum value into a bool value based on whether it matches the parameter
		/// </summary>
		/// <param name="value">The value to convert</param>
		/// <param name="targetType">The type of the target value</param>
		/// <param name="parameter">Unused</param>
		/// <param name="culture">Unused</param>
		/// <returns>The converted value, or null if there was an error</returns>
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is T enumVal)
			{
				if (parameter is T tParam)
				{
					return enumVal.Equals(tParam);
				}
				else if (parameter is string strParam)
				{
					if (Enum.TryParse(strParam, out T enumParam))
					{
						return enumVal.Equals(enumParam) ? true : null;
					}
				}
			}

			return DependencyProperty.UnsetValue;
		}

		/// <summary>
		/// Converts an bool value into an enum value based if the value matches the parameter
		/// </summary>
		/// <remarks>
		/// A value of "true" will return the passed in parameter. A value of "false" will do nothing.
		/// </remarks>
		/// <param name="value">The value to convert</param>
		/// <param name="targetType">The type of the target value</param>
		/// <param name="parameter">Unused</param>
		/// <param name="culture">Unused</param>
		/// <returns>The converted value, or null if there was an error</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool? boolVal = value as bool?;
			if (parameter is T tParam)
			{
				if (boolVal.HasValue && boolVal.Value) return tParam;
				return Binding.DoNothing;
			}
			else if (parameter is string strParam)
			{
				if (Enum.TryParse(strParam, out T enumParam))
				{
					if (boolVal.HasValue && boolVal.Value) return enumParam;
					return Binding.DoNothing;
				}
			}

			return DependencyProperty.UnsetValue;
		}
	}
}
