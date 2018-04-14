﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xeeny.Transports;
using Xeeny.Transports.Channels;

namespace Xeeny.Sockets.TcpSockets
{
    public class TcpSocketChannel : ITransportChannel
    {
        public ConnectionSide ConnectionSide => _connectionSide;
        public string ConnectionName => _connectionName;

        readonly Socket _socket;
        readonly IPAddress _ipAddress;
        readonly int _port;
        readonly SocketFlags _flags;
        readonly ConnectionSide _connectionSide;
        readonly string _connectionName;

        public TcpSocketChannel(Socket socket, IPAddress address, int port, SocketFlags flags, string connectionName)
        {
            _socket = socket;
            _ipAddress = address;
            _port = port;
            _flags = flags;
            _connectionName = connectionName;
            _connectionSide = ConnectionSide.Client;
        }

        public TcpSocketChannel(Socket socket, SocketFlags flags, string connectionName)
        {
            _socket = socket;
            _flags = flags;
            _connectionName = connectionName;
            _connectionSide = ConnectionSide.Server;
        }

        public async Task Connect(CancellationToken ct)
        {
            if(_connectionSide == ConnectionSide.Client)
            {
                await _socket.ConnectAsync(_ipAddress, _port);
            }
        }

        public Task SendAsync(ArraySegment<byte> segment, CancellationToken ct)
        {
            return _socket.SendAsync(segment, _flags);
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> segment, CancellationToken ct)
        {
            return _socket.ReceiveAsync(segment, _flags);
        }

        public void Close(CancellationToken ct)
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Disconnect(false);
                _socket.Close();
            }
            finally
            {
                _socket.Dispose();
            }
        }
    }
}
