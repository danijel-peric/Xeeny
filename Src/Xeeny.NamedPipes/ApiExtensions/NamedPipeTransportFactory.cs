using Microsoft.Extensions.Logging;
using Xeeny.Api.Client;
using Xeeny.Transports;

namespace Xeeny.NamedPipes.ApiExtensions
{
    class NamedPipeTransportFactory : ITransportFactory
    {
        private readonly NamedPipeTransportSettings _settings;
        private readonly string _pipeName;
        private readonly string _server;

        public NamedPipeTransportFactory(string pipeName, string server, NamedPipeTransportSettings settings)
        {
            _pipeName = pipeName;
            _server = server;
            _settings = settings;
        }

        public ITransport CreateTransport(ILoggerFactory loggerFactory)
        {
            return NamedPipeTools.CreateNamedPipeTransport(_pipeName, _server, _settings, loggerFactory);
        }
    }
}
