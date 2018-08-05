using System.IO.Pipes;

namespace Xeeny.NamedPipes
{
    static class NamedPipeServerStreamFactory
    {
        public static NamedPipeServerStream CreatePipe(string pipeName, int maxNumberOfServerInstance = 1)
        {
            return new NamedPipeServerStream(pipeName, PipeDirection.InOut, maxNumberOfServerInstance, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.WriteThrough, 0, 0);
        }
    }
}