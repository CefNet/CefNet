﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/capi/cef_ssl_info_capi.h
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
	/// Structure representing SSL information.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe partial struct cef_sslinfo_t
	{
		/// <summary>
		/// Base structure.
		/// </summary>
		public cef_base_ref_counted_t @base;

		/// <summary>
		/// cef_cert_status_t (*)(_cef_sslinfo_t* self)*
		/// </summary>
		public void* get_cert_status;

		/// <summary>
		/// Returns a bitmask containing any and all problems verifying the server
		/// certificate.
		/// </summary>
		[NativeName("get_cert_status")]
		public unsafe CefCertStatus GetCertStatus()
		{
			fixed (cef_sslinfo_t* self = &this)
			{
				return ((delegate* unmanaged[Stdcall]<cef_sslinfo_t*, CefCertStatus>)get_cert_status)(self);
			}
		}

		/// <summary>
		/// _cef_x509certificate_t* (*)(_cef_sslinfo_t* self)*
		/// </summary>
		public void* get_x509certificate;

		/// <summary>
		/// Returns the X.509 certificate.
		/// </summary>
		[NativeName("get_x509certificate")]
		public unsafe cef_x509certificate_t* GetX509certificate()
		{
			fixed (cef_sslinfo_t* self = &this)
			{
				return ((delegate* unmanaged[Stdcall]<cef_sslinfo_t*, cef_x509certificate_t*>)get_x509certificate)(self);
			}
		}
	}
}

