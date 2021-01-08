
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace CefNet.JSInterop
{
	public sealed class XrayObject : IDisposable
	{
		private static Dictionary<CefV8Context, List<XrayObject>> XrayRoots = new Dictionary<CefV8Context, List<XrayObject>>();

		private CefV8Value _value;
		private readonly CefV8Context _context;
		private GCHandle _handle;
		private int _refcount;

		public static XrayObject Wrap(CefV8Context context, CefV8Value value)
		{
			List<XrayObject> roots;
			lock (XrayRoots)
			{
				if (!XrayRoots.TryGetValue(context, out roots))
					throw new InvalidOperationException("Unexpected context.");
			}

			foreach (XrayObject obj in roots)
			{
				if (value == obj._value)
					return obj;
			}
			var xray = new XrayObject(context, value);
			roots.Add(xray);
			return xray;
		}

		private XrayObject(CefV8Context context, CefV8Value value)
		{
			_context = context;
			_value = value;
			this.IsFunction = _value.IsObjectOfType(CefV8ValueType.Function);
			_handle = GCHandle.Alloc(this, GCHandleType.Normal);
		}

		private void Dispose(bool disposing)
		{
			CefV8Value value = _value;
			if (value != null)
			{
				_value = null;
				value.Dispose();

				List<XrayObject> values;
				lock (XrayRoots)
				{
					XrayRoots.TryGetValue(_context, out values);
				}
				if (values == null)
					return;
				lock (values)
				{
					values.Remove(this);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool IsFunction { get; }

		public CefV8Value Value
		{
			get
			{
				if (_value == null || _context.IsDisposed)
					throw new ObjectDeadException();
				return _value;
			}
		}

		public CefV8Context Context
		{
			get
			{
				if (_context.IsDisposed)
					throw new ObjectDeadException();
				return _context;
			}
		}

		internal static void OnContextCreated(CefV8Context context)
		{
			lock (XrayRoots)
			{
				XrayRoots.Add(context, new List<XrayObject>(0));
			}
		}

		internal static void OnContextReleased(CefV8Context context)
		{
			List<XrayObject> values;
			lock (XrayRoots)
			{
				XrayRoots.Remove(context, out values);
			}
			if (values == null)
				return;
			foreach (XrayObject obj in values)
			{
				obj.Dispose();
			}
		}

		public XrayHandle CreateHandle()
		{
			Interlocked.Increment(ref _refcount);
			return new XrayHandle(Context.Frame.Identifier, GCHandle.ToIntPtr(_handle), IsFunction ? XrayDataType.Function : XrayDataType.Object);
		}

		internal void ReleaseHandle()
		{
			if (0 == Interlocked.Decrement(ref _refcount))
			{
				_handle.Free();
				Dispose();
			}
		}
	}
}
