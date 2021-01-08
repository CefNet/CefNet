using System;
using CefNet.Unsafe;

namespace CefNet.CApi
{
#pragma warning disable CS1591
	public unsafe partial struct cef_v8value_t
#pragma warning restore CS1591
	{
		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A hash code for the <see cref="cef_v8value_t"/> object.</returns>
		public override int GetHashCode()
		{
			if (IsValid() == 0)
				return 0;

			switch (GetCefType())
			{
				case CefV8ValueType.Object:
					return GetObjectIdentityHash();
				case CefV8ValueType.Bool:
					return GetBoolValue() | (int)CefV8ValueType.Bool;
				case CefV8ValueType.Double:
					return GetDoubleValue().GetHashCode();
				case CefV8ValueType.Int:
				case CefV8ValueType.UInt:
					return GetIntValue();
				case CefV8ValueType.Null:
					return (int)CefV8ValueType.Null;
				case CefV8ValueType.Undefined:
					return (int)CefV8ValueType.Undefined;
				case CefV8ValueType.String:
					return (CefString.ReadAndFree(GetStringValue()) ?? string.Empty).GetHashCode();
				case CefV8ValueType.Date:
					return GetDateValue().GetHashCode();
				case CefV8ValueType.BigInt:
					return GetObjectIdentityHash();
				case CefV8ValueType.Symbol:
					return GetObjectIdentityHash();
			}
			
			return 0;
		}
	}
}
