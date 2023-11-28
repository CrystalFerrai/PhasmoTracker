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
    /// Converts a boolean to a visibility value
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of this converter that interprets false as Visibility.Collapsed
        /// </summary>
        public static BoolToVisibilityConverter CollapseInstance { get; private set; }

        /// <summary>
        /// Gets an instance of this converter that interprets false as Visibility.Hidden
        /// </summary>
        public static BoolToVisibilityConverter HideInstance { get; private set; }

        /// <summary>
        /// Gets or sets whether the converter treats false as Visibility.Hidden rather than Visibility.Collapsed
        /// </summary>
        public bool UseHidden { get; set; }

        /// <summary>
        /// Initializes static members of the BoolToVisibilityConverter class
        /// </summary>
        static BoolToVisibilityConverter()
        {
            HideInstance = new BoolToVisibilityConverter() { UseHidden = true };
            CollapseInstance = new BoolToVisibilityConverter() { UseHidden = false };
        }

        /// <summary>
        /// Converts a boolean to a visibility value
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The type of the target value</param>
        /// <param name="parameter">Unused</param>
        /// <param name="culture">Unused</param>
        /// <returns>The converted value, or null if there was an error</returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Visible : !UseHidden ? Visibility.Collapsed : Visibility.Hidden;
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Converts a visibility value to a boolean
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The type of the target value</param>
        /// <param name="parameter">Unused</param>
        /// <param name="culture">Unused</param>
        /// <returns>The converted value, or null if there was an error</returns>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && targetType == typeof(bool))
            {
                return (Visibility)value == Visibility.Visible;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
