// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;
using NUnit.Framework;
using WireMock.Handlers;
using WireMock.Server;
using WireMock.Settings;

namespace DotnetTestSdk.Lib.Components;

public abstract class ExternalApiTestComponent : BaseTestComponent
{
    private readonly string _projectName;
    protected virtual List<ProxyServerInfo> ProxyServerInfos { get; } = new();

    protected ExternalApiTestComponent(ITestComponentManager componentManager, string projectName) : base(
        componentManager)
    {
        _projectName = projectName;
    }

    private readonly List<WireMockServer> _servers = new();

    protected override Task OnPrepareAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var mappingsDirectory =
            Path.Combine(currentDirectory[
                ..(currentDirectory.IndexOf(_projectName, StringComparison.InvariantCulture) +
                   _projectName.Length)], "Fixtures");

        foreach (var proxyServerInfo in ProxyServerInfos)
        {
            var url = proxyServerInfo.CompressTerminationEnabled
                ? new Uri(proxyServerInfo.CompressTerminationProxyUrl)
                : new Uri(proxyServerInfo.OriginalUrl);

            var proxyUrl = new Uri(proxyServerInfo.OriginalUrl);

            var mappingLocation = proxyServerInfo.IsGlobal
                ? Path.Combine(mappingsDirectory, "cassettes", "global", $"{proxyUrl.Host}:{proxyUrl.Port}")
                : Path.Combine(mappingsDirectory,
                    "cassettes",
                    "local",
                    TestContext.CurrentContext.Test.ClassName!,
                    TestContext.CurrentContext.Test.MethodName!,
                    $"{proxyUrl.Host}__{proxyUrl.Port}");

            var settings = new WireMockServerSettings
            {
                Urls = new[] { proxyServerInfo.LocalProxyUrl },
                ProxyAndRecordSettings =
                    new ProxyAndRecordSettings { Url = url.AbsoluteUri, SaveMapping = true, SaveMappingToFile = true },
                FileSystemHandler = new LocalFileSystemHandler(mappingLocation),
                ReadStaticMappings = true,
                ThrowExceptionWhenMatcherFails = true
            };

            var server = WireMockServer.Start(settings);
            _servers.Add(server);
        }

        return Task.CompletedTask;
    }

    protected override Task OnCleanupAsync()
    {
        _servers.ForEach(server => server.Stop());
        return Task.CompletedTask;
    }
}

[CustomConfiguration("ExternalApi")]
public class ExternalApiConfiguration : ICustomConfiguration
{
    public List<ProxyServerInfo> Servers { get; set; }
}

public class ProxyServerInfo
{
    public string OriginalUrl { get; set; } = null!;
    public string LocalProxyUrl { get; set; } = null!;
    public string CompressTerminationProxyUrl { get; set; }
    public bool CompressTerminationEnabled { get; set; }
    public bool IsGlobal { get; set; }
}
