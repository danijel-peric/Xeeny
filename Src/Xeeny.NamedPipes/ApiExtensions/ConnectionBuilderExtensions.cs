using System;
using Xeeny.Api.Client;
using Xeeny.NamedPipes;
using Xeeny.NamedPipes.ApiExtensions;
using Xeeny.Transports;

public static class ConnectionBuilderExtensions
{
    public static TBuilder WithNamedPipeTransport<TBuilder>(this TBuilder builder, string pipeName, string server,
        Action<NamedPipeTransportSettings> options = null)
        where TBuilder : BaseConnectionBuilder
    {
        var settings = new NamedPipeTransportSettings(ConnectionSide.Client);

        options?.Invoke(settings);

        builder.TransportFactory = new NamedPipeTransportFactory(pipeName, server, settings);

        return builder;
    }
}
