﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: Generated/Native/Types/cef_write_handler_t.cs
// --------------------------------------------------------------------------------------------﻿
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
// --------------------------------------------------------------------------------------------

#pragma warning disable 0169, 1591, 1573

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;
using CefNet.CApi;
using CefNet.Internal;

namespace CefNet
{
	/// <summary>
	/// Structure the client can implement to provide a custom stream writer. The
	/// functions of this structure may be called on any thread.
	/// </summary>
	/// <remarks>
	/// Role: Handler
	/// </remarks>
	public unsafe partial class CefWriteHandler : CefBaseRefCounted<cef_write_handler_t>, ICefWriteHandlerPrivate
	{
#if NET_LESS_5_0
		private static readonly WriteDelegate fnWrite = WriteImpl;

		private static readonly SeekDelegate fnSeek = SeekImpl;

		private static readonly TellDelegate fnTell = TellImpl;

		private static readonly FlushDelegate fnFlush = FlushImpl;

		private static readonly MayBlockDelegate fnMayBlock = MayBlockImpl;

#endif // NET_LESS_5_0
		internal static unsafe CefWriteHandler Create(IntPtr instance)
		{
			return new CefWriteHandler((cef_write_handler_t*)instance);
		}

		public CefWriteHandler()
		{
			cef_write_handler_t* self = this.NativeInstance;
			#if NET_LESS_5_0
			self->write = (void*)Marshal.GetFunctionPointerForDelegate(fnWrite);
			self->seek = (void*)Marshal.GetFunctionPointerForDelegate(fnSeek);
			self->tell = (void*)Marshal.GetFunctionPointerForDelegate(fnTell);
			self->flush = (void*)Marshal.GetFunctionPointerForDelegate(fnFlush);
			self->may_block = (void*)Marshal.GetFunctionPointerForDelegate(fnMayBlock);
			#else
			self->write = (delegate* unmanaged[Stdcall]<cef_write_handler_t*, void*, UIntPtr, UIntPtr, UIntPtr>)&WriteImpl;
			self->seek = (delegate* unmanaged[Stdcall]<cef_write_handler_t*, long, int, int>)&SeekImpl;
			self->tell = (delegate* unmanaged[Stdcall]<cef_write_handler_t*, long>)&TellImpl;
			self->flush = (delegate* unmanaged[Stdcall]<cef_write_handler_t*, int>)&FlushImpl;
			self->may_block = (delegate* unmanaged[Stdcall]<cef_write_handler_t*, int>)&MayBlockImpl;
			#endif
		}

		public CefWriteHandler(cef_write_handler_t* instance)
			: base((cef_base_ref_counted_t*)instance)
		{
		}

		[MethodImpl(MethodImplOptions.ForwardRef)]
		extern bool ICefWriteHandlerPrivate.AvoidWrite();

		/// <summary>
		/// Write raw binary data.
		/// </summary>
		protected internal unsafe virtual long Write(IntPtr ptr, long size, long n)
		{
			return default;
		}

#if NET_LESS_5_0
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate UIntPtr WriteDelegate(cef_write_handler_t* self, void* ptr, UIntPtr size, UIntPtr n);

#endif // NET_LESS_5_0
		// size_t (*)(_cef_write_handler_t* self, const void* ptr, size_t size, size_t n)*
#if !NET_LESS_5_0
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#endif
		private static unsafe UIntPtr WriteImpl(cef_write_handler_t* self, void* ptr, UIntPtr size, UIntPtr n)
		{
			var instance = GetInstance((IntPtr)self) as CefWriteHandler;
			if (instance == null || ((ICefWriteHandlerPrivate)instance).AvoidWrite())
			{
				return default;
			}
			return new UIntPtr((ulong)instance.Write(unchecked((IntPtr)ptr), (long)size, (long)n));
		}

		[MethodImpl(MethodImplOptions.ForwardRef)]
		extern bool ICefWriteHandlerPrivate.AvoidSeek();

		/// <summary>
		/// Seek to the specified offset position. |whence| may be any one of SEEK_CUR,
		/// SEEK_END or SEEK_SET. Return zero on success and non-zero on failure.
		/// </summary>
		protected internal unsafe virtual int Seek(long offset, int whence)
		{
			return default;
		}

#if NET_LESS_5_0
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate int SeekDelegate(cef_write_handler_t* self, long offset, int whence);

#endif // NET_LESS_5_0
		// int (*)(_cef_write_handler_t* self, int64 offset, int whence)*
#if !NET_LESS_5_0
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#endif
		private static unsafe int SeekImpl(cef_write_handler_t* self, long offset, int whence)
		{
			var instance = GetInstance((IntPtr)self) as CefWriteHandler;
			if (instance == null || ((ICefWriteHandlerPrivate)instance).AvoidSeek())
			{
				return default;
			}
			return instance.Seek(offset, whence);
		}

		/// <summary>
		/// Return the current offset position.
		/// </summary>
		protected internal unsafe virtual long Tell()
		{
			return default;
		}

#if NET_LESS_5_0
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate long TellDelegate(cef_write_handler_t* self);

#endif // NET_LESS_5_0
		// int64 (*)(_cef_write_handler_t* self)*
#if !NET_LESS_5_0
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#endif
		private static unsafe long TellImpl(cef_write_handler_t* self)
		{
			var instance = GetInstance((IntPtr)self) as CefWriteHandler;
			if (instance == null)
			{
				return default;
			}
			return instance.Tell();
		}

		/// <summary>
		/// Flush the stream.
		/// </summary>
		protected internal unsafe virtual int Flush()
		{
			return default;
		}

#if NET_LESS_5_0
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate int FlushDelegate(cef_write_handler_t* self);

#endif // NET_LESS_5_0
		// int (*)(_cef_write_handler_t* self)*
#if !NET_LESS_5_0
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#endif
		private static unsafe int FlushImpl(cef_write_handler_t* self)
		{
			var instance = GetInstance((IntPtr)self) as CefWriteHandler;
			if (instance == null)
			{
				return default;
			}
			return instance.Flush();
		}

		/// <summary>
		/// Return true (1) if this handler performs work like accessing the file
		/// system which may block. Used as a hint for determining the thread to access
		/// the handler from.
		/// </summary>
		protected internal unsafe virtual bool MayBlock()
		{
			return default;
		}

#if NET_LESS_5_0
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate int MayBlockDelegate(cef_write_handler_t* self);

#endif // NET_LESS_5_0
		// int (*)(_cef_write_handler_t* self)*
#if !NET_LESS_5_0
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#endif
		private static unsafe int MayBlockImpl(cef_write_handler_t* self)
		{
			var instance = GetInstance((IntPtr)self) as CefWriteHandler;
			if (instance == null)
			{
				return default;
			}
			return instance.MayBlock() ? 1 : 0;
		}
	}
}
