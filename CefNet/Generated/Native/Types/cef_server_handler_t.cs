﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/capi/cef_server_capi.h
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
	/// Implement this structure to handle HTTP server requests. A new thread will be
	/// created for each cef_server_t::CreateServer call (the &quot;dedicated server
	/// thread&quot;), and the functions of this structure will be called on that thread.
	/// It is therefore recommended to use a different cef_server_handler_t instance
	/// for each cef_server_t::CreateServer call to avoid thread safety issues in the
	/// cef_server_handler_t implementation.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe partial struct cef_server_handler_t
	{
		/// <summary>
		/// Base structure.
		/// </summary>
		public cef_base_ref_counted_t @base;

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server)*
		/// </summary>
		public void* on_server_created;

		/// <summary>
		/// Called when |server| is created. If the server was started successfully
		/// then cef_server_t::IsRunning will return true (1). The server will continue
		/// running until cef_server_t::Shutdown is called, after which time
		/// OnServerDestroyed will be called. If the server failed to start then
		/// OnServerDestroyed will be called immediately after this function returns.
		/// </summary>
		[NativeName("on_server_created")]
		public unsafe void OnServerCreated(cef_server_t* server)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, void>)on_server_created)(self, server);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server)*
		/// </summary>
		public void* on_server_destroyed;

		/// <summary>
		/// Called when |server| is destroyed. The server thread will be stopped after
		/// this function returns. The client should release any references to |server|
		/// when this function is called. See OnServerCreated documentation for a
		/// description of server lifespan.
		/// </summary>
		[NativeName("on_server_destroyed")]
		public unsafe void OnServerDestroyed(cef_server_t* server)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, void>)on_server_destroyed)(self, server);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server, int connection_id)*
		/// </summary>
		public void* on_client_connected;

		/// <summary>
		/// Called when a client connects to |server|. |connection_id| uniquely
		/// identifies the connection. Each call to this function will have a matching
		/// call to OnClientDisconnected.
		/// </summary>
		[NativeName("on_client_connected")]
		public unsafe void OnClientConnected(cef_server_t* server, int connection_id)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, int, void>)on_client_connected)(self, server, connection_id);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server, int connection_id)*
		/// </summary>
		public void* on_client_disconnected;

		/// <summary>
		/// Called when a client disconnects from |server|. |connection_id| uniquely
		/// identifies the connection. The client should release any data associated
		/// with |connection_id| when this function is called and |connection_id|
		/// should no longer be passed to cef_server_t functions. Disconnects can
		/// originate from either the client or the server. For example, the server
		/// will disconnect automatically after a cef_server_t::SendHttpXXXResponse
		/// function is called.
		/// </summary>
		[NativeName("on_client_disconnected")]
		public unsafe void OnClientDisconnected(cef_server_t* server, int connection_id)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, int, void>)on_client_disconnected)(self, server, connection_id);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server, int connection_id, const cef_string_t* client_address, _cef_request_t* request)*
		/// </summary>
		public void* on_http_request;

		/// <summary>
		/// Called when |server| receives an HTTP request. |connection_id| uniquely
		/// identifies the connection, |client_address| is the requesting IPv4 or IPv6
		/// client address including port number, and |request| contains the request
		/// contents (URL, function, headers and optional POST data). Call cef_server_t
		/// functions either synchronously or asynchronusly to send a response.
		/// </summary>
		[NativeName("on_http_request")]
		public unsafe void OnHttpRequest(cef_server_t* server, int connection_id, [Immutable]cef_string_t* client_address, cef_request_t* request)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, int, cef_string_t*, cef_request_t*, void>)on_http_request)(self, server, connection_id, client_address, request);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server, int connection_id, const cef_string_t* client_address, _cef_request_t* request, _cef_callback_t* callback)*
		/// </summary>
		public void* on_web_socket_request;

		/// <summary>
		/// Called when |server| receives a WebSocket request. |connection_id| uniquely
		/// identifies the connection, |client_address| is the requesting IPv4 or IPv6
		/// client address including port number, and |request| contains the request
		/// contents (URL, function, headers and optional POST data). Execute
		/// |callback| either synchronously or asynchronously to accept or decline the
		/// WebSocket connection. If the request is accepted then OnWebSocketConnected
		/// will be called after the WebSocket has connected and incoming messages will
		/// be delivered to the OnWebSocketMessage callback. If the request is declined
		/// then the client will be disconnected and OnClientDisconnected will be
		/// called. Call the cef_server_t::SendWebSocketMessage function after
		/// receiving the OnWebSocketConnected callback to respond with WebSocket
		/// messages.
		/// </summary>
		[NativeName("on_web_socket_request")]
		public unsafe void OnWebSocketRequest(cef_server_t* server, int connection_id, [Immutable]cef_string_t* client_address, cef_request_t* request, cef_callback_t* callback)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, int, cef_string_t*, cef_request_t*, cef_callback_t*, void>)on_web_socket_request)(self, server, connection_id, client_address, request, callback);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server, int connection_id)*
		/// </summary>
		public void* on_web_socket_connected;

		/// <summary>
		/// Called after the client has accepted the WebSocket connection for |server|
		/// and |connection_id| via the OnWebSocketRequest callback. See
		/// OnWebSocketRequest documentation for intended usage.
		/// </summary>
		[NativeName("on_web_socket_connected")]
		public unsafe void OnWebSocketConnected(cef_server_t* server, int connection_id)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, int, void>)on_web_socket_connected)(self, server, connection_id);
			}
		}

		/// <summary>
		/// void (*)(_cef_server_handler_t* self, _cef_server_t* server, int connection_id, const void* data, size_t data_size)*
		/// </summary>
		public void* on_web_socket_message;

		/// <summary>
		/// Called when |server| receives an WebSocket message. |connection_id|
		/// uniquely identifies the connection, |data| is the message content and
		/// |data_size| is the size of |data| in bytes. Do not keep a reference to
		/// |data| outside of this function. See OnWebSocketRequest documentation for
		/// intended usage.
		/// </summary>
		[NativeName("on_web_socket_message")]
		public unsafe void OnWebSocketMessage(cef_server_t* server, int connection_id, [Immutable]void* data, UIntPtr data_size)
		{
			fixed (cef_server_handler_t* self = &this)
			{
				((delegate* unmanaged[Stdcall]<cef_server_handler_t*, cef_server_t*, int, void*, UIntPtr, void>)on_web_socket_message)(self, server, connection_id, data, data_size);
			}
		}
	}
}

