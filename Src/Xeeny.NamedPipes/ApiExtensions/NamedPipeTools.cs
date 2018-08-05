using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xeeny.Transports;

namespace Xeeny.NamedPipes.ApiExtensions
{
    class NamedPipeTools
    {
        public static NamedPipeTransport CreateNamedPipeTransport(string pipeName, string server, NamedPipeTransportSettings settings, ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(pipeName))
            {
                throw new ArgumentNullException(nameof(pipeName));
            }

            if (string.IsNullOrEmpty(server))
            {
                server = ".";
            }

            return new NamedPipeTransport(pipeName, server, settings, loggerFactory);
        }

        public static IListener CreateNamedPipeListener(string pipeName, NamedPipeTransportSettings settings, ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(pipeName))
            {
                throw new ArgumentNullException(nameof(pipeName));
            }

            return new NamedPipeListener(pipeName, settings, loggerFactory);
        }

        public static void DisposePipe(PipeStream pipe)
        {
            if (pipe == null)
                return;
            using (var x = pipe)
                x.Close();
        }

        public static async Task<byte[]> ReadFromPipe(PipeStream pipe, CancellationToken ct)
        {
            byte[] buffer = new byte[255];

            int length = await pipe.ReadAsync(buffer, 0, buffer.Length, ct);

            byte[] chunk = new byte[length];

            Array.Copy(buffer, chunk, length);

            return chunk;
        }

        public static async Task WriteToPipe(PipeStream pipe, byte[] data, CancellationToken ct)
        {
            await pipe.WriteAsync(data, 0, data.Length, ct);

            pipe.WaitForPipeDrain();
        }
    }
}
