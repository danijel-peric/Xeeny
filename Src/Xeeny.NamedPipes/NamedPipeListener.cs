using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xeeny.NamedPipes.ApiExtensions;
using Xeeny.Transports;

namespace Xeeny.NamedPipes
{
    public class NamedPipeListener : IListener
    {
        private readonly NamedPipeTransportSettings _pipeSettings;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly string _pipeName;
        private readonly CancellationTokenSource _tokenSource;

        public NamedPipeListener(string pipeName, NamedPipeTransportSettings settings, ILoggerFactory loggerFactory)
        {
            _pipeName = pipeName;
            _pipeSettings = settings;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger(nameof(NamedPipeListener));
            _tokenSource = new CancellationTokenSource();
        }

        public void Listen()
        {
            // nothing to do
        }

        public async Task<ITransport> AcceptConnection()
        {
            NamedPipeServerStream dataPipe = null;

            try
            {
                string dataPipeName = await Handshake();

                dataPipe = NamedPipeServerStreamFactory.CreatePipe(dataPipeName, _pipeSettings.MaxNumberOfServerInstances);

                await dataPipe.WaitForConnectionAsync(_tokenSource.Token);

                return new NamedPipeTransport(dataPipe, _pipeSettings, _loggerFactory);

            }
            catch (Exception)
            {
                NamedPipeTools.DisposePipe(dataPipe);

                throw;
            }
        }

        private async Task<string> Handshake()
        {
            using (NamedPipeServerStream handshakePipe = NamedPipeServerStreamFactory.CreatePipe(_pipeName))
            {
                try
                {
                    await handshakePipe.WaitForConnectionAsync(_tokenSource.Token);

                    var dataPipeNameData = await NamedPipeTools.ReadFromPipe(handshakePipe, _tokenSource.Token);

                    return Encoding.UTF8.GetString(dataPipeNameData);
                }
                finally
                {
                    handshakePipe.Close();
                }
            }
        }

        public void Close()
        {
            _tokenSource.Cancel();
        }
    }
}

