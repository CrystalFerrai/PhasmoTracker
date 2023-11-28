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
using System.Windows;
using System.Windows.Input;

namespace PhasmoTracker
{
	/// <summary>
	/// Manages low level keyboard input for the application
	/// </summary>
	internal class InputManager : IDisposable
	{
		private LowLevelKeyboard mKeyboard;

		private Dictionary<Key, HashSet<InputAction>> mKeyMap;

		public InputManager()
		{
			mKeyboard = new(true);
			mKeyMap = new();

			mKeyboard.KeyDown += Keyboard_KeyDown;
		}

		public void Dispose()
		{
			mKeyboard.Dispose();
		}

		/// <summary>
		/// Enable or disable all input
		/// </summary>
		/// <param name="enable">Whether to enable or disable input</param>
		public void EnableInput(bool enable)
		{
			if (enable)
			{
				mKeyboard.Hook();
			}
			else
			{
				mKeyboard.Unhook();
			}
		}

		/// <summary>
		/// Adds an input action to the manager
		/// </summary>
		/// <param name="input">The action to add</param>
		/// <returns>True if the action was added, or false if an action with the same inpuit sequence was already registered</returns>
		public bool AddInput(InputAction input)
		{
			HashSet<InputAction>? list;
			if (!mKeyMap.TryGetValue(input.KeyBinding.Key, out list))
			{
				list = new();
				mKeyMap.Add(input.KeyBinding.Key, list);
			}

			return list.Add(input);
		}

		/// <summary>
		/// Removes an input action from the manager
		/// </summary>
		/// <param name="input">The action to remove</param>
		/// <returns>True if the action was found and removed, or false if the action was not registered</returns>
		public bool RemoveInput(InputAction input)
		{
			HashSet<InputAction>? list;
			if (mKeyMap.TryGetValue(input.KeyBinding.Key, out list))
			{
				return list.Remove(input);
			}
			return false;
		}

		private void Keyboard_KeyDown(object? sender, LowLevelKeyArgs e)
		{
			HandleInput(e.Key, e.ModifierKeys);
		}

		private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			HandleInput(e.Key, Keyboard.Modifiers);
		}

		private void HandleInput(Key key, ModifierKeys modifiers)
		{
#if DEBUG
			if (modifiers != ModifierKeys.None)
			{
				System.Diagnostics.Debug.Write($"({modifiers})");
			}
			System.Diagnostics.Debug.WriteLine($"↓ {key}");
#endif

			HashSet<InputAction>? list;
			if (mKeyMap.TryGetValue(key, out list))
			{
				foreach (InputAction input in list)
				{
					if (modifiers != input.KeyBinding.Modifiers) continue;
					if (!input.KeyBinding.Command.CanExecute(input.KeyBinding.CommandParameter)) continue;
					input.KeyBinding.Command.Execute(input.KeyBinding.CommandParameter);
				}
			}
		}
	}

	/// <summary>
	/// Data about an input action for use with the input manager
	/// </summary>
	internal class InputAction : IEquatable<InputAction>
	{
		/// <summary>
		/// The display name for the action
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The key sequence that should trigger the action
		/// </summary>
		public CustomKeyBinding KeyBinding { get; set; }

		public InputAction(string name, CustomKeyBinding binding)
		{
			Name = name;
			KeyBinding = binding;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public bool Equals(InputAction? other)
		{
			return other is not null && Name.Equals(other.Name);
		}

		public override bool Equals(object? obj)
		{
			return obj is InputAction other && Equals(other);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// A keyboard to command binding
	/// </summary>
	internal class CustomKeyBinding : ICommandSource
	{
		private string? mLabel;
		private string? mShortLabel;

		/// <summary>
		/// The key that triggers the binding
		/// </summary>
		[TypeConverter(typeof(KeyConverter))]
		public Key Key { get; }

		/// <summary>
		/// Modifier keys that need to be held (when Key is pressed) to trigger the binding
		/// </summary>
		[TypeConverter(typeof(ModifierKeysConverter))]
		public ModifierKeys Modifiers { get; }

		/// <summary>
		/// The command to execute when the binding is triggered
		/// </summary>
		public ICommand Command { get; }

		/// <summary>
		/// A parameter to pass to Command when executing it
		/// </summary>
		public object? CommandParameter { get; }

		/// <summary>
		/// The target for Command, not currently used
		/// </summary>
		public IInputElement? CommandTarget { get; }

		/// <summary>
		/// A string representing the key sequence needed to trigger the binding
		/// </summary>
		public string? Label => mLabel ??= ToString();

		/// <summary>
		/// An abbreviated string indicating the key sequence needed to trigger the binding
		/// </summary>
		public string? ShortLabel => mShortLabel ??= ToShortString();

		public CustomKeyBinding(ICommand command, Key key, ModifierKeys modifiers, object? commandParameter = null, IInputElement? commandTarget = null)
		{
			Key = key;
			Modifiers = modifiers;
			Command = command;
			CommandParameter = commandParameter;
			CommandTarget = commandTarget;
		}

		public override string? ToString()
		{
			string? key = TypeDescriptor.GetConverter(Key).ConvertToString(Key);
			if (Modifiers == ModifierKeys.None)
			{
				return key;
			}

			string? mods = TypeDescriptor.GetConverter(Modifiers).ConvertToString(Modifiers);
			return $"{mods}+{key}";
		}

		public string? ToShortString()
		{
			string? value = ToString();
			if (value is null) return null;
			value = value.Replace("Numpad", string.Empty, StringComparison.OrdinalIgnoreCase);
			if (Modifiers != ModifierKeys.None)
			{
				if ((Modifiers | ModifierKeys.Control) != ModifierKeys.None)
				{
					value = value.Replace("Ctrl+", "c", StringComparison.OrdinalIgnoreCase);
				}
				if ((Modifiers | ModifierKeys.Shift) != ModifierKeys.None)
				{
					value = value.Replace("Shift+", "↑", StringComparison.OrdinalIgnoreCase);
				}
				if ((Modifiers | ModifierKeys.Alt) != ModifierKeys.None)
				{
					value = value.Replace("Alt+", "a", StringComparison.OrdinalIgnoreCase);
				}
				if ((Modifiers | ModifierKeys.Windows) != ModifierKeys.None)
				{
					value = value.Replace("Windows+", "w", StringComparison.OrdinalIgnoreCase);
				}
			}
			return value;
		}
	}
}
