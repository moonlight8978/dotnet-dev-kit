// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Workflow.API;

namespace DummyApp.Features.API;

[BindingType(typeof(IErrorCodeProvider))]
public class ErrorCodeProvider : IErrorCodeProvider, ITypeDirectedSingletonBindedService
{
    public int ValidationError => 1000;
    public int Unauthenticated => 1001;
    public int NotFound => 1002;
    public int Unauthorized => 1002;

    public bool IsClientError(int errorCode)
    {
        return errorCode is > 0 and < 100_000;
    }

    public bool IsServerError(int errorCode)
    {
        return errorCode is < 0 or > 100_000;
    }
}
