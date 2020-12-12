﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/capi/cef_web_plugin_capi.h
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
	/// Structure to implement for visiting web plugin information. The functions of
	/// this structure will be called on the browser process UI thread.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe partial struct cef_web_plugin_info_visitor_t
	{
		/// <summary>
		/// Base structure.
		/// </summary>
		public cef_base_ref_counted_t @base;

		/// <summary>
		/// int (*)(_cef_web_plugin_info_visitor_t* self, _cef_web_plugin_info_t* info, int count, int total)*
		/// </summary>
		public void* visit;

		/// <summary>
		/// Method that will be called once for each plugin. |count| is the 0-based
		/// index for the current plugin. |total| is the total number of plugins.
		/// Return false (0) to stop visiting plugins. This function may never be
		/// called if no plugins are found.
		/// </summary>
		[NativeName("visit")]
		public unsafe int Visit(cef_web_plugin_info_t* info, int count, int total)
		{
			fixed (cef_web_plugin_info_visitor_t* self = &this)
			{
				return ((delegate* unmanaged[Stdcall]<cef_web_plugin_info_visitor_t*, cef_web_plugin_info_t*, int, int, int>)visit)(self, info, count, total);
			}
		}
	}
}

