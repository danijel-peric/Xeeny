using System;
using Xeeny.Api.Server;
using Xeeny.NamedPipes;
using Xeeny.NamedPipes.ApiExtensions;
using Xeeny.Transports;

public static class ServiceHostBuilderExtensions
{
    public static TBuilder AddNamedPipeServer<TBuilder>(this TBuilder builder, string pipeName,
        Action<NamedPipeTransportSettings> options = null)
        where TBuilder : BaseServiceHostBuilder
    {
        var settings = new NamedPipeTransportSettings(ConnectionSide.Server);
        options?.Invoke(settings);
        var listener = NamedPipeTools.CreateNamedPipeListener(pipeName, settings, builder.LoggerFactory);
        builder.Listeners.Add(listener);
        return builder;
    }
}
