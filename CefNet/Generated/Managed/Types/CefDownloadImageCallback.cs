﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: Generated/Native/Types/cef_download_image_callback_t.cs
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
	/// Callback structure for cef_browser_host_t::DownloadImage. The functions of
	/// this structure will be called on the browser process UI thread.
	/// </summary>
	/// <remarks>
	/// Role: Handler
	/// </remarks>
	public unsafe partial class CefDownloadImageCallback : CefBaseRefCounted<cef_download_image_callback_t>, ICefDownloadImageCallbackPrivate
	{
#if NET_LESS_5_0
		private static readonly OnDownloadImageFinishedDelegate fnOnDownloadImageFinished = OnDownloadImageFinishedImpl;

#endif // NET_LESS_5_0
		internal static unsafe CefDownloadImageCallback Create(IntPtr instance)
		{
			return new CefDownloadImageCallback((cef_download_image_callback_t*)instance);
		}

		public CefDownloadImageCallback()
		{
			cef_download_image_callback_t* self = this.NativeInstance;
			#if NET_LESS_5_0
			self->on_download_image_finished = (void*)Marshal.GetFunctionPointerForDelegate(fnOnDownloadImageFinished);
			#else
			self->on_download_image_finished = (delegate* unmanaged[Stdcall]<cef_download_image_callback_t*, cef_string_t*, int, cef_image_t*, void>)&OnDownloadImageFinishedImpl;
			#endif
		}

		public CefDownloadImageCallback(cef_download_image_callback_t* instance)
			: base((cef_base_ref_counted_t*)instance)
		{
		}

		[MethodImpl(MethodImplOptions.ForwardRef)]
		extern bool ICefDownloadImageCallbackPrivate.AvoidOnDownloadImageFinished();

		/// <summary>
		/// Method that will be executed when the image download has completed.
		/// |image_url| is the URL that was downloaded and |http_status_code| is the
		/// resulting HTTP status code. |image| is the resulting image, possibly at
		/// multiple scale factors, or NULL if the download failed.
		/// </summary>
		protected internal unsafe virtual void OnDownloadImageFinished(string imageUrl, int httpStatusCode, CefImage image)
		{
		}

#if NET_LESS_5_0
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate void OnDownloadImageFinishedDelegate(cef_download_image_callback_t* self, cef_string_t* image_url, int http_status_code, cef_image_t* image);

#endif // NET_LESS_5_0
		// void (*)(_cef_download_image_callback_t* self, const cef_string_t* image_url, int http_status_code, _cef_image_t* image)*
#if !NET_LESS_5_0
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#endif
		private static unsafe void OnDownloadImageFinishedImpl(cef_download_image_callback_t* self, cef_string_t* image_url, int http_status_code, cef_image_t* image)
		{
			var instance = GetInstance((IntPtr)self) as CefDownloadImageCallback;
			if (instance == null || ((ICefDownloadImageCallbackPrivate)instance).AvoidOnDownloadImageFinished())
			{
				ReleaseIfNonNull((cef_base_ref_counted_t*)image);
				return;
			}
			instance.OnDownloadImageFinished(CefString.Read(image_url), http_status_code, CefImage.Wrap(CefImage.Create, image));
		}
	}
}
