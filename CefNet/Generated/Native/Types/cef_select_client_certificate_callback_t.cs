﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/capi/cef_request_handler_capi.h
// --------------------------------------------------------------------------------------------﻿
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
// --------------------------------------------------------------------------------------------

#pragma warning disable 0169, 1591, 1573

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;

namespace CefNet.CApi
{
	/// <summary>
	/// Callback structure used to select a client certificate for authentication.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe partial struct cef_select_client_certificate_callback_t
	{
		/// <summary>
		/// Base structure.
		/// </summary>
		public cef_base_ref_counted_t @base;

		/// <summary>
		/// void (*)(_cef_select_client_certificate_callback_t* self, _cef_x509certificate_t* cert)*
		/// </summary>
		public void* select;

		/// <summary>
		/// Chooses the specified certificate for client certificate authentication.
		/// NULL value means that no client certificate should be used.
		/// </summary>
		[NativeName("select")]
		public unsafe void Select(cef_x509certificate_t* cert)
		{
			fixed (cef_select_client_certificate_callback_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_select_client_certificate_callback_t*, cef_x509certificate_t*, void>)select)(self, cert);
			}
		}
	}
}

