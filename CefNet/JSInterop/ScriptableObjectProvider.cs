using CefNet.Unsafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

// TODO: It should be documented. But these API are subject to change: this layer must be configurable.
#pragma warning disable 0169, 1591, 1573

namespace CefNet.JSInterop
{
	//[DebuggerNonUserCode]
	public class ScriptableObjectProvider : IEquatable<ScriptableObjectProvider>
	{
		private readonly CefFrame _frame;
		private readonly long _frameId;

		public ScriptableObjectProvider(CefFrame frame)
		{
			_frame = frame;
			_frameId = frame.Identifier;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ScriptableObjectProvider);
		}

		public virtual bool Equals(ScriptableObjectProvider obj)
		{
			if (obj is null)
				return false;
			return obj._frameId == _frameId;
		}

		public static bool operator ==(ScriptableObjectProvider a, ScriptableObjectProvider b)
		{
			if (a is null)
				return b is null;
			return a.Equals(b);
		}

		public static bool operator !=(ScriptableObjectProvider a, ScriptableObjectProvider b)
		{
			if (a is null)
				return !(b is null);
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return _frameId.GetHashCode();
		}

		private CefBrowser Browser
		{
			get
			{
				CefBrowser browser = _frame.Browser;
				if (browser is null)
					throw new InvalidOperationException();
				return browser;
			}
		}

		private CefFrame Frame
		{
			get
			{
				if (_frame.Identifier != _frameId || !_frame.IsValid)
					throw new InvalidOperationException();
				return _frame;
			}
		}

		public object SendRequest(XrayAction action, XrayHandle thisArg, params object[] args)
		{
			var sqi = new ScriptableRequestInfo();
			CefProcessMessage msg = null;
			try
			{
				msg = new CefProcessMessage(CefNetApplication.XrayRequestKey);
				CefListValue argList = msg.ArgumentList;
				if (!argList.SetSize(3 + args.Length))
					throw new InvalidOperationException();
				argList.SetInt(0, sqi.RequestId);
				argList.SetInt(1, (int)action);
				argList.SetBinary(2, ValidateXrayHandle(thisArg).ToCfxBinaryValue());
				AppendArgs(argList, 3, args);

				CefFrame frame = this.Frame;
				if (!frame.IsValid || frame.Identifier != _frameId)
					throw new ObjectDeadException();
				frame.SendProcessMessage(CefProcessId.Renderer, msg);
				sqi.Wait();
				return sqi.GetResult();
			}
			finally
			{
				msg?.Dispose();
				sqi.Dispose();
			}
		}

		public XrayHandle GetGlobal()
		{
			return GetGlobal(this.Frame);
		}

		private static XrayHandle GetGlobal(CefFrame frame)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				return GetGlobalInternal(frame);
			}
			else
			{
				var sqi = new ScriptableRequestInfo();
				CefProcessMessage msg = null;
				try
				{
					msg = new CefProcessMessage(CefNetApplication.XrayRequestKey);
					using (CefListValue args = msg.ArgumentList)
					{
						if (!args.SetSize(3))
							throw new InvalidOperationException();
						args.SetInt(0, sqi.RequestId);
						args.SetInt(1, (int)XrayAction.GetGlobal);
					}
					if (!frame.IsValid)
						throw new ObjectDeadException();
					frame.SendProcessMessage(CefProcessId.Renderer, msg);
					sqi.Wait();
					return (XrayHandle)sqi.GetResult();
				}
				finally
				{
					msg?.Dispose();
					sqi.Dispose();
				}
			}
		}

		private static XrayHandle GetGlobalInternal(CefFrame frame)
		{
			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => GetGlobalInternal(frame)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return (XrayHandle)callTask.GetResult();
				}
			}

			CefV8Context context = frame.V8Context;
			if (!context.Enter())
				throw new InvalidOperationException();
			try
			{
				CefV8Value globalObj = context.GetGlobal();
				return (XrayHandle)CastCefV8ValueToDotnetType(context, globalObj, out bool isxray);
			}
			finally
			{
				context.Exit();
			}
		}

		public object GetProperty(XrayHandle self, int index)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				return GetPropertyInternal(self, index);
			}
			else
			{
				return SendRequest(XrayAction.Get, self, index);
			}
		}

		private object GetPropertyInternal(XrayHandle self, int index)
		{
			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => GetPropertyInternal(self, index)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return callTask.GetResult();
				}
			}

			XrayObject target = self.GetTarget(this.Frame);
			if (target is null || !target.Context.Enter())
				throw new InvalidOperationException();

			object retval;
			try
			{
				CefV8Value value = target.Value.GetValueByIndex(index);
				retval = CastCefV8ValueToDotnetType(target.Context, value, out bool isxray);
				if (!isxray) value.Dispose();
			}
			finally
			{
				target.Context.Exit();
			}
			return retval;
		}

		public object GetProperty(XrayHandle self, string name)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				return GetPropertyInternal(self, name);
			}
			else
			{
				return SendRequest(XrayAction.Get, self, name);
			}
		}

		private object GetPropertyInternal(XrayHandle self, string name)
		{
			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => GetPropertyInternal(self, name)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return callTask.GetResult();
				}
			}


			XrayObject target = self.GetTarget(this.Frame);
			if (!target.Context.Enter())
				throw new InvalidOperationException();

			object retval;
			try
			{
				CefV8Value value = target.Value.GetValueByKey(name);
				retval = CastCefV8ValueToDotnetType(target.Context, value, out bool isxray);
				if (!isxray) value.Dispose();
			}
			finally
			{
				target.Context.Exit();
			}
			return retval;
		}

		public void SetProperty(XrayHandle self, int index, object value)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				SetPropertyInternal(self, index, value);
			}
			else
			{
				SendRequest(XrayAction.Set, self, index, value);
			}
		}

		private bool SetPropertyInternal(XrayHandle self, int index, object value)
		{
			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => SetPropertyInternal(self, index, value)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return (bool)callTask.GetResult();
				}
			}

			XrayObject target = self.GetTarget(this.Frame);
			if (target is null || !target.Context.Enter())
				throw new InvalidOperationException();

			bool result;
			try
			{
				CefV8Value v8value = CastDotnetTypeToCefV8Value(target.Context, value, out bool isNotXray);
				result = target.Value.SetValueByIndex(index, v8value);
				if (isNotXray)
				{
					v8value.Dispose();
				}
			}
			finally
			{
				target.Context.Exit();
			}
			return result;
		}

		public void SetProperty(XrayHandle self, string name, object value)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				SetPropertyInternal(self, name, value);
			}
			else
			{
				SendRequest(XrayAction.Set, self, name, value);
			}
		}

		private bool SetPropertyInternal(XrayHandle self, string name, object value)
		{
			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => SetPropertyInternal(self, name, value)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return (bool)callTask.GetResult();
				}
			}

			XrayObject target = self.GetTarget(this.Frame);
			if (target is null || !target.Context.Enter())
				throw new InvalidOperationException();

			bool result;
			try
			{
				CefV8Value v8value = CastDotnetTypeToCefV8Value(target.Context, value, out bool isNotXray);
				result = target.Value.SetValueByKey(name, v8value, CefV8PropertyAttribute.None);
				if (isNotXray)
				{
					v8value.Dispose();
				}
			}
			finally
			{
				target.Context.Exit();
			}
			return result;
		}

		public object Invoke(XrayHandle self, params object[] args)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				return InvokeInternal(self, args);
			}
			else
			{
				return SendRequest(XrayAction.Invoke, self, args);
			}	
		}

		public object InvokeMember(XrayHandle self, string name, params object[] args)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				return InvokeMemberInternal(self, name, args);
			}
			else
			{
				var callArgs = new object[args.Length + 1];
				callArgs[0] = name;
				Array.Copy(args, 0, callArgs, 1, args.Length);
				return SendRequest(XrayAction.InvokeMember, self, callArgs);
			}
		}

		public void ReleaseObject(XrayHandle handle)
		{
			if (CefNetApplication.ProcessType == ProcessType.Renderer)
			{
				if (CefApi.CurrentlyOn(CefThreadId.Renderer))
					handle.Release();
				else
					CefApi.PostTask(CefThreadId.Renderer, new V8CallTask(handle.Release));
				return;
			}

			CefBinaryValue obj = null;

			CefProcessMessage msg = null;
			try
			{
				if (_frameId != handle.frame)
					return;
				obj = handle.ToCfxBinaryValue();

				msg = new CefProcessMessage(CefNetApplication.XrayReleaseKey);
				using (CefListValue args = msg.ArgumentList)
				{
					if (!args.SetSize(1))
						return;
					args.SetBinary(0, obj);
				}
				_frame.SendProcessMessage(CefProcessId.Renderer, msg);
			}
			finally
			{
				if (msg != null)
					msg.Dispose();
				if (obj != null)
					obj.Dispose();
			}
		}

		protected XrayHandle ValidateXrayHandle(XrayHandle handle)
		{
			if (_frameId != handle.frame)
				throw new InvalidOperationException();
			return handle;
		}

		protected void AppendArgs(CefListValue argList, int start, IEnumerable<object> args)
		{
			foreach (object value in args)
			{
				int index = start;
				start++;

				if (value is V8Undefined)
				{
					argList.SetBinary(index, new byte[1]);
					continue;
				}

				if (value is null)
				{
					argList.SetNull(index);
					continue;
				}

				switch (value)
				{
					case string v:
						argList.SetString(index, v);
						continue;
					case int v:
						argList.SetInt(index, v);
						continue;
					case double v:
						argList.SetDouble(index, v);
						continue;
					case bool v:
						argList.SetBool(index, v);
						continue;
					case DateTime v:
						argList.SetBinary(index, XrayHandle.FromDateTime(v).ToCfxBinaryValue());
						continue;
					case XrayHandle v:
						argList.SetBinary(index, ValidateXrayHandle(v).ToCfxBinaryValue());
						continue;
					case ScriptableObject v:
						argList.SetBinary(index, ValidateXrayHandle((XrayHandle)v).ToCfxBinaryValue());
						continue;
				}

				throw new NotImplementedException("Type: " + value.GetType().Name);
			}
		}


		private static CefV8Value GetSafeThisArg(CefV8Context context, XrayObject target)
		{
			return target?.Value ?? context.GetGlobal();
		}

		public static long Get(CefV8Context context, XrayObject target, CefListValue args, out CefV8Value value)
		{
			CefV8Value thisArg = GetSafeThisArg(context, target);
			CefValue arg3 = args.GetValue(3);
			CefValueType valueType = arg3.Type;

			if (valueType == CefValueType.Int)
			{
				value = thisArg.GetValueByIndex(arg3.GetInt());
				return 0;
			}

			string name = arg3.GetString();
			value = thisArg.GetValue(name);
			return 0;
		}

		public static void Set(CefV8Context context, XrayObject target, CefListValue args)
		{

			CefV8Value thisArg = GetSafeThisArg(context, target);
			CefV8Value value = CastCefValueToCefV8Value(context, args.GetValue(4), out bool isNotXray);

			thisArg.SetValueByKey(args.GetString(3), value, CefV8PropertyAttribute.None);
			if (isNotXray)
			{
				value.Dispose();
			}
		}

		internal static CefV8Value InvokeMember(CefV8Context context, XrayObject target, CefListValue args)
		{
			CefV8Value thisArg = GetSafeThisArg(context, target);
			CefV8Value func = thisArg.GetValue(args.GetString(3));
			if (!func.IsObjectOfType(CefV8ValueType.Function))
			{
				func.Dispose();
				throw new MissingMethodException();
			}

			int size = (int)(args.GetSize() - 4);
			var xraylist = new List<int>(size);
			var fnArgs = new CefV8Value[size];
			try
			{
				for (int i = 0; i < fnArgs.Length; i++)
				{
					int index = (i + 4);
					fnArgs[i] = CastCefValueToCefV8Value(context, args.GetValue(index), out bool isNew);
					if (!isNew) xraylist.Add(index);
				}
				return func.ExecuteFunction(thisArg, fnArgs);
			}
			finally
			{
				func.Dispose();
				for (int i = 0; i < fnArgs.Length; i++)
				{
					if (!xraylist.Contains(i))
						fnArgs[i].Dispose();
				}
			}
		}

		public object InvokeMemberInternal(XrayHandle self, string name, object[] args)
		{
			if (args is null)
				throw new ArgumentNullException(nameof(args));

			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => InvokeMemberInternal(self, name, args)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return (bool)callTask.GetResult();
				}
			}

			XrayObject target = self.GetTarget(this.Frame);
			if (target is null || !target.Context.Enter())
				throw new InvalidOperationException();

			object retval;
			try
			{

				CefV8Value thisArg = target.Value;
				CefV8Value func = thisArg.GetValueByKey(name);
				if (func.IsObjectOfType(CefV8ValueType.Function))
				{
					func.Dispose();
					throw new MissingMethodException(string.Format("'{0} is not a function.'", name));
				}

				CefV8Value value;
				var xraylist = new List<int>(args.Length);
				var fnArgs = new CefV8Value[args.Length];
				try
				{
					for (int i = 0; i < fnArgs.Length; i++)
					{
						fnArgs[i] = CastDotnetTypeToCefV8Value(target.Context, args[i], out bool isNew);
						if (!isNew) xraylist.Add(i);
					}
					value = func.ExecuteFunction(thisArg, fnArgs);
				}
				finally
				{
					for (int i = 0; i < fnArgs.Length; i++)
					{
						if (!xraylist.Contains(i))
							fnArgs[i].Dispose();
					}
				}
				retval = CastCefV8ValueToDotnetType(target.Context, value, out bool isxray);
				if (!isxray) value.Dispose();
			}
			finally
			{
				target.Context.Exit();
			}
			return retval;
		}

		internal static CefV8Value Invoke(CefV8Context context, XrayObject target, CefListValue args)
		{
			CefV8Value func = target.Value;
			CefV8Value thisArg = CastCefValueToCefV8Value(context, args.GetValue(3), out bool isNewThisArg);

			int size = (int)(args.GetSize() - 4);
			var xraylist = new List<int>(size);
			var fnArgs = new CefV8Value[size];
			try
			{
				for (int i = 0; i < fnArgs.Length; i++)
				{
					int index = (i + 4);
					fnArgs[i] = CastCefValueToCefV8Value(context, args.GetValue(index), out bool isNew);
					if (!isNew) xraylist.Add(index);
				}
				return func.ExecuteFunction(thisArg, fnArgs);
			}
			finally
			{
				for (int i = 0; i < fnArgs.Length; i++)
				{
					if (!xraylist.Contains(i))
						fnArgs[i].Dispose();
				}
			}
		}

		private object InvokeInternal(XrayHandle self, object[] args)
		{
			if (args is null || args.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(args));

			if (!CefApi.CurrentlyOn(CefThreadId.Renderer))
			{
				using (var callTask = new V8CallTask(() => InvokeInternal(self, args)))
				{
					if (!CefApi.PostTask(CefThreadId.Renderer, callTask))
						throw new InvalidOperationException();
					return (bool)callTask.GetResult();
				}
			}

			XrayObject target = self.GetTarget(this.Frame);
			if (target is null || !target.Context.Enter())
				throw new InvalidOperationException();

			object retval;
			try
			{
				CefV8Value func = target.Value;
				CefV8Value thisArg = CastDotnetTypeToCefV8Value(target.Context, args[0], out bool isNewThisArg);
				CefV8Value value;

				int size = args.Length - 1;
				var xraylist = new List<int>(size);
				var fnArgs = new CefV8Value[size];
				try
				{
					for (int i = 0; i < fnArgs.Length; i++)
					{
						int index = (i + 1);
						fnArgs[i] = CastDotnetTypeToCefV8Value(target.Context, args[index], out bool isNew);
						if (!isNew) xraylist.Add(index);
					}
					value = func.ExecuteFunction(thisArg, fnArgs);
				}
				finally
				{
					for (int i = 0; i < fnArgs.Length; i++)
					{
						if (!xraylist.Contains(i))
							fnArgs[i].Dispose();
					}
				}
				retval = CastCefV8ValueToDotnetType(target.Context, value, out bool isxray);
				if (!isxray) value.Dispose();
			}
			finally
			{
				target.Context.Exit();
			}
			return retval;
		}

		internal unsafe static CefValue CastCefV8ValueToCefValue(CefV8Context context, CefV8Value value, out bool isXray)
		{
			isXray = false;
			if (value == null)
				return null;

			if (!value.IsValid)
				throw new InvalidCastException();

			CefValue v;
			switch (value.Type)
			{
				case CefV8ValueType.Undefined:
					v = new CefValue();
					v.SetBinary(new byte[1]);
					return v;
				case CefV8ValueType.Null:
					v = new CefValue();
					v.SetNull();
					return v;
				case CefV8ValueType.Bool:
					v = new CefValue();
					v.SetBool(value.GetBoolValue());
					return v;
				case CefV8ValueType.Int: // TYPE_INT, TYPE_UINT
				case CefV8ValueType.UInt:
					v = new CefValue();
					v.SetInt(value.GetIntValue());
					return v;
				case CefV8ValueType.Double:
					v = new CefValue();
					v.SetDouble(value.GetDoubleValue());
					return v;
				case CefV8ValueType.Date:
					v = new CefValue();
					v.SetBinary(XrayHandle.FromDateTime(value.GetDateValue().ToDateTime()).ToCfxBinaryValue());
					return v;
				case CefV8ValueType.String:
					v = new CefValue();
					if (!value.CopyV8StringToCefValue(v))
						throw new CefRuntimeException("Can't copy the string.");
					return v;
				case CefV8ValueType.Object:
					isXray = true;
					if (value.IsObjectOfType(CefV8ValueType.Array)) //TYPE_OBJECT (array)
					{
						throw new NotImplementedException();
					}
					if (value.IsObjectOfType(CefV8ValueType.ArrayBuffer)) //TYPE_OBJECT (arraybuffer)
					{
						throw new NotImplementedException();
					}
					v = new CefValue();
					v.SetBinary(XrayObject.Wrap(context, value).CreateHandle().ToCfxBinaryValue());
					return v;
			}
			throw new NotImplementedException();
		}

		internal static CefV8Value CastCefValueToCefV8Value(CefV8Context context, CefValue value, out bool isNew)
		{
			isNew = true;

			if (value is null)
				return CefV8Value.CreateNull();

			if (!value.IsValid)
				throw new InvalidCastException();

			CefValueType valueType = value.Type;
			switch (valueType)
			{
				case CefValueType.String:
					return new CefV8Value(value.GetString());
				case CefValueType.Int:
					return new CefV8Value(value.GetInt());
				case CefValueType.Bool:
					return new CefV8Value(value.GetBool());
				case CefValueType.Null:
					return CefV8Value.CreateNull();
				case CefValueType.Double:
					return new CefV8Value(value.GetDouble());
				case CefValueType.Binary:
					CefBinaryValue v = value.GetBinary();
					if (v.Size == 1)
						return CefV8Value.CreateUndefined();

					XrayHandle handle = XrayHandle.FromCfxBinaryValue(v);
					if (handle == XrayHandle.Zero)
						return context.GetGlobal();

					isNew = (handle.dataType != XrayDataType.Object && handle.dataType != XrayDataType.Function);
					return handle.ToCefV8Value(context.Frame);
			}

			throw new NotSupportedException();
		}

		internal static CefV8Value CastDotnetTypeToCefV8Value(CefV8Context context, object value, out bool isNew)
		{
			isNew = true;

			if (value is null)
				return CefV8Value.CreateNull();

			if (value is V8Undefined)
				return CefV8Value.CreateUndefined();

			switch (value)
			{
				case string v:
					return new CefV8Value(v);
				case int v:
					return new CefV8Value(v);
				case double v:
					return new CefV8Value(v);
				case bool v:
					return new CefV8Value(v);
				case DateTime v:
					return new CefV8Value(v);
				case XrayHandle v:
					isNew = (v.dataType != XrayDataType.Object && v.dataType != XrayDataType.Function);
					return v.ToCefV8Value(context.Frame);
				case ScriptableObject v:
					XrayHandle hv = (XrayHandle)v;
					isNew = (hv.dataType != XrayDataType.Object && hv.dataType != XrayDataType.Function);
					CefV8Value cv8 = hv.ToCefV8Value(context.Frame);
					GC.KeepAlive(v);
					return cv8;
			}

			throw new NotImplementedException("Type: " + value.GetType().Name);
		}

		internal unsafe static object CastCefV8ValueToDotnetType(CefV8Context context, CefV8Value value, out bool isXray)
		{
			isXray = false;
			if (value == null)
				return null;

			if (!value.IsValid)
				throw new InvalidCastException();

			switch (value.Type)
			{
				case CefV8ValueType.Undefined:
					return V8Undefined.Value;
				case CefV8ValueType.Null:
					return null;
				case CefV8ValueType.Bool:
					return value.GetBoolValue();
				case CefV8ValueType.Int: // TYPE_INT, TYPE_UINT
				case CefV8ValueType.UInt:
					return value.GetIntValue();
				case CefV8ValueType.Double:
					return value.GetDoubleValue();
				case CefV8ValueType.Date:
					return value.GetDateValue().ToDateTime();
				case CefV8ValueType.String:
					return value.GetStringValue();
				case CefV8ValueType.Object:
					isXray = true;
					if (value.IsObjectOfType(CefV8ValueType.Array)) //TYPE_OBJECT (array)
					{
						throw new NotImplementedException();
					}
					if (value.IsObjectOfType(CefV8ValueType.ArrayBuffer)) //TYPE_OBJECT (arraybuffer)
					{
						throw new NotImplementedException();
					}
					return XrayObject.Wrap(context, value).CreateHandle();
			}
			throw new NotImplementedException();
		}


	}
}
