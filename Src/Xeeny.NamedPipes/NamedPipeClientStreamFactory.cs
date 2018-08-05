using System.IO.Pipes;

namespace Xeeny.NamedPipes
{
    static class NamedPipeClientStreamFactory
    {
        public static NamedPipeClientStream CreatePipe(string pipeName, string serverName)
        {
            return new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}