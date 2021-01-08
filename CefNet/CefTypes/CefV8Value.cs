using CefNet.CApi;
using CefNet.Unsafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CefNet
{
	public unsafe partial class CefV8Value
	{

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type bool.
		/// </summary>
		/// <param name="value">The value of type <see cref="bool"/>.</param>
		public CefV8Value(bool value)
			: this(CefNativeApi.cef_v8value_create_bool(value ? 1 : 0))
		{

		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type int.
		/// </summary>
		/// <param name="value">The value of type <see cref="int"/>.</param>
		public CefV8Value(int value)
			: this(CefNativeApi.cef_v8value_create_int(value))
		{

		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type uint.
		/// </summary>
		/// <param name="value">The value of type <see cref="uint"/>.</param>
		public CefV8Value(uint value)
			: this(CefNativeApi.cef_v8value_create_uint(value))
		{

		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type double.
		/// </summary>
		/// <param name="value">The value of type <see cref="double"/>.</param>
		public CefV8Value(double value)
			: this(CefNativeApi.cef_v8value_create_double(value))
		{

		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type Date.<para/>
		/// This constructor should only be called from within the scope of a
		/// <see cref="CefRenderProcessHandler"/>, <see cref="CefV8Handler"/> or
		/// <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Exit"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <param name="value">The value of type <see cref="DateTime"/>.</param>
		public CefV8Value(DateTime value)
			: this(CreateDateInternal(value))
		{

		}

		private static cef_v8value_t* CreateDateInternal(DateTime date)
		{
			CefTime t = CefTime.FromDateTime(date);
			return CefNativeApi.cef_v8value_create_date((cef_time_t*)&t);
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type string.
		/// </summary>
		/// <param name="value">The value of type <see cref="string"/>.</param>
		public CefV8Value(string value)
			: this(CreateStringInternal(value))
		{

		}

		private static cef_v8value_t* CreateStringInternal(string value)
		{
			fixed (char* s = value)
			{
				var cstr = new cef_string_t { Str = s, Length = (value != null ? value.Length : 0) };
				return CefNativeApi.cef_v8value_create_string(&cstr);
			}
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type object with optional accessor
		/// and/or interceptor.<para/>
		/// This constructor should only be called from within the scope of a
		/// <see cref="CefRenderProcessHandler"/>, <see cref="CefV8Handler"/> or
		/// <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Exit"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <param name="accessor">A <see cref="CefV8Accessor"/> instance or null.</param>
		/// <param name="interceptor">A <see cref="CefV8Interceptor"/> instance or null.</param>
		public CefV8Value(CefV8Accessor accessor, CefV8Interceptor interceptor)
			: this(CefNativeApi.cef_v8value_create_object(accessor != null ? accessor.GetNativeInstance() : null, interceptor != null ? interceptor.GetNativeInstance() : null))
		{

		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type array with the specified
		/// <paramref name="length"/>.<para/>
		/// This function should only be called from within the scope of a
		/// <see cref="CefRenderProcessHandler"/>, <see cref="CefV8Handler"/> or
		/// <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Exit"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <param name="length">
		/// If <paramref name="length"/> is negative the returned array will have length 0.
		/// </param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type array.</returns>
		public static CefV8Value CreateArray(int length)
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_array(length));
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type ArrayBuffer which wraps the
		/// provided <paramref name="buffer"/> of size <paramref name="length"/> bytes.<para/>
		/// This function should only be called from within the scope of a
		/// <see cref="CefRenderProcessHandler"/>, <see cref="CefV8Handler"/> or
		/// <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Exit"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <param name="buffer">
		/// The ArrayBuffer is externalized, meaning that it does not own <paramref name="buffer"/>.
		/// </param>
		/// <param name="callback">
		/// The caller is responsible for freeing <paramref name="buffer"/> when requested via
		/// a call to <see cref="CefV8ArrayBufferReleaseCallback.ReleaseBuffer"/>.
		/// </param>
		/// <param name="length">The size of buffer in bytes.</param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type ArrayBuffer.</returns>
		public static CefV8Value CreateArrayBuffer(void* buffer, int length, CefV8ArrayBufferReleaseCallback callback)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (length < 0)
				throw new ArgumentOutOfRangeException(nameof(length));
			return new CefV8Value(CefNativeApi.cef_v8value_create_array_buffer(buffer, unchecked((UIntPtr)length), callback.GetNativeInstance()));
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type ArrayBuffer which wraps the
		/// provided <paramref name="buffer"/>.<para/>
		/// This function should only be called from within the scope of a
		/// <see cref="CefRenderProcessHandler"/>, <see cref="CefV8Handler"/> or
		/// <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Exit"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <param name="buffer">The buffer to wrap</param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type ArrayBuffer.</returns>
		public static CefV8Value CreateArrayBuffer(byte[] buffer)
		{
			var instance = new ArrayBuffer(buffer);
			return new CefV8Value(CefNativeApi.cef_v8value_create_array_buffer(instance.GetBuffer(), instance.Length, instance.GetNativeInstance()));
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type function.<para/>
		/// This function should only be called from within the scope of a
		/// <see cref="CefRenderProcessHandler"/>, <see cref="CefV8Handler"/> or
		/// <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Exit"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type function.</returns>
		public static CefV8Value CreateFunction(string name, CefV8Handler handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			fixed (char* s = name)
			{
				var aName = new cef_string_t { Str = s, Length = (name != null ? name.Length : 0) };
				return new CefV8Value(CefNativeApi.cef_v8value_create_function(&aName, handler.GetNativeInstance()));
			}
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type undefined.
		/// </summary>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type undefined.</returns>
		public static CefV8Value CreateUndefined()
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_undefined());
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type null.
		/// </summary>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type null.</returns>
		public static CefV8Value CreateNull()
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_null());
		}

		/// <summary>
		/// Create a new empty object.
		/// </summary>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type object.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CefV8Value CreateObject()
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_object(null, null));
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type int.
		/// </summary>
		/// <param name="value">The value of type <see cref="int"/>.</param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type int.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CefV8Value CreateInt32(int value)
		{
			return new CefV8Value(value);
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type double.
		/// </summary>
		/// <param name="value">The value of type <see cref="double"/>.</param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type double.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CefV8Value CreateDouble(double value)
		{
			return new CefV8Value(value);
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type Date.
		/// </summary>
		/// <param name="value">The value of type <see cref="DateTime"/>.</param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type Date.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CefV8Value CreateDate(DateTime value)
		{
			return new CefV8Value(value);
		}

		/// <summary>
		/// Create a new <see cref="CefV8Value"/> object of type string.
		/// </summary>
		/// <param name="value">The value of type <see cref="string"/>.</param>
		/// <returns>Returns a new <see cref="CefV8Value"/> object of type string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CefV8Value CreateString(string value)
		{
			return new CefV8Value(value);
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, CefV8Value value, CefV8PropertyAttribute attributes)
		{
			return SetValueByKey(key, value, attributes);
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, int value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, double value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, bool value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, uint value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, DateTime value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attribute">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, string value, CefV8PropertyAttribute attribute)
		{
			using (CefV8Value str = new CefV8Value(value))
			{
				return SetValueByKey(key, str, attribute);
			}
		}

		/// <summary>
		/// Associates a value with the specified identifier. 
		/// </summary>
		/// <param name="index">The property index.</param>
		/// <param name="value">The new value.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown. For read-only values this function will return true even though
		/// assignment failed.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(int index, CefV8Value value)
		{
			return SetValueByIndex(index, value);
		}

		/// <summary>
		/// Associates a value with the specified identifier. 
		/// </summary>
		/// <param name="key">The property name.</param>
		/// <param name="settings">A V8 access control value.</param>
		/// <param name="attributes">A V8 property attribute value.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown. For read-only values this function will return true even though
		/// assignment failed.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, CefV8AccessControl settings, CefV8PropertyAttribute attributes)
		{
			return SetValueByAccessor(key, settings, attributes);
		}

		/// <summary>
		/// Returns the value with the specified identifier.
		/// </summary>
		/// <param name="key">The property key.</param>
		/// <returns>
		/// Returns the value with the specified identifier. Returns null if
		/// this function is called incorrectly or an exception is thrown.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CefV8Value GetValue(string key)
		{
			return GetValueByKey(key);
		}

		/// <summary>
		/// Returns the value with the specified identifier.
		/// </summary>
		/// <param name="index">The property index.</param>
		/// <returns>
		/// Returns the value with the specified identifier. Returns null if
		/// this function is called incorrectly or an exception is thrown.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CefV8Value GetValue(int index)
		{
			return GetValueByIndex(index);
		}

		/// <summary>
		/// Copies the V8 string into <see cref="CefValue"/> object.
		/// </summary>
		/// <param name="value">The destination <see cref="CefValue"/> object.</param>
		/// <returns>Returns true if the value was set successfully.</returns>
		public bool CopyV8StringToCefValue(CefValue value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (Type != CefV8ValueType.String)
				throw new InvalidOperationException();

			cef_string_userfree_t userfreeStr = NativeInstance->GetStringValue();
			try
			{
				return value.NativeInstance->SetString((cef_string_t*)userfreeStr.Base.Base) != 0;
			}
			finally
			{
				CefString.Free(userfreeStr);
				GC.KeepAlive(this);
			}
		}

		/// <summary>
		/// Execute the function without arguments using the current V8 context.<para/>
		/// This function should only be called from within the scope of a <see cref="CefV8Handler"/>
		/// or <see cref="CefV8Accessor"/> callback, or in combination with calling
		/// <see cref="CefV8Context.Enter"/> and <see cref="CefV8Context.Enter"/>
		/// on a stored <see cref="CefV8Context"/> reference.
		/// </summary>
		/// <param name="thisArg">
		/// The receiver (&apos;this&apos; object) of the function. If <paramref name="thisArg"/>
		/// is null the current context&apos;s global objectwill be used.
		/// </param>
		/// <returns>
		/// Returns the function return value on success, or null if this function
		/// is called incorrectly or an exception is thrown.
		/// </returns>
		public unsafe virtual CefV8Value ExecuteFunction(CefV8Value thisArg)
		{
			return SafeCall(CefV8Value.Wrap(CefV8Value.Create, NativeInstance->ExecuteFunction(thisArg != null ? thisArg.GetNativeInstance() : null, new UIntPtr(), null)));
		}

		/// <summary>
		/// Gets the value associated with the specified keys chain.
		/// </summary>
		/// <param name="names">Names of properties.</param>
		/// <returns>
		/// A value associated with the specified keys chain.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="names"/> is null.</exception>
		/// <exception cref="InvalidOperationException">
		/// Any parent property is null or undefined, or if this function is called incorrectly
		/// or an JavaScript-exception is thrown.
		/// </exception>
		public CefV8Value GetValue(params string[] names)
		{
			if (names == null)
				throw new ArgumentNullException(nameof(names));

			return GetValue(new ArraySegment<string>(names));
		}

		/// <summary>
		/// Gets the value associated with the specified keys chain.
		/// </summary>
		/// <param name="names">Names of properties.</param>
		/// <returns>
		/// A value associated with the specified keys chain.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="names"/> is null.</exception>
		/// <exception cref="InvalidOperationException">
		/// Any parent property is null or undefined, or if this function is called incorrectly
		/// or an JavaScript-exception is thrown.
		/// </exception>
		public CefV8Value GetValue(ArraySegment<string> names)
		{
			if (names.Count <= 0)
				throw new ArgumentOutOfRangeException(nameof(names));

			cef_v8value_t* value = null;
			cef_v8value_t* self = GetNativeInstance();
			foreach (string name in names)
			{
				try
				{
					CefV8ValueType type = self->GetCefType();
					if (type == CefV8ValueType.Null)
						throw new InvalidOperationException($"Cannot read property '{name}' of null.");
					if (type == CefV8ValueType.Undefined)
						throw new InvalidOperationException($"Cannot read property '{name}' of undefined.");
					if (string.IsNullOrEmpty(name))
						throw new ArgumentOutOfRangeException(nameof(names));

					fixed (char* s = name)
					{
						var aName = new cef_string_t { Str = s, Length = name.Length };
						value = self->GetValueByKey(&aName);
					}
				}
				finally
				{
					self->@base.Release();
				}

				if (value == null)
					throw new InvalidOperationException();
				self = value;
			}
			GC.KeepAlive(this);
			return CefV8Value.Wrap(CefV8Value.Create, value);
		}

		/// <summary>
		/// Sets the value of property associated with the specified keys chain.
		/// </summary>
		/// <param name="names">Names of properties.</param>
		/// <param name="value">The new value.</param>
		/// <param name="attribute">A bitwise combination of <see cref="CefV8PropertyAttribute"/>.</param>
		public void SetValue(string[] names, CefV8Value value, CefV8PropertyAttribute attribute)
		{
			if (names == null)
				throw new ArgumentNullException(nameof(names));

			int last = names.Length - 1;
			if (last < 0)
				throw new ArgumentOutOfRangeException(nameof(names));

			CefV8Value self = this;
			if (last > 0)
			{
				self = GetValue(new ArraySegment<string>(names, 0, last));
			}
			if (!self.SetValueByKey(names[last], value, attribute))
				throw new InvalidOperationException();
		}

		private static readonly Dictionary<HashKey, WeakReference<CefV8Value>> WeakRefs = new Dictionary<HashKey, WeakReference<CefV8Value>>(new HashKeyComparer());

#if DEBUG
		public static int GetWeakRefsCacheSize()
		{
			return WeakRefs.Count;
		}
#endif

		private int _hashcode;
		private WeakReference<CefV8Value> _weakRef;

		/// <summary>
		/// Gets weak reference to this object.
		/// </summary>
		public WeakReference<CefV8Value> WeakRef
		{
			get
			{
				if (_weakRef == null)
				{
					lock (WeakRefs)
					{
						if (_weakRef == null)
							_weakRef = new WeakReference<CefV8Value>(this);
					}
				}
				return _weakRef;
			}
		}

#pragma warning disable CS1591
		protected override void Dispose(bool disposing)
#pragma warning restore CS1591
		{
			if (!IsDisposed)
			{
				lock (WeakRefs)
				{
					WeakRefs.Remove(new HashKey(_hashcode, WeakRef));
				}
			}
			base.Dispose(disposing);
		}


		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return IsDisposed ? 0 : _hashcode;
		}

		/// <summary>
		/// Returns a wrapper for the specified pointer to <see cref="cef_v8value_t"/> instance.
		/// </summary>
		/// <param name="create">Represents a method that create a new wrapper.</param>
		/// <param name="instance">The pointer to <see cref="cef_v8value_t"/> object.</param>
		/// <returns>Returns an existing or new wrapper of type <see cref="CefV8Value"/>.</returns>
		public unsafe static CefV8Value Wrap(Func<IntPtr, CefV8Value> create, cef_v8value_t* instance)
		{
			if (instance == null)
				return null;

			IntPtr ptr = new IntPtr(instance);
			lock (WeakRefs)
			{
				CefV8Value wrapper;
				int hashcode = instance->GetHashCode();

				if (WeakRefs.TryGetValue(new HashKey(hashcode, instance), out WeakReference<CefV8Value> weakRef)
					&& weakRef.TryGetTarget(out wrapper)
					&& instance->IsSame(wrapper.GetNativeInstance()) != 0)
				{
					instance->@base.Release();
					Debug.Print("V8Value type: {0}", wrapper.Type);
					Debug.Print("WeakRefs count: {0}", WeakRefs.Count);
					return wrapper;
				}
				wrapper = create(ptr);
				wrapper._hashcode = hashcode;
				WeakRefs.Add(new HashKey(hashcode, wrapper.WeakRef), wrapper.WeakRef);
				return wrapper;
			}
		}


	}



}
