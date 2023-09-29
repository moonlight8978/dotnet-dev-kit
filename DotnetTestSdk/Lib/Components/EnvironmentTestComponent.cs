// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net.Http.Headers;
using MessagePack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestSdk.Lib.Components;

public interface IEnvironmentTestComponent : ITestComponent
{
    HttpClient DefaultHttpClient { get; }

    HttpClient CreateHttpClient();

    IServiceProvider ServiceProvider { get; }

    HubConnection CreateWsClient(string path, Action<IHubConnectionBuilder>? configureConnection = null,
        Action<HttpConnectionOptions>? configureUrl = null);
}

public abstract class EnvironmentTestComponent<T> : BaseTestComponent, IEnvironmentTestComponent where T : class
{
    protected WebApplicationFactory<T> _application = null!;
    private IServiceScope _serviceScope = null!;
    private HttpMessageHandler? _wsHandlerCache;

    public HttpClient DefaultHttpClient { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    protected EnvironmentTestComponent(ITestComponentManager componentManager) : base(componentManager)
    {
    }

    protected override async Task OnPrepareAsync()
    {
        _application = new WebApplicationFactory<T>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                OnConfigureWebHostBuilder(builder);
            });

        await OnConfigureApplication(_application);

        _serviceScope = _application.Services.CreateScope();
        ServiceProvider = _serviceScope.ServiceProvider;

        DefaultHttpClient = CreateHttpClient();
        await OnCreateDefaultHttpClient(DefaultHttpClient);
    }

    protected virtual Task OnConfigureApplication(WebApplicationFactory<T> application)
    {
        return Task.CompletedTask;
    }

    protected virtual void OnConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) { }

    protected override async Task OnCleanupAsync()
    {
        _serviceScope.Dispose();
        await _application.DisposeAsync();
    }

    protected virtual Task OnCreateDefaultHttpClient(HttpClient defaultHttpClient) => Task.CompletedTask;

    public HttpClient CreateHttpClient() => _application.CreateClient();

    public HubConnection CreateWsClient(string path, Action<IHubConnectionBuilder>? configureConnection = null,
        Action<HttpConnectionOptions>? configureUrl = null)
    {
        _wsHandlerCache ??= _application.Server.CreateHandler();
        var builder = new HubConnectionBuilder()
            .WithUrl(new UriBuilder(_application.Server.BaseAddress) { Scheme = "ws", Path = path }.Uri,
                options =>
                {
                    configureUrl?.Invoke(options);
                    options.Transports = HttpTransportType.WebSockets;
                    options.SkipNegotiation = true;
                    options.HttpMessageHandlerFactory = _ => _wsHandlerCache;
                    options.WebSocketFactory = async (context, token) =>
                    {
                        var wsClient = _application.Server.CreateWebSocketClient();
                        wsClient.ConfigureRequest = request =>
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                                context.Options.AccessTokenProvider?.Invoke().Result).ToString();
                        };
                        return await wsClient.ConnectAsync(context.Uri, token);
                    };
                })
            .AddMessagePackProtocol(options =>
            {
                options.SerializerOptions = MessagePackSerializerOptions.Standard;
            });

        configureConnection?.Invoke(builder);
        return builder.Build();
    }
}
