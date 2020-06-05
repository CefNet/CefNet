﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/internal/cef_types.h
// --------------------------------------------------------------------------------------------﻿
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
// --------------------------------------------------------------------------------------------

#pragma warning disable 0169, 1591, 1573

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;

namespace CefNet
{
	/// <summary>
	/// Icon types for a MediaSink object. Should be kept in sync with Chromium&apos;s
	/// media_router::SinkIconType type.
	/// </summary>
	public enum CefMediaSinkIconType
	{
		Cast = 0,

		CastAudioGroup = 1,

		CastAudio = 2,

		Meeting = 3,

		Hangout = 4,

		Education = 5,

		WiredDisplay = 6,

		Generic = 7,

		/// <summary>
		/// The total number of values.
		/// </summary>
		TotalCount = 8,
	}
}
