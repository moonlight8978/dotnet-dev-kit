// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetTestSdk.Lib.Components;

namespace DotnetDevelopmentSdkTests.Supports;

public class SdkExternalApiTestComponent : ExternalApiTestComponent
{
    protected override List<ProxyServerInfo> ProxyServerInfos { get; } = new()
    {
        new ProxyServerInfo()
        {
            IsGlobal = false,
            OriginalUrl = "https://google.com",
            CompressTerminationProxyUrl = "",
            CompressTerminationEnabled = false,
            LocalProxyUrl = "http://localhost:9095"
        }
    };

    public SdkExternalApiTestComponent(ITestComponentManager componentManager) : base(componentManager,
        "DotnetDevelopmentSdkTests")
    {
    }
}
