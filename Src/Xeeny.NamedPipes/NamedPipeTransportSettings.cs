using Xeeny.Transports;

namespace Xeeny.NamedPipes
{
    public class NamedPipeTransportSettings : TransportSettings
    {
        /// <summary>
        /// Message Framing Protocol
        /// </summary>
        public FramingProtocol FramingProtocol { get; set; }

        public int MaxNumberOfServerInstances { get; set; }

        public NamedPipeTransportSettings(ConnectionSide connectionSide) : base(connectionSide)
        {
            MaxNumberOfServerInstances = 1;
            FramingProtocol = FramingProtocol.SerialFragments;
        }
    }
}
