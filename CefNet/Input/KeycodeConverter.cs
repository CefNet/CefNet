using System;
using CefNet.Input.Linux;
using CefNet.Input.MacOS;
using CefNet.WinApi;

namespace CefNet.Input
{
	/// <summary>
	/// Provides methods to convert between platform-dependent key codes
	/// and platform-neutral values.
	/// </summary>
	public class KeycodeConverter
	{
		private static KeycodeConverter _Default = new KeycodeConverter();

		/// <summary>
		/// Gets and sets the default <see cref="KeycodeConverter"/>. 
		/// </summary>
		public static KeycodeConverter Default
		{
			get { return _Default; }
			set { _Default = value ?? new KeycodeConverter(); }
		}

		/// <summary>
		/// Translates the specified character to the corresponding virtual-key code
		/// for the current keyboard.
		/// </summary>
		/// <param name="c">The character to be translated into a virtual-key code.</param>
		/// <returns>The virtual key code.</returns>
		public virtual VirtualKeys CharacterToVirtualKey(char character)
		{
			if (PlatformInfo.IsWindows)
			{
				int virtualKeyCode = (WinApi.NativeMethods.VkKeyScan(character) & 0xFF);
				if (virtualKeyCode == 0xFF)
					return VirtualKeys.None;
				return (VirtualKeys)virtualKeyCode;
			}

			if (PlatformInfo.IsLinux)
			{
				XKeySym keysym = Linux.KeyInterop.CharToXKeySym(character);
				if (keysym == XKeySym.None)
					keysym = CefNet.Linux.NativeMethods.XStringToKeysym("U" + ((int)character).ToString("X"));
				return Linux.KeyInterop.XKeySymToVirtualKey(TranslateXKeySymToAsciiXKeySym(keysym));
			}

			if (PlatformInfo.IsMacOS)
			{
				// US QWERTY only
				XKeySym keysym = Linux.KeyInterop.CharToXKeySym(character);
				return Linux.KeyInterop.XKeySymToVirtualKey(keysym);
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts the specified virtual-key code into the current platform&apos;s
		/// native keycode (scancode).
		/// </summary>
		/// <param name="key">The virtual key.</param>
		/// <param name="modifiers">A bitwise combination of the <see cref="CefEventFlags"/> values.</param>
		/// <param name="extended">The extended key flag.</param>
		/// <returns>
		/// A native key code of virtual-key for the current keyboard. If there is no translation, the return value is zero.
		/// </returns>
		public virtual int VirtualKeyToNativeKeyCode(VirtualKeys key, CefEventFlags modifiers, bool extended)
		{
			if (PlatformInfo.IsWindows)
				return GetWindowsNativeKeyCode(key, extended);
			if (PlatformInfo.IsLinux)
				return GetLinuxNativeKeyCode(key, modifiers.HasFlag(CefEventFlags.ShiftDown));
			if (PlatformInfo.IsMacOS)
				return GetMacOSNativeKeyCode(key, extended);
			return 0;
		}


		/// <summary>
		/// Translates a virtual key to the corresponding native key code.
		/// </summary>
		/// <param name="key">Specifies the virtual key.</param>
		/// <param name="extended">The extended key flag.</param>
		/// <returns>A Windows OEM scan code.</returns>
		public static int GetWindowsNativeKeyCode(VirtualKeys key, bool extended)
		{
			uint scan_code = NativeMethods.MapVirtualKey((uint)key, MapVirtualKeyType.MAPVK_VK_TO_VSC);
			if (extended && scan_code != 0)
				scan_code |= 0xE000U;
			return (int)scan_code;
		}

		/// <summary>
		/// Translates a virtual key to the corresponding native key code.
		/// </summary>
		/// <param name="key">Specifies the virtual key.</param>
		/// <param name="extended">The extended key flag.</param>
		/// <returns>A XKB scan code.</returns>
		public static int GetMacOSNativeKeyCode(VirtualKeys key, bool extended)
		{
			MacOSVirtualKey vkcode = MacOS.KeyInterop.WindowsKeyToMacOSKey(key, extended);
			if (vkcode != MacOSVirtualKey.Invalid)
				return (int)vkcode;
			return 0;
		}

		/// <summary>
		/// Returns a native key code for the specified virtual key.
		/// </summary>
		/// <param name="key">Specifies the virtual key.</param>
		/// <param name="shift">Specifies a Shift key state.</param>
		/// <returns>A native key code for the current keyboard.</returns>
		public static int GetLinuxNativeKeyCode(VirtualKeys key, bool shift)
		{
			XKeySym keysym = Linux.KeyInterop.VirtualKeyToXKeySym(key, shift);
			if (keysym == XKeySym.None)
				return 0;
			return GetLinuxHardwareKeyCode(keysym);
		}

		/// <summary>
		/// Converts the value of a UTF-16 encoded character into a native key code.
		/// </summary>
		/// <param name="character">The character to be converted.</param>
		/// <param name="extended">The extended key flag.</param>
		/// <returns>A Windows OEM scan code.</returns>
		public static int GetWindowsNativeKeyCode(char character, bool extended)
		{
			ushort virtualKey = WinApi.NativeMethods.VkKeyScan(character);
			if (virtualKey == 0xFFFFU)
				return 0;
			return GetWindowsNativeKeyCode((VirtualKeys)virtualKey, extended);
		}

		/// <summary>
		/// Converts the value of a UTF-16 encoded character into a native key code.
		/// </summary>
		/// <param name="character">The character to be converted.</param>
		/// <param name="extended">The extended key flag.</param>
		/// <returns>A native key code for the current keyboard.</returns>
		public static int GetMacOSNativeKeyCode(char character, bool extended)
		{
			XKeySym keysym = Linux.KeyInterop.CharToXKeySym(character);
			if (keysym == XKeySym.None)
				return 0;
			return GetMacOSNativeKeyCode(Linux.KeyInterop.XKeySymToVirtualKey(keysym), extended);
		}

		/// <summary>
		/// Converts the value of a UTF-16 encoded character into a native key code.
		/// </summary>
		/// <param name="character">The character to be converted.</param>
		/// <returns>A native key code for the current keyboard.</returns>
		public static int GetLinuxNativeKeyCode(char character)
		{
			XKeySym keysym = Linux.KeyInterop.CharToXKeySym(character);
			if (keysym == XKeySym.None)
				return 0;
			return GetLinuxHardwareKeyCode(keysym);
		}



		/// <summary>
		/// Returns a hardware key code for the specified X keysym (only Linux OS).
		/// </summary>
		/// <param name="keysym">Specifies the KeySym.</param>
		/// <returns>A hardware key code.</returns>
		public static byte GetLinuxHardwareKeyCode(XKeySym keysym)
		{
			IntPtr display = CefNet.Linux.NativeMethods.XOpenDisplay(IntPtr.Zero);
			try
			{
				return CefNet.Linux.NativeMethods.XKeysymToKeycode(display, keysym);
			}
			finally
			{
				CefNet.Linux.NativeMethods.XCloseDisplay(display);
			}
		}

		/// <summary>
		/// Translates the specified <paramref name="lParam"/> to a Windows OEM scan code.
		/// </summary>
		/// <param name="lParam">The lParam field of a keyboard input notification message.</param>
		/// <returns>The scan code contained in <paramref name="lParam"/>.</returns>
		public static unsafe int GetWindowsScanCodeFromLParam(IntPtr lParam)
		{
			uint value = unchecked((uint)lParam.ToPointer());
			uint scan_code = (value >> 16) & 0x00FFU;
			if ((value & (1 << 24)) != 0) // KF_EXTENDED
				scan_code |= 0xE000U;
			return (int)scan_code;
		}

		/// <summary>
		/// Translates the specified Windows OEM scan code to a lParam value.
		/// </summary>
		/// <param name="eventType">The key event type of a message.</param>
		/// <param name="repeatCount">The repeat count for a message.</param>
		/// <param name="scanCode">The scan code for a key.</param>
		/// <param name="isSystemKey">The system key flag.</param>
		/// <param name="isExtended">The extended key flag.</param>
		/// <returns>A lParam to the specified event type message.</returns>
		public static unsafe IntPtr GetLParamFromWindowsScanCode(CefKeyEventType eventType, int repeatCount, byte scanCode, bool isSystemKey, bool isExtended)
		{
			if (repeatCount < 0)
				throw new ArgumentOutOfRangeException(nameof(repeatCount));

			//const uint KF_MENUMODE = 0x1000; // 28 bit
			//const uint KF_DLGMODE = 0x0800; // 27 bit
			//0x400 not used 26 bit
			//0x200 not used 25 bit

			const uint KF_EXTENDED = 0x100; // 24 bit
			const uint KF_ALTDOWN = 0x2000; // 29 bit
			const uint KF_UP = 0x8000; // 31 bit
			const uint KF_REPEAT = 0x4000; // 30 bit

			uint keyInfo = scanCode;

			if (eventType == CefKeyEventType.KeyUp)
			{
				repeatCount = 1;
				keyInfo |= KF_UP;
			}

			if (repeatCount > 0)
				keyInfo |= KF_REPEAT;
			else
				repeatCount = 1;

			if (isSystemKey)
				keyInfo |= KF_ALTDOWN;
			if (isExtended)
				keyInfo |= KF_EXTENDED;

			return new IntPtr((void*)((keyInfo << 16) | ((uint)repeatCount & 0xFFFFU)));
		}

		/// <summary>
		/// Determines that a character requires the Shift modifier key.
		/// </summary>
		/// <param name="c">The Unicode character to evaluate.</param>
		/// <returns>
		/// Returns true if a character requires the Shift modifier key;
		/// otherwise, false.
		/// </returns>
		public static bool IsShiftRequired(char c)
		{
			if (c >= '!' && c <= '+')
				return c != '\'';
			if (c == ':')
				return true;
			if (c >= '<' && c <= '@')
				return c != '=';
			if (c == '^' && c == '_')
				return true;
			if (c >= '{' && c <= '~')
				return true;
			return char.IsUpper(c);
		}

		/// <summary>
		/// Translates a KeySym to the corresponding KeySym from latin range for the current keyboard.
		/// </summary>
		/// <param name="keysym">The KeySym to be translated.</param>
		/// <returns>The KeySym.</returns>
		public static XKeySym TranslateXKeySymToAsciiXKeySym(XKeySym keysym)
		{
			IntPtr display = CefNet.Linux.NativeMethods.XOpenDisplay(IntPtr.Zero);
			try
			{
				byte keycode = CefNet.Linux.NativeMethods.XKeysymToKeycode(display, keysym);
				return CefNet.Linux.NativeMethods.XKeycodeToKeysym(display, keycode, 0);
			}
			finally
			{
				CefNet.Linux.NativeMethods.XCloseDisplay(display);
			}
		}

		/// <summary>
		/// Converts a combination of a virtual key code and a modifier key state into a string of one or more Unicode characters.
		/// </summary>
		/// <param name="virtualKey">The virtual key code that is to be translated.</param>
		/// <param name="modifiers">A bitwise combination of the <see cref="CefEventFlags"/> values.</param>
		/// <returns>The character resulting from the virtual key code being handled.</returns>
		public static char TranslateVirtualKey(VirtualKeys virtualKey, CefEventFlags modifiers)
		{
			XKeySym keysym = Linux.KeyInterop.VirtualKeyToXKeySym(virtualKey, modifiers.HasFlag(CefEventFlags.ShiftDown));
			if (keysym == XKeySym.None)
				return char.MinValue;
			return Linux.KeyInterop.XKeySymToChar(keysym);
		}

	}
}
