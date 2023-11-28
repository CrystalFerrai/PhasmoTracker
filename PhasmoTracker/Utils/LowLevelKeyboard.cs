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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace PhasmoTracker.Utils
{
	/// <summary>
	/// Utility for observing global low level keyboard input.
	/// </summary>
	internal sealed class LowLevelKeyboard : IDisposable
	{
		private static readonly HashSet<uint> sMessages;

		private readonly HOOKPROC mCallback;

		private readonly bool mRemapNumpad;

		private nint mHook;

		private readonly HashSet<Key> mDownKeys;

		/// <summary>
		/// Event fired when a key is pressed
		/// </summary>
		public event EventHandler<LowLevelKeyArgs>? KeyDown;

		/// <summary>
		/// Event fired each repeat while a key is held. Will also
		/// fire on initial key press immediately after KeyDown.
		/// </summary>
		/// <remarks>
		/// This event is useful for text input interception.
		/// </remarks>
		public event EventHandler<LowLevelKeyArgs>? KeyRepeat;

		/// <summary>
		/// Event fired when a key is released
		/// </summary>
		public event EventHandler<LowLevelKeyArgs>? KeyUp;

		/// <summary>
		/// Currently pressed modifier keys
		/// </summary>
		public ModifierKeys Modifiers { get; private set; }

		static LowLevelKeyboard()
		{
			sMessages = new()
			{
				WM_KEYDOWN,
				WM_KEYUP,
				WM_SYSKEYDOWN,
				WM_SYSKEYUP
			};
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remapNumpad">
		/// Whether to remap numpad keys when numlock is off or shift is held so that they still
		/// report as numpad keys. Also remaps numpad Enter to F24 to distinguish it from Return.
		/// Only has an effect when NumLock is enabled on the keyboard.
		/// </param>
		public LowLevelKeyboard(bool remapNumpad)
		{
			mRemapNumpad = remapNumpad;
			mCallback = new(LowLevelKeyboardProc);
			mHook = nint.Zero;
			mDownKeys = new();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		~LowLevelKeyboard()
		{
			Dispose(false);
		}

		/// <summary>
		/// Hooks into the low level keyboard chain to start receiving events
		/// </summary>
		/// <exception cref="Win32Exception">An error occurred during hook registration</exception>
		public void Hook()
		{
			if (mHook != nint.Zero) return;

			mHook = SetWindowsHookEx(WH_KEYBOARD_LL, mCallback, nint.Zero, 0u);
			if (mHook == nint.Zero)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// Unhooks from the low level keyboard chain and stops receiving events.
		/// </summary>
		/// <exception cref="Win32Exception">An error occurred during hook unregistration</exception>
		public void Unhook()
		{
			if (mHook != nint.Zero)
			{
				bool success = UnhookWindowsHookEx(mHook);
				mHook = nint.Zero;
				if (!success)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
			mDownKeys.Clear();
			Modifiers = ModifierKeys.None;
		}

		private void Dispose(bool disposing)
		{
			Unhook();
			if (disposing)
			{
				KeyDown = null;
				KeyUp = null;
			}
		}

		private nint LowLevelKeyboardProc(int nCode, nuint wParam, [In] KBDLLHOOKSTRUCT lParam)
		{
			if (nCode == HC_ACTION)
			{
				uint message = wParam.ToUInt32();

				if (sMessages.Contains(message))
				{
					Key key = GetVKey(lParam, out bool addShift);
					if (addShift)
					{
						Modifiers |= ModifierKeys.Shift;
					}

					bool consume = false;
					switch (message)
					{
						case WM_KEYDOWN:
						case WM_SYSKEYDOWN:
							if (mDownKeys.Add(key))
							{
								switch (key)
								{
									case Key.LeftAlt:
									case Key.RightAlt:
										Modifiers |= ModifierKeys.Alt;
										break;
									case Key.LeftCtrl:
									case Key.RightCtrl:
										Modifiers |= ModifierKeys.Control;
										break;
									case Key.LeftShift:
									case Key.RightShift:
										Modifiers |= ModifierKeys.Shift;
										break;
									case Key.LWin:
									case Key.RWin:
										Modifiers |= ModifierKeys.Windows;
										break;
								}

								LowLevelKeyArgs args = new(key, Modifiers);

								KeyDown?.Invoke(this, args);
								consume |= args.ConsumeInput;
							}
							{
								LowLevelKeyArgs args = new(key, Modifiers);
								KeyRepeat?.Invoke(this, args);
								consume |= args.ConsumeInput;
							}
							break;
						case WM_KEYUP:
						case WM_SYSKEYUP:
							if (mDownKeys.Remove(key))
							{
								switch (key)
								{
									case Key.LeftAlt:
										if (!mDownKeys.Contains(Key.RightAlt))
										{
											Modifiers &= ~ModifierKeys.Alt;
										}
										break;
									case Key.RightAlt:
										if (!mDownKeys.Contains(Key.LeftAlt))
										{
											Modifiers &= ~ModifierKeys.Alt;
										}
										break;
									case Key.LeftCtrl:
										if (!mDownKeys.Contains(Key.RightCtrl))
										{
											Modifiers &= ~ModifierKeys.Control;
										}
										break;
									case Key.RightCtrl:
										if (!mDownKeys.Contains(Key.LeftCtrl))
										{
											Modifiers &= ~ModifierKeys.Control;
										}
										break;
									case Key.LeftShift:
										if (!mDownKeys.Contains(Key.RightShift))
										{
											Modifiers &= ~ModifierKeys.Shift;
										}
										break;
									case Key.RightShift:
										if (!mDownKeys.Contains(Key.LeftShift))
										{
											Modifiers &= ~ModifierKeys.Shift;
										}
										break;
									case Key.LWin:
										if (!mDownKeys.Contains(Key.RWin))
										{
											Modifiers &= ~ModifierKeys.Windows;
										}
										break;
									case Key.RWin:
										if (!mDownKeys.Contains(Key.LWin))
										{
											Modifiers &= ~ModifierKeys.Windows;
										}
										break;
								}

								LowLevelKeyArgs args = new(key, Modifiers);
								KeyUp?.Invoke(this, args);
								consume |= args.ConsumeInput;
							}
							break;
					}
					if (consume)
					{
						return 1;
					}
				}
			}
			return CallNextHookEx(mHook, nCode, wParam, lParam);
		}

		private Key GetVKey(KBDLLHOOKSTRUCT param, out bool addShift)
		{
			Key key = KeyInterop.KeyFromVirtualKey((int)param.vkCode);
			addShift = false;
			if (!mRemapNumpad || !Keyboard.IsKeyToggled(Key.NumLock)) return key;

			if ((param.flags & 1) == 0)
			{
				switch (key)
				{
					case Key.Insert:
						addShift = true;
						return Key.NumPad0;
					case Key.Delete:
						addShift = true;
						return Key.Decimal;
					case Key.End:
						addShift = true;
						return Key.NumPad1;
					case Key.Down:
						addShift = true;
						return Key.NumPad2;
					case Key.PageDown:
						addShift = true;
						return Key.NumPad3;
					case Key.Left:
						addShift = true;
						return Key.NumPad4;
					case Key.Clear:
						addShift = true;
						return Key.NumPad5;
					case Key.Right:
						addShift = true;
						return Key.NumPad6;
					case Key.Home:
						addShift = true;
						return Key.NumPad7;
					case Key.Up:
						addShift = true;
						return Key.NumPad8;
					case Key.PageUp:
						addShift = true;
						return Key.NumPad9;
				}
			}
			else
			{
				if (key == Key.Return)
				{
					return Key.F24;
				}
			}

			return key;
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
		private static extern nint SetWindowsHookEx(int idHook, HOOKPROC lpfn, nint hMod, uint dwThreadId);

		[DllImport("user32.dll", SetLastError = true)]
		[return:MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(nint hhk);

		[DllImport("user32.dll")]
		private static extern nint CallNextHookEx(nint hhk, int nCode, nuint wParam, [In] KBDLLHOOKSTRUCT lParam);

		[StructLayout(LayoutKind.Sequential)]
		private struct KBDLLHOOKSTRUCT
		{
			public uint vkCode;
			public uint scanCode;
			public uint flags;
			public uint time;
			public nuint dwExtraInfo;
		}

		private delegate nint HOOKPROC(int nCode, nuint wParam, [In] KBDLLHOOKSTRUCT lParam);

		private const int HC_ACTION = 0;
		private const int WH_KEYBOARD_LL = 13;

		private const uint WM_KEYDOWN = 0x0100;
		private const uint WM_KEYUP = 0x0101;
		private const uint WM_SYSKEYDOWN = 0x0104;
		private const uint WM_SYSKEYUP = 0x0105;
	}

	/// <summary>
	/// Args for a low level keyboard event
	/// </summary>
	internal class LowLevelKeyArgs : EventArgs
	{
		/// <summary>
		/// The affected key
		/// </summary>
		public Key Key { get; }

		/// <summary>
		/// Modifier keys pressed at the time this event occurred
		/// </summary>
		public ModifierKeys ModifierKeys { get; }

		/// <summary>
		/// Set to true to prevent the event from reaching the rest of the system.
		/// Use with caution.
		/// </summary>
		public bool ConsumeInput { get; set; }

		public LowLevelKeyArgs(Key key, ModifierKeys modifierKeys)
		{
			Key = key;
			ModifierKeys = modifierKeys;
			ConsumeInput = false;
		}
	}
}
