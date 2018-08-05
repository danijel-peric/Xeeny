using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xeeny.NamedPipes.ApiExtensions;
using Xeeny.Transports;
using Xeeny.Transports.Channels;

namespace Xeeny.NamedPipes
{
    class NamedPipeChannel : ITransportChannel
    {
        public string ConnectionName => _connectionName;
        public ConnectionSide ConnectionSide => _connectionSide;

        private readonly string _pipeName;
        private readonly string _server;
        private readonly ConnectionSide _connectionSide;
        private readonly string _connectionName;
        private readonly string _connectionId;

        private PipeStream _pipeStream;

        public NamedPipeChannel(NamedPipeServerStream serverStream, string connectionName)
        {
            _pipeStream = serverStream;
            _connectionName = connectionName;
            _connectionSide = ConnectionSide.Server;
        }

        public NamedPipeChannel(string pipeName, string server, string connectionName, string connectionId)
        {
            _pipeName = pipeName;
            _server = server;
            _connectionName = connectionName;
            _connectionId = connectionId.Replace("-", "");
            _connectionSide = ConnectionSide.Client;
        }

        public async Task Connect(CancellationToken ct)
        {
            if (_connectionSide == ConnectionSide.Client)
            {
                var dataPipeName = GenerateDataPipeName(_pipeName);

                using (var handshakePipe = NamedPipeClientStreamFactory.CreatePipe(_pipeName, _server))
                {
                    await handshakePipe.ConnectAsync(ct).ConfigureAwait(false);

                    await NamedPipeTools.WriteToPipe(handshakePipe, Encoding.UTF8.GetBytes(dataPipeName), ct);

                    handshakePipe.Close();
                }

                var dataPipe = NamedPipeClientStreamFactory.CreatePipe(dataPipeName, _server);

                await dataPipe.ConnectAsync(ct);

                _pipeStream = dataPipe;
            }
        }

        public async Task SendAsync(ArraySegment<byte> segment, CancellationToken ct)
        {
            await _pipeStream.WriteAsync(segment.Array, segment.Offset, segment.Count, ct).ConfigureAwait(false);

            _pipeStream.WaitForPipeDrain();
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> segment, CancellationToken ct)
        {
            var result = await _pipeStream.ReadAsync(segment.Array, segment.Offset, segment.Count, ct).ConfigureAwait(false);

            return result;
        }

        public Task Close(CancellationToken ct)
        {
            try
            {
                NamedPipeTools.DisposePipe(_pipeStream);
            }
            catch
            {
                //
            }

            return Task.CompletedTask;
        }

        private string GenerateDataPipeName(string pipeName)
        {
            return $"{pipeName}_{_connectionId}";
        }
    }
}
