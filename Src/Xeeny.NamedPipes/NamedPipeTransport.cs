using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xeeny.Transports;
using Xeeny.Transports.Channels;

namespace Xeeny.NamedPipes
{
    class NamedPipeTransport : TransportBase
    {
        readonly IMessageChannel _channel;

        public NamedPipeTransport(NamedPipeServerStream pipeStream, NamedPipeTransportSettings settings, ILoggerFactory loggerFactory)
            : base(settings, ConnectionSide.Server, loggerFactory.CreateLogger(nameof(NamedPipeTransport)))
        {
            var transport = new NamedPipeChannel(pipeStream, this.ConnectionName);
            _channel = CreateChannel(transport, settings);
        }

        public NamedPipeTransport(string pipeName, string server, NamedPipeTransportSettings settings, ILoggerFactory loggerFactory)
            : base(settings, ConnectionSide.Client, loggerFactory.CreateLogger(nameof(NamedPipeTransport)))
        {
            var transport = new NamedPipeChannel(pipeName, server, this.ConnectionName, this.ConnectionId);
            _channel = CreateChannel(transport, settings);
        }

        IMessageChannel CreateChannel(ITransportChannel transport, NamedPipeTransportSettings settings)
        {
            var framing = settings.FramingProtocol;
            switch (framing)
            {
                case FramingProtocol.SerialFragments: return new SerialMessageStreamChannel(transport, settings);
                case FramingProtocol.ConcurrentFragments: return new ConcurrentMessageStreamChannel(transport, settings);
                default: throw new NotSupportedException(framing.ToString());
            }
        }

        protected override Task OnConnect(CancellationToken ct)
        {
            return _channel.Connect(ct);
        }

        protected override Task SendMessage(Message message, CancellationToken ct)
        {
            return _channel.SendMessage(message, ct);
        }

        protected override Task<Message> ReceiveMessage(CancellationToken ct)
        {
            return _channel.ReceiveMessage(ct);
        }

        protected override Task OnClose(CancellationToken ct)
        {
            return _channel.Close(ct);
        }

        protected override void OnKeepAlivedReceived(Message message)
        {
            //nothing
        }

        protected override void OnAgreementReceived(Message message)
        {
            //nothing
        }
    }
}
